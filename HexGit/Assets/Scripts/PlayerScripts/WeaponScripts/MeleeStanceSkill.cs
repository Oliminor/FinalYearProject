using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeStanceSkill : StanceSkillBase
{
    // Start is called before the first frame update
    void Start()
    {
        Initalize();
    }
    
    private void OnDisable()
    {
        if (GameManager.instance.HUDManager) GameManager.instance.HUDManager.ToggleTargetEnemyIndicator(false, Vector3.zero);
    }

    private void FixedUpdate()
    {
        ShowTeleportIndicator();
    }

    public override void Initalize()
    {
        base.Initalize();
    }

    public override void AttackTrigger(AttackButton _button)
    {
        switch (_button)
        {
            case AttackButton.LEFTCLICK:
                if (LeftClickSlot) LeftClickSlot.AttackTrigger();
                break;
            case AttackButton.SKILL01:
                if (qSkillAttack) qSkillAttack.AttackTrigger();
                break;
            case AttackButton.SKILL02:
                if (eSkillAttack) eSkillAttack.AttackTrigger();
                break;
        }
    }

    /// <summary>
    /// Switch on the target indicator if the enemy is close enough
    /// </summary>
    private void ShowTeleportIndicator()
    {
        GameManager.instance.HUDManager.ToggleTargetEnemyIndicator(false, Vector3.zero);

        Transform enemy;
        if (TargetEnemy(out enemy))
        {
            float sizeOfEnemy = enemy.GetComponent<Collider>().bounds.size.y / 2;
            Vector3 position = Camera.main.WorldToScreenPoint(enemy.position + Vector3.up * sizeOfEnemy);
            GameManager.instance.HUDManager.ToggleTargetEnemyIndicator(true, position);
        }
    }

    /// <summary>
    /// Checks if the enemy is close enough to the character
    /// </summary>
    private bool TargetEnemy(out Transform _target)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        float rayLength = 30.0f;
        RaycastHit hit;

        if (Physics.SphereCast(ray.origin, 3f, ray.direction, out hit, rayLength, target))
        {
            _target = hit.transform;
            if (hit.distance > 15f) return true;
        }

        _target = null;
        return false;
    }
}
