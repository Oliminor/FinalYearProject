using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPuzzle : MonoBehaviour
{
    [SerializeField] Chest chest;
    bool isFinished = false;

    // When the player enters to the puzzle object and has the Collectable puzzle piece, it will complete this part of the puzzle
    private void OnTriggerEnter(Collider other)
    {
        if (isFinished) return;
        if (other.tag != "Player") return;
        Transform slotT = GameManager.playerStash.CheckSlots("CollectPuzzle");
        if (slotT == GameManager.playerStash.transform) return;
        slotT.gameObject.GetComponent<Collectable>().FinishPuzzle(transform);
        isFinished = true;
        chest.PuzzleSolved();
    }
}
