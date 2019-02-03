using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Animator animator;
    private Text displayText;

    public void OnEnable()
    {
        AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfos[0].clip.length - 0.1f);
        displayText = animator.GetComponent<Text>();
    }

    public void SetText(string text, Color color, int fontSize = 12)
    {
        if (color != null)
            displayText.color = color;
        displayText.fontSize = fontSize;
        displayText.text = text;
    }
}
