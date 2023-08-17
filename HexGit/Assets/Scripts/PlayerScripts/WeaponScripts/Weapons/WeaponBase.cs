using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected float animationSpeed;
    [SerializeField] protected SkillModule skillModule;
    [SerializeField] protected LayerMask solidObject;
    [SerializeField] protected Transform hitEffectPrefab;

    [SerializeField] private float dissolveDelayTime;

    protected ModularController player;
    protected Animator anim;

    [SerializeField] private List<LineRenderer> lineRendererList;
    [SerializeField] private List<MeshRenderer> meshRList;
    [SerializeField] private List<SkinnedMeshRenderer> sMeshRList;

    private float dissolveAmount;

    private IEnumerator switchOffWeapon;
    private IEnumerator switchOnWeapon;

    public Transform HitEffectPrefab { get { return hitEffectPrefab; } }
    public SkillModule SkillModule { get { return skillModule; } }
    public int Index { get { return skillModule.Index; } }


    public abstract bool AttackTrigger();

    virtual public void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        DissolveAmountUpdate(1);
    }

    virtual protected void Initialize()
    {
        player = GameManager.instance.ModularController;

        if (TryGetComponent(out Animator animator)) anim = animator;

        if (TryGetComponent(out MeshRenderer _meshRenderer)) meshRList.Add(_meshRenderer);

        if (TryGetComponent(out SkinnedMeshRenderer _sMeshRenderer)) sMeshRList.Add(_sMeshRenderer);

        if (TryGetComponent(out LineRenderer _LineRenderer)) lineRendererList.Add(_LineRenderer);
    }

    public virtual void DissolveEffect(bool _bool)
    {
        if (_bool)
        {
            SwitchRenderer(true);
            switchOnWeapon = SwitchOnDissolve();
            StartCoroutine(switchOnWeapon);
            if (switchOffWeapon != null) StopCoroutine(switchOffWeapon);
        }
        else
        {
            switchOffWeapon = SwitchOffDissolve();
            StartCoroutine(switchOffWeapon);
            if (switchOnWeapon != null) StopCoroutine(switchOnWeapon);
        }
    }

    protected void SwitchRenderer(bool _bool)
    {
        foreach (var _meshR in meshRList) _meshR.enabled = _bool;
    }

    IEnumerator SwitchOnDissolve()
    {
        float lerpTimer = 0;
        while (lerpTimer < 1)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            lerpTimer += Time.fixedDeltaTime * 3;
            lerpTimer = Mathf.Min(lerpTimer, 1);
            dissolveAmount = Mathf.Lerp(1, 0, lerpTimer);
            DissolveAmountUpdate(dissolveAmount);
        }
    }

    IEnumerator SwitchOffDissolve()
    {
        yield return new WaitForSeconds(dissolveDelayTime);

        float lerpTimer = 0;
        while (lerpTimer < 1)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            lerpTimer += Time.fixedDeltaTime;
            lerpTimer = Mathf.Min(lerpTimer, 1);
            dissolveAmount = Mathf.Lerp(0, 1, lerpTimer);
            DissolveAmountUpdate(dissolveAmount);
        }

        SwitchRenderer(false);
    }

    protected void DissolveAmountUpdate(float _dissolveAmount)
    {
        foreach (var _meshR in meshRList) _meshR.material.SetFloat("_dissAmount", _dissolveAmount);

        foreach (var _sMeshR in sMeshRList) _sMeshR.material.SetFloat("_dissAmount", _dissolveAmount);

        foreach (var _lineR in lineRendererList) _lineR.material.SetFloat("_dissAmount", _dissolveAmount);
    }
}
