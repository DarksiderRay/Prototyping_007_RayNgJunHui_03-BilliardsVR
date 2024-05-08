Shader "Environment/S_ProceduralGrid"
{
    Properties
    {
        [Header(Global Grid)]
        [Space(10)]
        _LineThickness ("Line Thickness", float) = .1
        _XAxisColor ("X-Axis Color", color) = (1, 0, 0, 1)
        _XAxisLineMult ("X-Axis Line Mult", float) = 2
        _ZAxisColor ("Z-Axis Color", color) = (0, 0, 1, 1)
        _ZAxisLineMult ("Z-Axis Line Mult", float) = 2
        
        [Header(Primary Grid)]
        [Space(10)]
        _PrimaryGridColor ("Grid Color", color) = (1, 1, 1, 1)
        _PrimaryGridSpacing ("Grid Spacing", float) = 10
        _PrimaryFadeDistanceStart ("Fade Distance (Start)", float) = 0
        _PrimaryFadeDistanceEnd ("Fade Distance (End)", float) = 100

        [Header(Secondary Grid)]
        [Space(10)]
        _SecondaryGridColor ("Grid Color", color) = (0.75, 0.75, 0.75, 1)
        _SecondaryGridSpacing ("Grid Spacing", float) = 1
        _SecondaryFadeDistanceStart ("Fade Distance (Start)", float) = 0
        _SecondaryFadeDistanceEnd ("Fade Distance (End)", float) = 40
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off 
        Cull Off // Double-sided command

         HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // Declare non-texture properties for SRP Batcher
            CBUFFER_START(UnityPerMaterial)
                float   _LineThickness;
                float4  _XAxisColor;
                float   _XAxisLineMult;
                float4  _ZAxisColor;
                float   _ZAxisLineMult;
                float4  _PrimaryGridColor;
                float   _PrimaryGridSpacing;
                float   _PrimaryFadeDistanceStart;
                float   _PrimaryFadeDistanceEnd;
                float4  _SecondaryGridColor;
                float   _SecondaryGridSpacing;
                float   _SecondaryFadeDistanceStart;
                float   _SecondaryFadeDistanceEnd;
            CBUFFER_END

        ENDHLSL

        Pass
        {
            HLSLPROGRAM
                
                // Tell HLSL what the vertex and fragment shaders are named
                #pragma vertex vert
                #pragma fragment frag

                //////////////////////////////////////
                // Declare structs for vertex (VertexInput) and fragment (VertexOutput) shaders
                struct VertexInput
                {
                    float4 position : POSITION;
                    float2 uv : TEXCOORD0;
                };
                struct VertexOutput
                {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                };
    
                //////////////////////////////////////
                // Function to make grid
                float4 GridMask (VertexOutput i, float gridSpacing, float fadeDistanceStart, float fadeDistanceEnd)
                {
                    // Generate grid from uv
                    i.uv = (i.worldPos.xz / gridSpacing) - 0.5f; // change object UV to world UV then scale to gridsize
                    float2 wrapped = frac(i.uv) - 0.5f; // repeat UVs
                    float2 range = abs(wrapped); // get absolute values. 0-1 repeating instead of 0-1,1-2,2-3 ...

                    // Get screen-space derivatives (somehow; google Manhattan Norm)
                    float2 speeds;
                    speeds = fwidth(i.uv);

                    // Get lineWeight based on screen-space (somehow; Cheap AA)
                    float2 pixelRange = range/speeds;
                    float lineWeight = saturate(min(pixelRange.x, pixelRange.y) - _LineThickness);
                    
                    // Blend
                    float4 gridMask = lerp(1, 0, lineWeight);
                
                    // Distance falloff
                    half3 viewDirW = _WorldSpaceCameraPos - i.worldPos;
                    half viewDist = length(viewDirW);
                    half falloff = saturate((viewDist - fadeDistanceStart) / (fadeDistanceEnd - fadeDistanceStart) );
                    gridMask.a *= (1.0f - falloff);

                    return saturate(gridMask);
                }

                //////////////////////////////////////
                // Function to make axis
                float4 AxisMask (VertexOutput i, float2 uvAxis, float axisMult)
                {
                    // Triangle wave pattern
                    float2 range = abs(uvAxis * 2 -1);

                    // Get screen-space derivatives (somehow; google Manhattan Norm)
                    float2 speeds;
                    speeds = fwidth(uvAxis);

                    // Get lineWeight based on screen-space (somehow; Cheap AA)
                    float2 pixelRange = range/speeds;
                    float lineWeight = saturate(min(pixelRange.x, pixelRange.y) - _LineThickness * 4 * axisMult);
                    
                    float4 axisMask = lerp(1, 0, lineWeight);

                    // Distance falloff
                    half3 viewDirW = _WorldSpaceCameraPos - i.worldPos;
                    half viewDist = length(viewDirW);
                    half falloff = saturate((viewDist - _PrimaryFadeDistanceStart - 50) / (_PrimaryFadeDistanceEnd - _PrimaryFadeDistanceStart - 50) );
                    axisMask.a *= (1.0f - falloff);
                    
                    return saturate(axisMask);
                }

                //////////////////////////////////////
                // Vertex Shader
                VertexOutput vert (VertexInput i)
                {
                    VertexOutput o;
                    o.position = TransformObjectToHClip(i.position.xyz);
                    o.worldPos = TransformObjectToWorld(i.position.xyz);
                    o.uv = i.uv;
                    return o;
                }

                //////////////////////////////////////
                // Fragment (Pixel) Shader
                float4 frag (VertexOutput i) : SV_Target
                {   
                    float4 primaryGridMask = GridMask
                    (
                        i,
                         _PrimaryGridSpacing, 
                         _PrimaryFadeDistanceStart, 
                         _PrimaryFadeDistanceEnd
                    );
                    float4 secondaryGridMask = GridMask
                    (
                        i, 
                        _SecondaryGridSpacing, 
                        _SecondaryFadeDistanceStart, 
                        _SecondaryFadeDistanceEnd
                    );

                    i.uv = i.worldPos.xz + 0.5f;  

                    float4 xAxisMask = AxisMask(i, i.uv.y, _XAxisLineMult);
                    float4 zAxisMask = AxisMask(i, i.uv.x, _ZAxisLineMult);
                    
                    //////////////////////////////////////
                    // Start blending everything together.

                    float4 o, gridColor, primaryGridColor, secondaryGridColor;

                    // subtract primarygrid from secondarygrid,
                    // then axismasks from primarygrid
                    // so additive blending works properly where they overlap.
                    // Saturate (clamp) to prevent negative values.
                    secondaryGridMask = saturate(secondaryGridMask - primaryGridMask);
                    primaryGridMask = saturate(primaryGridMask - (xAxisMask + zAxisMask));

                    primaryGridColor = _PrimaryGridColor * primaryGridMask;
                    secondaryGridColor = _SecondaryGridColor * secondaryGridMask;

                    gridColor = secondaryGridColor + primaryGridColor;
                    
                    o = lerp(lerp(gridColor, _ZAxisColor, zAxisMask), _XAxisColor, xAxisMask);

                    return o;
                }

            ENDHLSL
        }
    }
}