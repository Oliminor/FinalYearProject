using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActivateCutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector cutscene;
    [SerializeField] private Transform lookAtPosition;

    bool isActivated = false;

    public void SetIsActivated(bool _bool) { isActivated = _bool; }

    // Activates the cutscene when the player step on the collider
    private void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;

        if (other.transform ==  GameManager.instance.ModularController.transform)
        {
            if (lookAtPosition) GameManager.instance.CameraManager.LockPlayerCameraTo(lookAtPosition);
            cutscene.Play();

            isActivated = true;
        }
    }
}
