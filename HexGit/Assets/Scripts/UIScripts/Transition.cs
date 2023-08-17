using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    private Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TriggerTransitionOn()
    {
        anim.Play("TransitionOn");
    }

    public void TriggerTransitionOff()
    {
        anim.Play("TransitionOff");
    }

}
