using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterStats : MonoBehaviour
{
    public static MainCharacterStats instance;

    public delegate void OnGameOver();
    public static OnGameOver onGameOver;

    private int level = 4;

    // Character base stats
    private Attributes baseAttributes = new Attributes(550, 550, 175, 175, 10, 1.0f, 1.5f, 5.0f, 0.0f, 1.0f);

    private Attributes additionalAttributes = new();

    private Attributes tempAttributes = new();

    private ModularController player;

    private int currentHealth;
    private float currentStamina;
    private int currentMana;

    bool isCritical = false;

    private IEnumerator staminaCoroutine;

    public int GetLevel() { return level; }
    public void SetCritical(bool _bool) { isCritical = _bool; }
    public bool GetCritical() { return isCritical; }
    public int GetMaxHealth() { return baseAttributes.flatHealth + additionalAttributes.flatHealth; }
    public int GetMaxStamina() { return baseAttributes.flatStamina + additionalAttributes.flatStamina; }
    public int GetMaxMana() { return baseAttributes.flatMana + additionalAttributes.flatMana; }
    public int GetCurrentHealth() { return currentHealth; }
    public float GetCurrentStamina() { return currentStamina; }
    public int GetCurrentMana() { return currentMana; }
    public float GetLifeSteal() { return (baseAttributes.lifestealPercent + additionalAttributes.lifestealPercent + tempAttributes.lifestealPercent) / 100.0f; }
    public int GetFlatDefense() { return baseAttributes.flatDef + additionalAttributes.flatDef + tempAttributes.flatDef; }

    void Awake()
    {
        if (instance != null) Debug.LogError("We have a problem chief - singleton instace is already in use (MainCharacterStats)");
        instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        player =  GameManager.instance.ModularController;
        currentHealth = baseAttributes.flatHealth + additionalAttributes.flatHealth;
        currentMana = baseAttributes.flatMana + additionalAttributes.flatMana;
        currentStamina = baseAttributes.flatStamina + additionalAttributes.flatStamina;

        onGameOver += GameOver;

        GameManager.instance.HUDManager.UpdateAllBar();
    }

    /// <summary>
    /// Player takes damage (calculating based on the player armor)
    /// </summary>
    public void TakeDamage(int _damage)
    {
        float defense = GetDefensePercent(100, 0.003f, 100, 0.002f, 1000, 0.001f);
        float damage = _damage * defense;

        GameManager.instance.HUDManager.PopUpTextManager.DamageTextInstiate((int)damage,  GameManager.instance.ModularController.GetPlayerDamagePoint(), false, false);
        currentHealth -= (int)damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, GetMaxHealth());
        if (currentHealth <= 0) onGameOver();

        GameManager.instance.HUDManager.UpdateHealthbar();
    }

    /// <summary>
    /// Set the player health and updates the health bar UI
    /// </summary>
    public void SetHealth(int _health) 
    {
        currentHealth -= _health;
        currentHealth = Mathf.Clamp(currentHealth, 0, GetMaxHealth());
        if (currentHealth <= 0) onGameOver();

        GameManager.instance.HUDManager.UpdateHealthbar();
    }

    /// <summary>
    /// Set the player stamina and updates the health bar UI
    /// </summary>
    public void SetStamina(float _stamina)
    {
        currentStamina -= _stamina;
        currentStamina = Mathf.Clamp(currentStamina, 0, GetMaxStamina());

        GameManager.instance.HUDManager.UpdateStaminaBar();

        float delay = 1.5f;
        if (currentStamina == 0) delay = 3.0f;

        if (staminaCoroutine != null) StopCoroutine(staminaCoroutine);
        staminaCoroutine = StaminaCharging(delay);
        StartCoroutine(staminaCoroutine);
    }

    /// <summary>
    /// Set the player health and updates the health bar UI
    /// </summary>
    public void SetMana(int _mana)
    {
        currentMana -= _mana;
        currentMana = Mathf.Clamp(currentMana, 0, GetMaxMana());

        GameManager.instance.HUDManager.UpdateManaBar();
    }

    /// <summary>
    /// Resets the health, mana, stamina when Game Over
    /// </summary>
    private void GameOver()
    {
        player.transform.position = GameManager.instance.CheckpointManager.GetCurrentRespawnPoint();

        ResetMainStats();
    }
    
    /// <summary>
    /// Resets the health, mana, stamina
    /// </summary>
    public void ResetMainStats()
    {
        currentHealth = GetMaxHealth();
        currentMana = GetMaxMana();
        currentStamina = GetMaxStamina();

        GameManager.instance.HUDManager.UpdateAllBar();
    }

    /// <summary>
    /// Charge the stamina if needed
    /// </summary>
    IEnumerator StaminaCharging(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        while (currentStamina < GetMaxStamina())
        {
            yield return new WaitForSeconds(Time.deltaTime);
            currentStamina += 6.0f;
            GameManager.instance.HUDManager.UpdateStaminaBar();
        }
    }

    /// <summary>
    /// Calculating the defense value percent (the more defense the player has, the less value overtime)
    /// </summary>
    private float GetDefensePercent(int limit01, float limitValue01, int limit02, float limitValue02, int limit03, float limitValue03)
    {
        int defense = GetFlatDefense();
        float defensePercent = 1;
        defensePercent -= Mathf.Min(Mathf.Max(0, defense), limit01) * limitValue01; // No cap
        defense -= limit01;
        defensePercent -= Mathf.Min(Mathf.Max(0, defense), limit02) * limitValue02; // Soft cap
        defense -= limit02;
        defensePercent -= Mathf.Min(Mathf.Max(0, defense), limit03) * limitValue03; // Hard cap

        return Mathf.Max(defensePercent, 0.1f);
    }

    /// <summary>
    /// Calculates the lifesteal value if the player has any lifesteal and restore the health
    /// </summary>
    public void LifeStealCalculation(float _damage)
    {
        float lifeSteal = GetLifeSteal();

        int healAmount = (int)(_damage * lifeSteal);

        SetHealth(-healAmount);

        if (GetCurrentHealth() >= GetMaxHealth()) return;

        GameManager.instance.HUDManager.PopUpTextManager.DamageTextInstiate(healAmount,  GameManager.instance.ModularController.GetPlayerDamagePoint(), false, true);
    }

    /// <summary>
    /// Calculates the player damage
    /// </summary>
    public float DamageCalculation ()
    {
        AdditionalStatUpdate();

        Attributes calculation = baseAttributes + additionalAttributes + tempAttributes;

        SetCritical(IsCritical(calculation.criticalStrikeRate));

        float damage = Random.Range(baseAttributes.flatAttack - 10, baseAttributes.flatAttack + 10);

        damage += calculation.flatAttack;

        if (isCritical) damage *= calculation.criticalDamageRate;

        damage *= calculation.addDamageBonusPercent;

        return damage;
    }

    /// <summary>
    ///  return true if the critical strike is success
    /// </summary>
    private bool IsCritical(float _criticalRate)
    {
        bool _isCritical = false;

        if ((_criticalRate) >= Random.Range(0.0f, 100.0f)) _isCritical = true;

        return _isCritical;
    }

    /// <summary>
    /// Updates the additional attributes 
    /// </summary>
    private void AdditionalStatUpdate()
    {

        additionalAttributes.ResetAttributes();

        int activeStanceIndex = GameManager.instance.StanceManager.GetActiveStanceIndex();

        List<Inventory> _equippedGem = new(GameManager.instance.DataManager.GetStanceGemList()[activeStanceIndex]);

        for (int i = 0; i < _equippedGem.Count; i++)
        {
            if (_equippedGem[i].index == 0) continue;

            int _gemIndex = _equippedGem[i].index;
            int _gemRarity = _equippedGem[i].rarity;

            SetGemAdditionalBaseAttributes(_gemIndex, _gemRarity, 0);
        }

        List<Inventory> duplicateGemList = new(GameManager.instance.HUDManager.StanceManagerUI.GetGemDuplicationList(activeStanceIndex, false));

        for (int i = 0; i < duplicateGemList.Count; i++)
        {
            int _gemIndex = duplicateGemList[i].index;
            int _gemRarity = duplicateGemList[i].rarity;

            if (duplicateGemList[i].quantity >= 2) SetGemAdditionalBaseAttributes(_gemIndex, _gemRarity, 1);
        }
        
    }

    /// <summary>
    /// Adds the attributes from the currently active buffs
    /// </summary>>
    public void SetBuffAdditionalAtrributes(int _index, bool isRemove, bool isGembuff)
    {
        int startCount = 0;
        if (isGembuff) startCount = 2;

        for (int i = startCount; i < GameManager.instance.DataManager.GetItemDatabase()[_index].attributes.Count; i++)
        {
            float attributeValue = GameManager.instance.DataManager.GetItemDatabase()[_index].attributes[i].mainAttributeValue;
            int attributeIndex = GameManager.instance.DataManager.GetItemDatabase()[_index].attributes[i].attributeIndex;

            if (isRemove) tempAttributes.AddAttributes(attributeIndex, -attributeValue);
            else tempAttributes.AddAttributes(attributeIndex, attributeValue);

        }
    }

    /// <summary>
    /// Adds the attributes from the gems 
    /// </summary>
    private void SetGemAdditionalBaseAttributes(int _index, int _rarity, int _attributeIndex)
    {
        float attributeValue = GameManager.instance.DataManager.GetItemDatabase()[_index].attributes[_attributeIndex].mainAttributeValue;

        float mainValue = GameManager.instance.DataManager.GetItemDatabase()[_index].attributes[_attributeIndex].mainAttributeValue;
        float secondaryValue = (int)GameManager.instance.DataManager.GetItemDatabase()[_index].attributes[_attributeIndex].secondaryAttributeValue;

        if (_rarity > 0) attributeValue = GameManager.instance.DataManager.CalculateValue((int)mainValue, (int)secondaryValue, _rarity, 0);

        int attributeIndex = GameManager.instance.DataManager.GetItemDatabase()[_index].attributes[_attributeIndex].attributeIndex;

        additionalAttributes.AddAttributes(attributeIndex, attributeValue);
    }
}