using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RangedStanceSkill : StanceSkillBase
{
    [SerializeField] private Transform bowParent;

    
    // Start is called before the first frame update
    void Start()
    {
        Initalize();
    }

    // Update is called once per frame
    void Update()
    {
        BowRotation();
    }

    public override void Initalize()
    {
        base.Initalize();

        GameManager.instance.InputManager.GetInputActions().Player.RightMouseButton.started += BasicAimOn;
        GameManager.instance.InputManager.GetInputActions().Player.RightMouseButton.canceled += BasicAimOff;
    }
    private void BasicAimOn(InputAction.CallbackContext context)
    {
        if (player.isBusy()) return;

        if (!gameObject.activeSelf) return;

        player.IsAiming = true;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("DrawBow")) anim.SetTrigger("aimTrigger");
        LeftClickSlot.GetComponent<RangedWeapon>().AimMode(true);
        LeftClickSlot.DissolveEffect(true);
    }
    private void BasicAimOff(InputAction.CallbackContext context)
    {
        if (!gameObject.activeSelf) return;

        player.IsAiming = false;

        LeftClickSlot.GetComponent<RangedWeapon>().AimMode(false);
        LeftClickSlot.DissolveEffect(false);
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


    // Rotates the bow based on the camera rotation
    private void BowRotation()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        float rayLength = 200.0f;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength, solidObject))
        {
            bowParent.transform.LookAt(hit.point);
        }
        else
        {
            bowParent.transform.LookAt(ray.GetPoint(1000));
        }
    }
}
