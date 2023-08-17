using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackModule", menuName = "ScriptableObjects/PlayerAttackModule/AttackModule")]
public class AttackModule : ScriptableObject
{
    [SerializeField] int index;
    [SerializeField] int comboIndex;
    [SerializeField] private AnimationClip animation;

    public int Index { get { return index; } }
    public int ComboIndex { get { return comboIndex; } }
    public string AnimationName { get { return animation.name; } }
}
