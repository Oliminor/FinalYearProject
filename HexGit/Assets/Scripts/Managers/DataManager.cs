using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.InputSystem;

public enum ItemType
{
    Gem = 1
}

public class DataManager : MonoBehaviour
{
    private Dictionary<int, ItemData> itemDatabase = new();
    private List<List<Inventory>> stanceGemList = new();
    private List<EquippeSkills> SkillTree = new();

    public List<EquippeSkills> GetSkillTreeList() { return SkillTree; }
    public List<List<Inventory>> GetStanceGemList() { return stanceGemList; }
    public Dictionary<int, ItemData> GetItemDatabase() { return itemDatabase; }

    void Awake()
    {
        LoadData("/StreamingAssets/GemData.Json", "GemData");
        LoadData("/StreamingAssets/SkillData.Json", "SkillData");
        LoadData("/StreamingAssets/StanceData.Json", "StanceData");
    }

    private void Start()
    {
        LoadInventory();
        LoadStancePages();
        LoadSkillTree();

         GameManager.instance.InputManager.GetInputActions().UI.SaveGame.performed += SaveGame;
    }

    private void SaveGame(InputAction.CallbackContext context)
    {
        SaveInventory();
        SaveStancePages();
        SaveSkillTree();

        Debug.Log("Game Saved");
    }

    /// <summary>
    /// Saves the Inventory elements
    /// </summary>
    private void SaveInventory()
    {
        InventoryList inventoryList = new();

        for (int i = 0; i < GameManager.instance.HUDManager.InventoryItems.GetGemInventoryContent().transform.childCount; i++)
        {
            BaseIconUI baseIcon = GameManager.instance.HUDManager.InventoryItems.GetGemInventoryContent().transform.GetChild(i).gameObject.GetComponent<BaseIconUI>();

            inventoryList.inventory.Add(new Inventory(0,0,0)
            {
                index = baseIcon.GetIndex(),
                quantity = baseIcon.GetQuantity(),
                rarity = baseIcon.GetRarityIndex()
            });

            string json = JsonUtility.ToJson(inventoryList, true);

            File.WriteAllText(Application.dataPath + "/StreamingAssets/InventoryData.Json", json);
        }
    }

    /// <summary>
    /// Saves the stance gem pages
    /// </summary>
    private void SaveStancePages()
    {
        StanceGemList stanceGemList = new();

        for (int i = 0; i < GetStanceGemList().Count; i++)
        {
            for (int j = 0; j < GetStanceGemList()[i].Count; j++)
            {
                Inventory gem = GetStanceGemList()[i][j];

                stanceGemList.stanceGem.Add(new StanceGem()
                {
                    pageIndex = i,
                    slotIndex = j,
                    gemIndex = gem.index,
                    gemRarity = gem.rarity
                });

                string json = JsonUtility.ToJson(stanceGemList, true);

                File.WriteAllText(Application.dataPath + "/StreamingAssets/StanceGemPageData.Json", json);
            }
        }
    }

    /// <summary>
    /// Save skilltree elements
    /// </summary>
    private void SaveSkillTree()
    {
        SkillTreeList skillTreeList = new();

        for (int i = 0; i < GetSkillTreeList().Count; i++)
        {
            EquippeSkills skills = GetSkillTreeList()[i];

            skillTreeList.skillTree.Add(new SkillTreeData()
            {
                pageIndex = skills.pageIndex,
                skillIndex = skills.skillIndex,
                skillIsActivated = skills.isActivated
            });

            string json = JsonUtility.ToJson(skillTreeList, true);

            File.WriteAllText(Application.dataPath + "/StreamingAssets/SkillTreeData.Json", json);
        }
    }

    /// <summary>
    /// Loads the skill tree elements
    /// </summary>
    public void LoadSkillTree()
    {
        if (!File.Exists(Application.dataPath + "/StreamingAssets/SkillTreeData.Json")) return;

        SkillTreeList loadedStancePageData = new();

        string saveString = File.ReadAllText(Application.dataPath + "/StreamingAssets/SkillTreeData.Json");

        loadedStancePageData = JsonUtility.FromJson<SkillTreeList>(saveString);

        for (int i = 0; i < loadedStancePageData.skillTree.Count; i++)
        {
            int pageIndex = loadedStancePageData.skillTree[i].pageIndex;
            int skillIndex = loadedStancePageData.skillTree[i].skillIndex;
            bool skillIsActivated = loadedStancePageData.skillTree[i].skillIsActivated;

            SetSkillTreeList(pageIndex, skillIndex, skillIsActivated);
        }

        Debug.Log("SkillTree loaded");
    }

