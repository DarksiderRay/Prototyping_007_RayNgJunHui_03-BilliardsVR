using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CueStick : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable xrGrabInteractable;
    [SerializeField] private IXRSelectInteractor primaryXRInteractor;
    [SerializeField] private IXRSelectInteractor secondaryXRInteractor;
    [SerializeField] private Transform primaryAttachTransform;

    private bool lockUpDir = false;
    private Vector3 upDir;

    private bool cueStickOnAim = false;
    private Vector3 cueStickInitialPos;
    private Vector3 cueStickDir;
    private Vector3 primaryXRInteractorInitPos;
    
    void Awake()
    {
        xrGrabInteractable.selectEntered.AddListener(arg0 =>
        {
            AddXRInteractor(arg0.interactorObject);
            Debug.Log($"Select Entered: {arg0.interactorObject.transform.name}");
        });
        xrGrabInteractable.selectExited.AddListener(arg0 =>
        {
            RemoveXRInteractor(arg0.interactorObject);
            Debug.Log($"Select Exited: {arg0.interactorObject.transform.name}");
        });
        xrGrabInteractable.activated.AddListener(arg0 =>
        {
            // lockUpDir = false;
            // upDir = transform.up;
            xrGrabInteractable.trackPosition = false;
            xrGrabInteractable.trackRotation = false;
            InitializeAim();
            
            Debug.Log($"Activated: {arg0.interactorObject.transform.name}");
        });
        xrGrabInteractable.deactivated.AddListener(arg0 =>
        {
            //lockUpDir = true;
            xrGrabInteractable.trackPosition = true;
            xrGrabInteractable.trackRotation = true;
            cueStickOnAim = false;
            
            Debug.Log($"Deactivated: {arg0.interactorObject.transform.name}");
        });
    }

    void Update()
    {
        if (lockUpDir)
            transform.up = upDir;

        if (cueStickOnAim && primaryXRInteractor != null)
        {
            if (primaryXRInteractor == null)
            {
                cueStickOnAim = false;
                return;
            }
            
            transform.position = cueStickInitialPos;
            transform.up = cueStickDir;

            var cueStickNormal = Vector3.Cross(transform.up, transform.forward);
            var dirFromHandToStick = primaryXRInteractor.transform.position - primaryAttachTransform.position;

            var alignedDir = Vector3.ProjectOnPlane(dirFromHandToStick, cueStickNormal);
            alignedDir = Vector3.ProjectOnPlane(alignedDir, transform.forward);

            var dist = alignedDir.magnitude;
            if (Vector3.Dot(alignedDir.normalized, transform.up) < 0)
            {
                dist = -dist;
            }

            transform.position += dist * transform.up;
        }
    }

    void AddXRInteractor(IXRSelectInteractor newXRInteractor)
    {
        if (primaryXRInteractor == null)
        {
            primaryXRInteractor = newXRInteractor;
        }
        else
        {
            secondaryXRInteractor = newXRInteractor;
        }
        
        //Debug.Log("Primary: " + primaryXRInteractor + " | Secondary: " + secondaryXRInteractor);
    }

    void RemoveXRInteractor(IXRSelectInteractor removedXRInteractor)
    {
        if (removedXRInteractor == secondaryXRInteractor)
        {
            secondaryXRInteractor = null;
        }
        else if (removedXRInteractor == primaryXRInteractor)
        {
            if (secondaryXRInteractor == null)
            {
                primaryXRInteractor = null;
            }
            else
            {
                primaryXRInteractor = secondaryXRInteractor;
                secondaryXRInteractor = null;
            }
        }
        
        //Debug.Log("Primary: " + primaryXRInteractor + " | Secondary: " + secondaryXRInteractor);
    }

    private void InitializeAim()
    {
        cueStickOnAim = true;
        cueStickInitialPos = transform.position;
        cueStickDir = transform.up;
        primaryXRInteractorInitPos = primaryXRInteractor.transform.position;
    }
}
