using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AreaTransitionText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI areaNameText;

    private Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    /// <summary>
    /// Trigges the transition animation
    /// </summary>
    public void TriggerAreaTextTransition()
    {
        //areaNameText.text = _areaName;
        anim.SetTrigger("TriggerText");
    }

}
