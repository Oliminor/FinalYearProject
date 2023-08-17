using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckPointObj : MonoBehaviour
{
    [SerializeField] private CheckPoint cp;
    [SerializeField] private Transform respawnPos;

    [SerializeField] private Transform treeCrown;
    [SerializeField] private Transform treeTrunk;

    private bool isCharacterOnTheCollider;

    private void Start()
    {
        cp.position = respawnPos.position;

        GameManager.instance.CheckpointManager.AddCheckPoint(cp);

        GameManager.instance.InputManager.GetInputActions().UI.Interact.performed += ActiveChecpoint;

        SetMaterialRadius(5);

        if (!cp.isDiscovered) return;

        SetMaterialRadius(40);
    }

    /// <summary>
    /// Activates the checkpoint if the player close enough
    /// </summary>
    private void ActiveChecpoint(InputAction.CallbackContext context)
    {
        if (!isCharacterOnTheCollider) return;

        if (!cp.isDiscovered)
        {
            cp.isDiscovered = true;

            StartCoroutine(DiscoveredRadiusAnimation());

            GameManager.instance.HUDManager.ShowPressToInteract("Press F to Rest");
            return;
        }

        if (!cp.isActive)
        {
            GameManager.instance.CheckpointManager.SetCurrentCP(cp);
        }


        GameManager.instance.EnemyManager.RespawnEnemies();
        MainCharacterStats.instance.ResetMainStats();
    }

    // Shows the pop up UI if close enough to the checpoint
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.ModularController.gameObject)
        {
            isCharacterOnTheCollider = true;
            if (!cp.isDiscovered) GameManager.instance.HUDManager.ShowPressToInteract("Press F to Discover");
            else GameManager.instance.HUDManager.ShowPressToInteract("Press F to Rest");
        }
    }

    // disables the pop up UI if close enough to the checpoint
    private void OnTriggerExit(Collider other)
    {
        isCharacterOnTheCollider = false;

        GameManager.instance.HUDManager.DisablePressToInteract();
    }

    IEnumerator DiscoveredRadiusAnimation()
    {
        float lerp = 0;

        float radius = 0;
        float startRadius = 5;
        float endRadius = 40;

        while (lerp < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            lerp += Time.deltaTime / 3;

            radius = Mathf.Lerp(startRadius, endRadius, lerp);

            SetMaterialRadius(radius);
        }
    }

    private void SetMaterialRadius(float _radius)
    {
        if (treeCrown) treeCrown.GetComponent<MeshRenderer>().material.SetFloat("_Radius", _radius);
        if (treeTrunk) treeTrunk.GetComponent<MeshRenderer>().material.SetFloat("_Radius", _radius);
    }
}

