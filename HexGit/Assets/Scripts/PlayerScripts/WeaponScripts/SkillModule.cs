using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillModule", menuName = "ScriptableObjects/PlayerAttackModule/SkillModule")]

public class SkillModule : ScriptableObject
{
    [SerializeField] int index;
    [SerializeField] List<AttackModule> attackModules;

    public int Index { get { return index; } }
    public List<AttackModule> AttackModuleList { get { return attackModules; } }
}
