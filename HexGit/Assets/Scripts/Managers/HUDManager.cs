using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private DisplayItemDetails displayItemDetails;
    [SerializeField] private PopUpTextManager popUpTextManager;
    [SerializeField] private AreaTransitionText areaTransitionText;
    [SerializeField] private Transition transition;
    [SerializeField] private StanceAcquiredPopUp stanceAcquiredPopUp;
    [SerializeField] private SideIcon sideIcon;
    [SerializeField] private ItemAquiredManager itemAquiredManager;
    [SerializeField] private StanceManagerUI stanceManagerUI;
    [SerializeField] private InventoryItems inventoryItems;
    [SerializeField] private SkillManagerUI skillManagerUI;
    [SerializeField] private DialogueManager dialogueManager;

    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform manaBar;
    [SerializeField] private RectTransform staminaBar;

    [SerializeField] private RectTransform buffIconHolder;

    [SerializeField] private RectTransform crossHair;

    [SerializeField] private RectTransform pressToInteract;
    [SerializeField] private RectTransform messageBoard;

    [SerializeField] private RectTransform gameOverScreen;

    [SerializeField] private RectTransform bossHealthBar;

    [SerializeField] private RectTransform aquiredItemPrefab;
    [SerializeField] private RectTransform aquiredItemParent;

    [SerializeField] private RectTransform targetEnemyIndicator;

    MainCharacterStats mainCharacterStats;

    public RectTransform GetBuffIconHolder() { return buffIconHolder; }
    public bool GetIsPressToInteractActive() { return pressToInteract.gameObject.activeInHierarchy; }
    public bool GetIsMessageBoardActive() { return messageBoard.gameObject.activeInHierarchy; }

    public DisplayItemDetails DisplayItemDetails { get { return displayItemDetails; } }
    public PopUpTextManager PopUpTextManager { get { return popUpTextManager; } }
    public AreaTransitionText AreaTransitionText { get { return areaTransitionText; } }
    public Transition Transition { get { return transition; } }
    public StanceAcquiredPopUp StanceAcquiredPopUp { get { return stanceAcquiredPopUp; } }
    public SideIcon SideIcon { get { return sideIcon; } }
    public ItemAquiredManager ItemAquiredManager { get { return itemAquiredManager; } }
    public StanceManagerUI StanceManagerUI { get { return stanceManagerUI; } }
    public InventoryItems InventoryItems { get { return inventoryItems; } }
    public SkillManagerUI SkillManagerUI { get { return skillManagerUI; } }
    public DialogueManager DialogueManager { get { return dialogueManager; } }

    void Start()
    {
        mainCharacterStats = MainCharacterStats.instance;
        Initialize();
    }

    private void Initialize()
    {
        if (crossHair) crossHair.gameObject.SetActive(false);
        if (pressToInteract) pressToInteract.gameObject.SetActive(false);
        if (messageBoard) messageBoard.gameObject.SetActive(false);
        if (areaTransitionText) areaTransitionText.gameObject.SetActive(true);
        if (gameOverScreen) gameOverScreen.gameObject.SetActive(false);
        if (stanceAcquiredPopUp) stanceAcquiredPopUp.gameObject.SetActive(false);
        if (bossHealthBar) bossHealthBar.gameObject.SetActive(false);
        if (targetEnemyIndicator) targetEnemyIndicator.gameObject.SetActive(false);

        areaTransitionText.TriggerAreaTextTransition();

        MainCharacterStats.onGameOver += GameOverScreen;
    }
    
    /// <summary>
    /// Acticates the game over Screen
    /// </summary>
    private void GameOverScreen()
    {
        if (gameOverScreen)
        {
            gameOverScreen.gameObject.SetActive(true);
            StartCoroutine(DisableGameOver());
        }
        areaTransitionText.TriggerAreaTextTransition();
        StartCoroutine(TransitionDelay());
    }

    // Disables the game over screen after N seconds
    private IEnumerator DisableGameOver()
    {
        yield return new WaitForSeconds(4.0f);
        gameOverScreen.gameObject.SetActive(false);
    }

    // Activates the transition after N seconds
    private IEnumerator TransitionDelay()
    {
        yield return new WaitForSeconds(1.0f);
        transition.TriggerTransitionOn();
    }


    public void UpdateManaBar()
    {
        UpdateStatusBar(manaBar, mainCharacterStats.GetMaxMana(), mainCharacterStats.GetCurrentMana());
    }

    public void UpdateStaminaBar()
    {
        UpdateStatusBar(staminaBar, mainCharacterStats.GetMaxStamina(), mainCharacterStats.GetCurrentStamina());
    }

    public void UpdateHealthbar()
    {
        UpdateStatusBar(healthBar, mainCharacterStats.GetMaxHealth(), mainCharacterStats.GetCurrentHealth());
    }

    /// <summary>
    /// Update health, stamina and mana bar UI element
    /// </summary>
    public void UpdateAllBar()
    {
        UpdateHealthbar();
        UpdateStaminaBar();
        UpdateManaBar();
    }

    /// <summary>
    /// Updates boss health UI element
    /// </summary>
    public void UpdateBossHealth(string _bossName, float _maxValue, float _currentValue)
    {
        RectTransform _statusBar = bossHealthBar.GetChild(0).GetComponent<RectTransform>();

        bossHealthBar.GetChild(1).GetComponent<TextMeshProUGUI>().text = _bossName;

        float percent = (float)_currentValue / _maxValue;

        _statusBar.GetChild(0).GetChild(1).localScale = new Vector2(percent, 1);

        if (_statusBar) StartCoroutine(SetBar(_statusBar));
    }

    /// <summary>
    /// Toggles boss health bar on or off
    /// </summary>
    public void ToggleBossHealthbar(bool _bool)
    {
        bossHealthBar.gameObject.SetActive(_bool);
    }

    /// <summary>
    /// if Enemy close, toggles on the target indicator (melee stance UI element)
    /// </summary>
    public void ToggleTargetEnemyIndicator(bool _bool, Vector3 _position)
    {
        if (!targetEnemyIndicator) return;
        targetEnemyIndicator.gameObject.SetActive(_bool);
        targetEnemyIndicator.transform.position = _position;
    }

    /// <summary>
    ///  Update the bars size based on the health, stamina or mana
    /// </summary>
    public void UpdateStatusBar(RectTransform _statusBar, float _maxValue, float _currentValue)
    {
        _statusBar.sizeDelta = new Vector2(_maxValue, _statusBar.sizeDelta.y);

        float percent = (float)_currentValue / _maxValue;

        _statusBar.GetChild(0).GetChild(1).localScale = new Vector2(percent, 1);

        if (_statusBar)StartCoroutine(SetBar(_statusBar));
    }

    // Same but lerp it slowly the secondary bar behind the main
    private IEnumerator SetBar(RectTransform _statusBar)
    {
        float time = 0;

        float mainBarScale = _statusBar.GetChild(0).GetChild(1).localScale.x;
        float secondaryBarScale = _statusBar.GetChild(0).GetChild(0).localScale.x;

        if (mainBarScale > secondaryBarScale)
        {
            _statusBar.GetChild(0).GetChild(0).localScale = new Vector2(mainBarScale, 1);
            yield break;
        }

        while (time < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            time += Time.deltaTime * 2;

            _statusBar.GetChild(0).GetChild(0).localScale = new Vector2(Mathf.Lerp(secondaryBarScale, mainBarScale, time), 1);
        }
    }

    /// <summary>
    /// Activates and deactivates crosshair
    /// </summary>
    public void SetCrossHair(bool _isOn)
    {
        crossHair.gameObject.SetActive(_isOn);
    }

    /// <summary>
    /// Shows the interact text and change it to custom text
    /// </summary>
    public void ShowPressToInteract(string _text)
    {
        pressToInteract.gameObject.SetActive(true);
        pressToInteract.GetChild(0).GetComponent<TextMeshProUGUI>().text = _text;
    }

    /// <summary>
    /// Disables the interact UI element
    /// </summary>
    public void DisablePressToInteract()
    {
        pressToInteract.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the message board with custom text
    /// </summary>
    public void ShowMessageBoard(string _text)
    {
        messageBoard.gameObject.SetActive(true);
        messageBoard.GetChild(0).GetComponent<TextMeshProUGUI>().text = _text;
    }

    /// <summary>
    /// Disables message board
    /// </summary>
    public void DisableMessageBoard()
    {
        StartCoroutine(DisableMessageBoardIE());
    }

    // by lerping it
    IEnumerator DisableMessageBoardIE()
    {
        messageBoard.GetComponent<Animator>().Play("MessageBoardAnimOff");
        yield return new WaitForSeconds(0.3f);
        messageBoard.gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts Stance acquired animation
    /// </summary>
    public void ShowStanceAquiredPopUp()
    {
        stanceAcquiredPopUp.gameObject.SetActive(true);
        StartCoroutine(DisableShowStanceAquiredPopUp());
    }

    // Disables it
    IEnumerator DisableShowStanceAquiredPopUp()
    {
        yield return new WaitForSeconds(10.0f);
        stanceAcquiredPopUp.gameObject.SetActive(false);
    }
}
