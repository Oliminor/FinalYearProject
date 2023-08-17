using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CooldownManager : MonoBehaviour
{
    Dictionary<int, float> cooldownDictionary = new();

    void Awake()
    {
        MainCharacterStats.onGameOver += ResetCooldowns;
    }

    void Update()
    {
        CooldownDecrease();
    }

    /// <summary>
    /// Resets every cooldown to 0
    /// </summary>
    private void ResetCooldowns()
    {
        foreach (var item in cooldownDictionary.ToList())
        {
            cooldownDictionary[item.Key] = 0;
        }
    }

    /// <summary>
    /// Returns true if the cooldown is finished (not in the dictionary anymore)
    /// </summary>
    public bool IsCooldownFinished(int _index)
    {
        if (!cooldownDictionary.ContainsKey(_index)) return true;

        return false;
    }

    /// <summary>
    /// Returns the remaining time from the cooldown
    /// </summary>
    public float GetCooldowTimeLeft(int _index)
    {
        if (!cooldownDictionary.ContainsKey(_index)) return 0;

        return cooldownDictionary[_index];
    }

    /// <summary>
    /// Returns the remaining normlazied time from the cooldown (between 0 and 1) mainly for UI elements to visualize
    /// </summary>
    public float GetCooldownTImeLeftNormalized(int _index)
    {
        float maxCooldown = GameManager.instance.DataManager.GetCooldown(_index);
        float cooldownLeft = GetCooldowTimeLeft(_index);

        return 1.0f - cooldownLeft / maxCooldown;
    }

    /// <summary>
    /// Run through every cooldown and decrease the time (Update them)
    /// </summary>
    private void CooldownDecrease()
    {
        List<int> ZeroValueList = new();

        foreach (var item in cooldownDictionary.ToList())
        {
            cooldownDictionary[item.Key] -= Time.deltaTime;

            if (cooldownDictionary[item.Key] <= 0) ZeroValueList.Add(item.Key);
        }

        for (int i = ZeroValueList.Count - 1; i >= 0; i--)
        {
            cooldownDictionary.Remove(ZeroValueList[i]);
        }
    }

    /// <summary>
    /// Add a new cooldown to the system
    /// </summary>
    public void AddCooldown(int _index, float _time)
    {
        if (!cooldownDictionary.ContainsKey(_index))
        {
            cooldownDictionary.Add(_index, _time);
        }
    }
}
