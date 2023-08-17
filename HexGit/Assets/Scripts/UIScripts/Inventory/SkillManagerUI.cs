using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform skillSlots;

    private void Start()
    {
        InstantiateSkills(0);
    }

    /// <summary>
    /// Instantiates the skill icons
    /// </summary>
    private void InstantiateSkills(int _index)
    {
        ClearSkillSlots();

        List<EquippeSkills> activatedSkills = new List<EquippeSkills>(GameManager.instance.DataManager.GetSkillTreeList());

        for (int i = 0; i < activatedSkills.Count; i++)
        {
            if (activatedSkills[i].pageIndex != _index) continue;

            GameObject go = Instantiate(GameManager.instance.HUDManager.InventoryItems.GetDefaultSkill(), skillSlots);
            go.GetComponent<BaseIconUI>().SetIconData(GameManager.instance.DataManager.GetItemDatabase()[activatedSkills[i].skillIndex]);
        }
    }

    /// <summary>
    /// Clears the skill slots
    /// </summary>
    private void ClearSkillSlots()
    {
        for (int i = 0; i < skillSlots.childCount; i++)
        {
            Destroy(skillSlots.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// Load the selected stance skills
    /// </summary>
    public void LoadSkillPage(int _index)
    {
        InstantiateSkills(_index);
    }
}

[System.Serializable]
public class EquippeSkills
{
    public int pageIndex;
    public int skillIndex;
    public bool isActivated;

    public EquippeSkills(int _pageIndex, int _skillIndex, bool _isActivated)
    {
        pageIndex = _pageIndex;
        skillIndex = _skillIndex;
        isActivated = _isActivated;
    }
}