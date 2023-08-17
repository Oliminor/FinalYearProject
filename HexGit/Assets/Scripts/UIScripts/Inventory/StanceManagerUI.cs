using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StanceManagerUI : MonoBehaviour
{    
    [SerializeField] private GameObject gemSocket;
    [SerializeField] private RectTransform stanceGemSlots;
    [SerializeField] private TextMeshProUGUI EquippedGemDetails;


    private void Start()
    {
        InstantiateSlots();
    }

    /// <summary>
    /// Updates the gem lists based on the player modification
    /// </summary>
    public void CheckGemSlots()
    {
        int activePageIndex = GameManager.instance.HUDManager.InventoryItems.GetStanceIndex();

        for (int i = 0; i < stanceGemSlots.childCount; i++)
        {
            if (stanceGemSlots.GetChild(i).transform.childCount < 1)
            {
                GameManager.instance.DataManager.GetStanceGemList()[activePageIndex][i].index = 0;
                GameManager.instance.DataManager.GetStanceGemList()[activePageIndex][i].rarity = 0;
                continue;
            }
            int gemIndex = stanceGemSlots.transform.GetChild(i).GetChild(0).GetComponent<BaseIconUI>().GetIndex();
            int gemRarity = stanceGemSlots.transform.GetChild(i).GetChild(0).GetComponent<BaseIconUI>().GetRarityIndex();

            GameManager.instance.DataManager.SetStanceGemList(activePageIndex, i, gemIndex, gemRarity);
        }

        UpdateDescription();
    }

    /// <summary>
    /// Updates the left side description if the gems are changed around
    /// </summary>
    private void UpdateDescription()
    {
        EquippedGemDetails.text = "";
        EquippedGemDetails.text += GetStanceDescription() + "\n";
        EquippedGemDetails.text += GetGemDescription();

        UpdateSlots();
    }

    /// <summary>
    /// Gets the stance descriptions from the datamanager
    /// </summary>
    public string GetStanceDescription()
    {
        int activePageIndex = GameManager.instance.HUDManager.InventoryItems.GetStanceIndex();

        int stanceDescriptionIndex = 0;

        switch (activePageIndex)
        {
            case 0:
                stanceDescriptionIndex = 90000;
                break;
        }

        string text = "";

        if (stanceDescriptionIndex == 0) return text;

        text += GameManager.instance.DataManager.GetDescription(stanceDescriptionIndex, 0, 0, 1) + "\n";
        text += GameManager.instance.DataManager.GetDescription(stanceDescriptionIndex, 0, 1, 1) + "\n";
        text += GameManager.instance.DataManager.GetDescription(stanceDescriptionIndex, 0, 2, 1) + "\n";

        return text;
    }

    /// <summary>
    /// Gets the gem descriptions from the datamanager
    /// </summary>
    public string GetGemDescription()
    {
        int activePageIndex = GameManager.instance.HUDManager.InventoryItems.GetStanceIndex();

        List<Inventory> duplicateGemList = new(GetGemDuplicationList(activePageIndex, false));

        string s = "";

        for (int i = 0; i < duplicateGemList.Count; i++)
        {
            if (duplicateGemList[i].index <= 0) continue;

            s += GameManager.instance.DataManager.GetDescription(duplicateGemList[i].index, duplicateGemList[i].rarity, 0, duplicateGemList[i].quantity) + "\n";

        }

        // if there are more at least 2 identical gems 
        for (int i = 0; i < duplicateGemList.Count; i++)
        {
            int _gemIndex = duplicateGemList[i].index;
            int _gemRarity = duplicateGemList[i].rarity;
            if (duplicateGemList[i].quantity >= 2) s += GameManager.instance.DataManager.GetDescription(_gemIndex, _gemRarity, 1, 0) + "\n";
        }


        duplicateGemList = new(GetGemDuplicationList(activePageIndex, true));

        // if there are more at least 4 identical gems 
        for (int i = 0; i < duplicateGemList.Count; i++)
        {
            int _gemIndex = duplicateGemList[i].index;
            int _gemRarity = duplicateGemList[i].rarity;
            if (duplicateGemList[i].quantity >= 4) s += GameManager.instance.DataManager.GetDescription(_gemIndex, _gemRarity, 2, 0) + "\n";
        }

        return s;
    }

    /// <summary>
    /// Calculates the gem duplicate numbers
    /// </summary>
    public List<Inventory> GetGemDuplicationList(int _index, bool _setBonus)
    {
        List<Inventory> gemList = new (GameManager.instance.DataManager.GetStanceGemList()[_index]);

        // Delete the invalid gem index from the gemList
        for (int i = 0; i < gemList.Count; i++)
        {
            if (gemList[i].index <= 0)
            {
                gemList.Remove(gemList[i]);
                i--;
            }
        }

        Dictionary<string, int> shoppingDictionary = new();

        // Sorting the gems by type
        for (int i = 0; i < gemList.Count; i++)
        {
            if (!shoppingDictionary.ContainsKey(gemList[i].ToString()))
            {
                shoppingDictionary.Add(gemList[i].ToString(), 1);
            }
            else
            {
                shoppingDictionary[gemList[i].ToString()]++;
            }
        }

        List<Inventory> counterList = new();

        // Put thre result inside a List
        for (int i = 0; i < gemList.Count; i++)
        {
            if (shoppingDictionary.ContainsKey(gemList[i].ToString()))
            {
                counterList.Add(new Inventory(gemList[i].index, shoppingDictionary[gemList[i].ToString()], gemList[i].rarity));
                shoppingDictionary.Remove(gemList[i].ToString());
            }
        }

        // This part only active if the set bonus is matter (set bonus only active if the type and rarity is same)
        if (_setBonus)
        {
            for (int i = 0; i < counterList.Count; i++)
            {
                if (counterList[i].quantity < 4)
                {
                    counterList.RemoveAt(i);
                    i--;
                }
            }

            int gemRarity;

            for (int i = 0; i < counterList.Count; i++)
            {
                gemRarity = counterList[i].rarity;

                for (int j = 0; j < counterList.Count; j++)
                {
                    if (counterList[i].index != counterList[j].index) continue;

                    if (gemRarity <= counterList[j].rarity)
                    {
                        gemRarity = counterList[j].rarity;
                        if (j > 0) counterList.RemoveAt(j - 1);
                    }
                    else
                    {
                        counterList.RemoveAt(j);
                        j--;
                    }
                }
            }

            return counterList;
        }

        return counterList;
    }

    /// <summary>
    /// Instantiate gems inside the inventory
    /// </summary>
    private void InstantiateGems(int _index)
    {
        for (int i = 0; i < stanceGemSlots.childCount; i++)
        {
            if (stanceGemSlots.GetChild(i).childCount > 0) Destroy(stanceGemSlots.GetChild(i).GetChild(0).gameObject);
        }

        for (int i = 0; i < GameManager.instance.DataManager.GetStanceGemList()[_index].Count; i++)
        {
            if (GameManager.instance.DataManager.GetStanceGemList()[_index][i].index == 0) continue;

            GameObject go = Instantiate(GameManager.instance.HUDManager.InventoryItems.GetDefaultGem(), stanceGemSlots.GetChild(i));

            go.GetComponent<BaseIconUI>().SetIconData(GameManager.instance.DataManager.GetStanceGemList()[_index][i]);
        }
    }

    /// <summary>
    /// Instantiates the gem slots in a multiple circle pattern
    /// </summary>
    public void InstantiateSlots()
    {
        int radius = 110;
        int elements = 5;
        int layer = 3;
        Vector2 offset = new Vector2(0, -50);

        float angleStep = 360f / elements;
        float angle = 0f;
        for (int j = 0; j < layer; j++)
        {
            for (int i = 0; i < elements; i++)
            {
                GameObject go = Instantiate(gemSocket.gameObject, stanceGemSlots.localPosition, Quaternion.identity, stanceGemSlots);
                Vector2 pos = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * radius;
                go.GetComponent<RectTransform>().anchoredPosition = pos + offset;
                angle += angleStep;
            }
            elements += 5;
            radius += 110;

            angleStep = 360f / elements;
            angle = 0f;
        }
    }

    /// <summary>
    /// Acticate and deactivate slats based on the character level
    /// </summary>
    private void UpdateSlots()
    {
        int level = MainCharacterStats.instance.GetLevel();

        if (stanceGemSlots.childCount < 1) return;

        for (int i = 0; i < stanceGemSlots.childCount; i++)
        {
            if (i < level)
            {
                stanceGemSlots.transform.GetChild(i).gameObject.tag = "GemSocket";
                stanceGemSlots.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
            else
            {
                stanceGemSlots.transform.GetChild(i).gameObject.tag = "Untagged";
                stanceGemSlots.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
            }
        }
    }

    /// <summary>
    /// Load the gem page when the player changes to other stances inside the inventory
    /// </summary>
    public void LoadGemPage(int _index)
    {
        InstantiateGems(_index);
        UpdateDescription();
    }
}
