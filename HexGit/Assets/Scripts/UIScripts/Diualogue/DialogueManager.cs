using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] List<Color> colorList;
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private GameObject continueDialogueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject background;
    [SerializeField] private float dialogeTextProgress;

    private List<string> dialogueList = new List<string>();
    private Animator anim;
    private int currentIndex;

    private bool isDialogueFinished = false;
    private bool isCutSceneDialogue = false;

    private string fullCutSceneDialogue;

    private IEnumerator delayIE;

    public void SetIsCutSceneDialogue(bool _bool) { isCutSceneDialogue = _bool; }
    public void SetDialogeTextProgress(float _time) { dialogeTextProgress = _time; }

    void Start()
    {
        anim = GetComponent<Animator>();

         GameManager.instance.InputManager.GetInputActions().UI.Interact.performed += TriggerDialogue;
    }

    private void Update()
    {
        UpdateCutSceneDialogue();
    } 

    /// <summary>
    /// Starts Cutscene Dialogue
    /// </summary>
    public void AddCutSceneDialogue(string _dialogue)
    {
        SetIsCutSceneDialogue(true);

        TriggerStartDialogueAnimation();

        fullCutSceneDialogue = _dialogue;
    }

    /// <summary>
    /// Updates the cutscene dialoge based on the dialogue time (between 1 and 0)
    /// </summary>
    private void UpdateCutSceneDialogue()
    {
        if (!isCutSceneDialogue) return;

        dialogeTextProgress = Mathf.Clamp01(dialogeTextProgress);

        float dialogueProgress = fullCutSceneDialogue.Length * dialogeTextProgress;

        dialogueText.text = "";

        // Because the text is middle aligned it's need a placeholder invisible text and change the inivisible letters to the visible ones
        string currentDialogue = "";
        string invisibleCode = "<color=#FFFFFF00>";
        string closeColorCode = "</color>";
        string invisibleDialogue = fullCutSceneDialogue;

        for (int i = 0; i < (int)dialogueProgress; i++)
        {
            currentDialogue += fullCutSceneDialogue[i];
            invisibleDialogue = invisibleDialogue.Substring(1);
        }

        string tempDialogue = invisibleCode + invisibleDialogue + closeColorCode;

        string finalDialogue = currentDialogue + tempDialogue;

        dialogueText.text = finalDialogue;
    }

    /// <summary>
    /// Triggers the Coroutine to start the dialogue typewriter effect
    /// </summary>
    public void TriggerCutSceneDialogue(float _time)
    {
        StartCoroutine(CutSceneDialogueUpdate(_time));
    }

    /// <summary>
    /// Type out the text in N time
    /// </summary>
    IEnumerator CutSceneDialogueUpdate(float _time)
    {
        dialogeTextProgress = 0;

        while (dialogeTextProgress < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime / _time);
            dialogeTextProgress += Time.deltaTime / _time;
            UpdateCutSceneDialogue();
        }

        UpdateCutSceneDialogue();
    }

    /// <summary>
    /// Triggers the NPC dialogue
    /// </summary>
    private void TriggerDialogue(InputAction.CallbackContext context)
    {
        if (dialogueList.Count <= 0) return;

        if (delayIE != null) return;

        if (!isDialogueFinished)
        {
            InterruptTyping();
        }
        else
        {
            if (currentIndex < dialogueList.Count)
            {
                delayIE = DelayInterrupt();
                StartCoroutine(delayIE);
                StartCoroutine(TypeDialogue(currentIndex));
            }
            else EndDialogue();
        }
    }

    /// <summary>
    /// Interrupts the typing (instantly finishes the dialogue)
    /// </summary>
    private void InterruptTyping()
    {
        isDialogueFinished = true;
    }

    IEnumerator DelayInterrupt()
    {
        yield return new WaitForSeconds(0.5f);
        delayIE = null;
    }

    // Types out the NPC dialogue
    public IEnumerator TypeDialogue(int index)
    {
        currentIndex = index;
        string currentDialogue = "";

        string invisibleCode = "<color=#FFFFFF00>";
        string closeColorCode = "</color>";
        string invisibleDialogue = ClearDialogueText(dialogueList[index]); // Clears out the dialoge from the text colour and other addition

        isDialogueFinished = false;
        int characterNumber = invisibleDialogue.Length;
        int counter = 0;

        while (!isDialogueFinished)
        {
            int colorIndex;

            if (int.TryParse(dialogueList[index][counter].ToString(), out colorIndex)) // Grab the color index from the text (numbers 0 to 8)
            {
                if (colorIndex == 9) currentDialogue += closeColorCode;
                else currentDialogue += CreateColorCode(colorList[colorIndex]);

            }
            else
            {
                invisibleDialogue = invisibleDialogue.Substring(1);
                currentDialogue += dialogueList[index][counter];
            }
            // Because the text is middle aligned it's need a placeholder invisible text and change the inivisible letters to the visible ones
            string tempDialogue = invisibleCode + invisibleDialogue + closeColorCode;

            string finalDialogue = currentDialogue + tempDialogue;

            yield return new WaitForSecondsRealtime(typeSpeed);

            dialogueText.text = finalDialogue;
            counter++;
            if (counter >= characterNumber) isDialogueFinished = true;
        }
        dialogueText.text = FinishDialogue(dialogueList[index]);
        currentIndex++;

        if (currentIndex == dialogueList.Count) continueDialogueIcon.SetActive(false);
    }

    /// <summary>
    /// Sets up the dialoge manager to the new dialogue
    /// </summary>
    public void SetDialogueList(ref List<string> _list)
    {
        delayIE = DelayInterrupt();
        StartCoroutine(delayIE);

        TriggerStartDialogueAnimation();                                    // Animation to show the Dialogue UI
        background.SetActive(true);                                         // Activates the UI background
        dialogueList.Clear();                                               // Clear the dialogueList in case something is still there
        dialogueList = new List<string>(_list);                             // set the new dialogue list
        if (dialogueList.Count > 1) continueDialogueIcon.SetActive(true);   // set the dialogueIcon true if the list has more than one dialogue
         GameManager.instance.InputManager.DisablePlayerInputActions();                  // disables the player inputs while the dialoge is on
        StartCoroutine(TypeDialogue(0));                                    // starts the first dialogue
        dialogueText.text = "";                                             // Cleans the UI in case text is remains there
    }

    /// <summary>
    /// Ends the dialogue
    /// </summary>
    private void EndDialogue()
    {
        TriggerEndDialogueAnimation();
        dialogueList.Clear();
         //GameManager.instance.ModularController.GetComponent<ModularController>().SetCharacterStatus(playerStatus.GROUND);
         GameManager.instance.InputManager.EnablePlayerInputActions();
    }

    /// <summary>
    /// Triggers the swith on dialogue UI
    /// </summary>
    public void TriggerStartDialogueAnimation() 
    {
        dialogueText.text = "";
        background.SetActive(true);
        background.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        anim.SetTrigger("StartDialogue");
    }


    /// <summary>
    /// Triggers the swith off dialogue UI
    /// </summary>
    public void TriggerEndDialogueAnimation() 
    { 
        anim.SetTrigger("EndDialogue");
        background.SetActive(false);
        SetIsCutSceneDialogue(false);
    }

    /// <summary>
    /// Creates the color code for the colored part of the dialogue text
    /// </summary>
    private string CreateColorCode(Color _color)
    {
        string startPart = "<color=#";
        string colorToHexadec = ColorUtility.ToHtmlStringRGBA(_color);
        string endPart = ">";

        string colorCode = startPart + colorToHexadec + endPart;

        return colorCode;
    }

    /// <summary>
    /// Clears the dialogue text from the additional indexes (for color codes)
    /// </summary>
    private string ClearDialogueText(string _dialogue)
    {
        string clearedDialogue = "";
        for (int i = 0; i < _dialogue.Length; i++)
        {
            if (!int.TryParse(_dialogue[i].ToString(), out int colorIndex)) clearedDialogue += _dialogue[i];
        }

        return clearedDialogue;
    }

    /// <summary>
    /// Finishes the dialogue if the player interrupts the typewriter
    /// </summary>
    private string FinishDialogue(string _dialogue)
    {
        string finishedDialogue = "";
        string closeColorCode = "</color>";

        for (int i = 0; i < _dialogue.Length; i++)
        {
            int colorIndex;

            if (int.TryParse(_dialogue[i].ToString(), out colorIndex))
            {
                if (colorIndex == 9) finishedDialogue += closeColorCode;
                else finishedDialogue += CreateColorCode(colorList[colorIndex]);

            }
            else
            {
                finishedDialogue += _dialogue[i];
            }
        }

        return finishedDialogue;
    }
}
