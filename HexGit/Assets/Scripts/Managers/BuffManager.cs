using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffTrigger { NotBuff = 0, BaseAttack = 1, SkillAttack = 2, EnemyEliminiate = 3, ActivateSkillPage = 4 }

public class BuffManager : MonoBehaviour
{
    private DataManager dataManager;
    private CooldownManager cooldownManager;

    void Start()
    {
        dataManager = GameManager.instance.DataManager;
        cooldownManager = GameManager.instance.CooldownManager;
    }

    /// <summary>
    /// Triggers the gem buffs
    /// </summary>
    public void BaseGemBuffTrigger(int _activeStanceIndex, BuffTrigger _buffTrigger)
    {
        List<Inventory> duplicateGemList = new(GameManager.instance.HUDManager.StanceManagerUI.GetGemDuplicationList(_activeStanceIndex, true));

        for (int i = 0; i < duplicateGemList.Count; i++)
        {
            if (GameManager.instance.DataManager.GetItemDatabase()[duplicateGemList[i].index].buffTriggerIndex != (int)_buffTrigger) continue;
            StartCoroutine(BasicBuff(duplicateGemList[i].index, duplicateGemList[i].rarity,  true));
        }
    }

    /// <summary>
    /// Triggers the melee stance buff (when stance is change from anything to melee stance)
    /// </summary>
    public void TriggerMeleeStanceBuff() { StartCoroutine(BasicBuff(90000, 0, false)); }

    // Activates Gem buff
    private IEnumerator BasicBuff(int _index, int _rarity, bool _isGem)
    {
        if (!cooldownManager.IsCooldownFinished(_index)) yield break;

        cooldownManager.AddCooldown(_index, dataManager.GetCooldown(_index)); // Add the cooldown time and index to the cooldown Manager

        MainCharacterStats.instance.SetBuffAdditionalAtrributes(_index, false, _isGem); // Sets the attributes accordingly to the gem stats

        GameObject go = InstantiateBuffIcon(_index, _rarity, _isGem); // Instantiate the buff icon on the screen for the player

        yield return new WaitForSeconds(dataManager.GetDuration(_index));

        MainCharacterStats.instance.SetBuffAdditionalAtrributes(_index, true, _isGem); // After the cooldown owner, the cooldown is removed

        Destroy(go.gameObject); // The buff icon is also removed
    }

    // Instantiates the buff icon and sets the icon elements (UPDATE)
    private GameObject InstantiateBuffIcon(int _index, int _rarity, bool _isGem)
    {
        GameObject go = Instantiate(GameManager.instance.HUDManager.InventoryItems.GetDefaultBuff(), GameManager.instance.HUDManager.GetBuffIconHolder());
        Inventory buffIconData = new(_index, 0, _rarity);
        go.GetComponent<BaseIconUI>().SetIconData(buffIconData);
        go.GetComponent<BaseIconUI>().SetIsGemBuff(_isGem);

        return go;
    }
}
