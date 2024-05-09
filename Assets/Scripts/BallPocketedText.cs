using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BallPocketedText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    [Header("Movement")]
    [SerializeField] private float moveOffset = 0.5f;
    [SerializeField] private float fadeDuration = 5f;
    private string textFormat = "Ball {0}\npocketed!";
    
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveY(transform.position.y + moveOffset, fadeDuration);
        text.DOFade(0, fadeDuration).OnComplete(() => Destroy(gameObject));
    }
    
    public void SetText(int ballNo)
    {
        text.text = string.Format(textFormat, ballNo);
    }
}