    /// <summary>
    /// Loads the skill stance pages
    /// </summary>
    public void LoadStancePages()
    {
        InitalizeGemList(30);

        if (!File.Exists(Application.dataPath + "/StreamingAssets/StanceGemPageData.Json")) return;

        StanceGemList loadedStancePageData = new();

        string saveString = File.ReadAllText(Application.dataPath + "/StreamingAssets/StanceGemPageData.Json");

        loadedStancePageData = JsonUtility.FromJson<StanceGemList>(saveString);

        for (int i = 0; i < loadedStancePageData.stanceGem.Count; i++)
        {
            int pageIndex = loadedStancePageData.stanceGem[i].pageIndex;
            int slotIndex = loadedStancePageData.stanceGem[i].slotIndex;
            int gemIndex = loadedStancePageData.stanceGem[i].gemIndex;
            int gemRarity = loadedStancePageData.stanceGem[i].gemRarity;

            SetStanceGemList(pageIndex, slotIndex, gemIndex, gemRarity);
        }

        Debug.Log("Stance Pages loaded");
    }

    /// <summary>
    /// Loads data from custom path
    /// </summary>
    private void LoadData(string _path, string _loaded)
    {
        if (!File.Exists(Application.dataPath + _path)) return;

        string saveString = File.ReadAllText(Application.dataPath + _path);

        ItemDataBase _data = JsonUtility.FromJson<ItemDataBase>(saveString); ;

        for (int i = 0; i < _data.itemDatabase.Count; i++)
        {
            itemDatabase.Add(_data.itemDatabase[i].index, _data.itemDatabase[i]);
        }

        Debug.Log(_loaded + " loaded");
    }

    /// <summary>
    /// Loads the inventory elements
    /// </summary>
    private void LoadInventory()
    {
        if (!File.Exists(Application.dataPath + "/StreamingAssets/InventoryData.Json")) return;

        string saveString = File.ReadAllText(Application.dataPath + "/StreamingAssets/InventoryData.Json");

        InventoryList inventoryData = JsonUtility.FromJson<InventoryList>(saveString);

        Debug.Log("Inventory Data loaded");

        InstantiateGemInventory(inventoryData);
    }

    /// <summary>
    /// Instantiates the Gem elements from the loaded gem list
    /// </summary>
    private void InstantiateGemInventory(InventoryList _inventoryData)
    {
        for (int i = 0; i < _inventoryData.inventory.Count; i++)
        {
            Inventory gemInv = _inventoryData.inventory[i];

            GameManager.instance.HUDManager.InventoryItems.AddGem(gemInv);
        }
        Debug.Log("Gems are Instantiated (Inventory)");

    }

    /// <summary>
    /// Get the item damage value based on the index
    /// </summary>
    public int GetDamage(int _itemIndex, int _valueIndex)
    {
        if (_valueIndex >= itemDatabase[_itemIndex].attributes.Count)
        {
            Debug.Log("Value Index doesn't exits");
            return 0;
        }

        int value = (int)itemDatabase[_itemIndex].attributes[_valueIndex].mainAttributeValue;

        return value;
    }

    /// <summary>
    /// Gets the icon name from the asset
    /// </summary>
    public string GetIconName(int _itemIndex)
    {
        if (!itemDatabase.ContainsKey(_itemIndex)) return "";

        return itemDatabase[_itemIndex].name;
    }

    /// <summary>
    /// Gets the icon path from the asset
    /// </summary>
    public string GetIconPath(int _itemIndex)
    {
        if (!itemDatabase.ContainsKey(_itemIndex)) return "";

        return itemDatabase[_itemIndex].imagePath;
    }

    /// <summary>
    /// Gets the rarity
    /// </summary>
    public int GetRarity(int _itemIndex)
    {
        if (!itemDatabase.ContainsKey(_itemIndex)) return 0;

        return itemDatabase[_itemIndex].rarity;
    }

    /// <summary>
    /// Gets the buff duration time (different from the cooldown)
    /// </summary>
    public float GetDuration(int _itemIndex)
    {
        if (!itemDatabase.ContainsKey(_itemIndex)) return 0;

        return itemDatabase[_itemIndex].duration;
    }

    /// <summary>
    /// Gets the cooldown duration
    /// </summary>
    public float GetCooldown(int _itemIndex)
    {
        if (!itemDatabase.ContainsKey(_itemIndex)) return 0;

        return itemDatabase[_itemIndex].cooldown;
    }

    /// <summary>
    /// Gets the cost of the ability (stamina or int)
    /// </summary>
    public int GetCost(int _itemIndex)
    {
        if (!itemDatabase.ContainsKey(_itemIndex)) return 0;

        return itemDatabase[_itemIndex].skillCost;
    }

