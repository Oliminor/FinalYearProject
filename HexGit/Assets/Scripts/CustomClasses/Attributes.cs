using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attributes
{
    public int flatHealth;
    public int flatStamina;
    public int flatMana;
    public int flatAttack;
    public int flatDef;
    public float attackSpeed;
    public float criticalDamageRate;
    public float criticalStrikeRate;
    public float lifestealPercent;
    public float addDamageBonusPercent;

    /// <summary>
    /// Increase the attribute element based on the Index
    /// </summary>
    public void AddAttributes(int _attributeIndex, float attributeValue)
    {
        switch (_attributeIndex)
        {
            case 1:
                flatAttack += (int)attributeValue;
                break;
            case 2:
                criticalDamageRate += attributeValue / 100;
                break;
            case 3:
                criticalStrikeRate += attributeValue;
                break;
            case 4:
                lifestealPercent += attributeValue;
                break;
            case 5:
                addDamageBonusPercent += attributeValue / 100;
                break;
            case 6:
                attackSpeed += attributeValue / 100;
                break;
            case 7:
                flatDef += (int)attributeValue;
                break;
            case 8:
                flatMana += (int)attributeValue;
                break;
            case 9:
                flatStamina += (int)attributeValue;
                break;
            case 10:
                flatHealth += (int)attributeValue;
                break;
        }
    }

    // Default constructor
    public Attributes() { ResetAttributes(); }

    // Constructor to set every element
    public Attributes(int _flatHealth, int _flatStamina, int _flatMana, int _flatAttack,
        int _flatDef, float _attackSpeed, float _criticalDamageRate, float _criticalStrikeRate, float _lifestealPercent, float _addDamageBonusPercent)
    {
        flatHealth = _flatHealth;
        flatStamina = _flatStamina;
        flatMana = _flatMana;
        flatAttack = _flatAttack;
        flatDef = _flatDef;
        attackSpeed = _attackSpeed;
        criticalDamageRate = _criticalDamageRate;
        criticalStrikeRate = _criticalStrikeRate;
        lifestealPercent = _lifestealPercent;
        addDamageBonusPercent = _addDamageBonusPercent;
    }

    // Operator Overloading
    public static Attributes operator +(Attributes a, Attributes b)
    {
        return new Attributes(a.flatHealth + b.flatHealth, a.flatStamina + b.flatStamina, a.flatMana + b.flatMana, a.flatAttack + b.flatAttack,
             a.flatDef + b.flatDef, a.attackSpeed + b.attackSpeed, a.criticalDamageRate + b.criticalDamageRate, a.criticalStrikeRate + b.criticalStrikeRate,
             a.lifestealPercent + b.lifestealPercent, a.addDamageBonusPercent + b.addDamageBonusPercent);
    }

    /// <summary>
    /// Sets the Attributes to zero
    /// </summary>
    public void ResetAttributes()
    {
        flatHealth = 0;
        flatStamina = 0;
        flatMana = 0;
        flatAttack = 0;
        flatDef = 0;
        criticalDamageRate = 0.0f;
        criticalStrikeRate = 0;
        lifestealPercent = 0;
        addDamageBonusPercent = 0.0f;
        attackSpeed = 0.0f;
    }
}