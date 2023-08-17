using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDialogue : MonoBehaviour
{
    [SerializeField] private float dialogueTime;

    [TextArea(5, 15)]
    [SerializeField] private string dialogueText;

    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = GameManager.instance.HUDManager.DialogueManager;
    }

    /// <summary>
    /// Starts the Dialogue using dialogueManager
    /// </summary>
    public void StartDialogue()
    {
        dialogueManager.TriggerStartDialogueAnimation();

        dialogueManager.AddCutSceneDialogue(dialogueText);
        dialogueManager.TriggerCutSceneDialogue(dialogueTime);
    }


    /// <summary>
    /// stops the Dialogue using dialogueManager
    /// </summary>
    public void EndDialogue()
    {
        dialogueManager.TriggerEndDialogueAnimation();
    }
}