    /// <summary>
    /// Returns the gem descrition texts 
    /// (_textindex:(0) Base descrition (1) first set bonus descrition (2) second set bonus descrition)
    /// </summary>
    public string GetDescription(int _index, int _rarity, int _textIndex, int _duplicateNumber)
    {
        if (_textIndex < 0 && _textIndex > 2) Debug.Log("Gem description out of range");

        string s = "";
        ItemData replaceableString = itemDatabase[_index];

        string newString = replaceableString.description[_textIndex];

        newString = newString.Replace("{duration}", replaceableString.duration.ToString()); // swap certain elements from description text to number
        newString = newString.Replace("{cooldown}", replaceableString.cooldown.ToString());
        newString = newString.Replace("{skillCost}", replaceableString.skillCost.ToString());

        string[] splitString = newString.Split('{', '}');
        List<int> replaceIndex = new();
        int counter = 0;
        for (int i = 1; i < splitString.Length; i += 2)
        {
            replaceIndex.Add(int.Parse(splitString[i]));
            splitString[i] = CalculateValue((int)replaceableString.attributes[replaceIndex[counter]].mainAttributeValue,
                (int)replaceableString.attributes[replaceIndex[counter]].secondaryAttributeValue, _rarity, _duplicateNumber).ToString(); // calculuate gem Value
            counter++;
        }
        for (int i = 0; i < splitString.Length; i++) s += splitString[i];

        return s;
    }


    /// <summary>
    /// Calculates the gem attributes based on the rarity level.
    /// </summary>
    public int CalculateValue(int _mainValue, int _secondaryValue, int _rarity, int _duplicateNumber)
    {
        int duplicateNumber = _duplicateNumber;
        if (_duplicateNumber == 0) duplicateNumber = 1;
        return (_mainValue + (_secondaryValue * _rarity)) * duplicateNumber;
    }

    /// <summary>
    /// Instantiates the gem List
    /// </summary>
    private void InitalizeGemList(int _gemSlotNumber)
    {
        for (int i = 0; i < 4; i++)
        {
            stanceGemList.Add(new());
            for (int j = 0; j < _gemSlotNumber; j++)
            {
                stanceGemList[i].Add(new Inventory(0, 0, 0));
            }
        }
    }

    public void SetStanceGemList(int _pageIndex, int _slotIndex, int _gemIndex, int _gemRarity)
    {
        GetStanceGemList()[_pageIndex][_slotIndex].index = _gemIndex;
        GetStanceGemList()[_pageIndex][_slotIndex].rarity = _gemRarity;
    }

    public void SetSkillTreeList(int _pageIndex, int _skillIndex, bool _isActivated)
    {
        GetSkillTreeList().Add(new EquippeSkills(_pageIndex, _skillIndex, _isActivated));
    }

    /// <summary>
    /// Get the item list from certain type of item
    /// </summary>
    public List<int> GetItemList(ItemType _type)
    {
        List<int> indexList = new();

        foreach (var item in itemDatabase.Keys)
        {
            int firstDigit = (int)(item / Math.Pow(10, (int)Math.Log10(item)));

            switch (_type)
            {
                case ItemType.Gem:

                    if (firstDigit == 2) indexList.Add(item);
                    break;
            }
        }
        if (indexList.Count > 0) return indexList;
        return null;
    }

    /// <summary>
    /// Get random item from that type of list (Mainly loot from chest)
    /// </summary>
    public int GetRandomItem(ItemType _type, out int _rarity)
    {
        int rarity = 0;

        int percent = UnityEngine.Random.Range(1, 101);

        if (percent < 5) rarity = 4;
        else if (percent < 15) rarity = 3;
        else if (percent < 30) rarity = 2;
        else if (percent < 50) rarity = 1;
        else if (percent < 100) rarity = 0;

        int randomItem = UnityEngine.Random.Range(0, GetItemList(_type).Count);

        int randomItemIndex = GetItemList(_type)[randomItem];

        _rarity = rarity;
        return randomItemIndex;
    }
}

[System.Serializable]
public class ItemData
{
    [Serializable]
    public class AttributeDetails
    {
        public int attributeIndex;
        public float mainAttributeValue;
        public float secondaryAttributeValue;
    }

    public int index;
    public int rarity;
    public int skillTier;
    public int linkToSkillIndex;
    public int skillCost;
    public int buffTriggerIndex;
    public float cooldown;
    public float duration;
    public string name;
    public string imagePath;
    public List<AttributeDetails> attributes;
    public List<string> description;
}

[System.Serializable]
public class ItemDataBase { public List<ItemData> itemDatabase = new(); }

// Class for Inventory Data
[System.Serializable]
public class Inventory
{
    public int index;
    public int quantity;
    public int rarity;

    public override string ToString() => $"{index} {rarity}";

    public Inventory(int _gemIndex, int _duplicateNumber, int _gemRarity)
    {
        index = _gemIndex;
        quantity = _duplicateNumber;
        rarity = _gemRarity;
    }
}

[System.Serializable]
public class InventoryList { public List<Inventory> inventory = new(); }

//Class for the Stance Pages
[System.Serializable]
public class StanceGem
{
    public int pageIndex;
    public int slotIndex;
    public int gemIndex;
    public int gemRarity;
}
public class StanceGemList { public List<StanceGem> stanceGem = new(); }

[System.Serializable]
public class SkillTreeData
{
    public int skillIndex;
    public int pageIndex;
    public bool skillIsActivated;
}

[Serializable]
public class SkillTreeList { public List<SkillTreeData> skillTree = new(); }
