using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingSpearProjectile : Projectile
{
    [SerializeField] private int piercingTargetNumber;
    [SerializeField] private float checkRadius;

    private HashSet<Transform> targetList = new();
    private Transform currentTarget;

    public bool newTargetTransition;
    private float defaultSpeed;

    IEnumerator switchTarget;

    private void Start()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        defaultSpeed = speed;
        SwitchTargetTrigger();
    }

    private void Update()
    {
        float distance = Vector3.Distance(previousPosition, transform.position);
        RaycastHit hit;

        if (Physics.Raycast(previousPosition, transform.forward, out hit, distance, targetLayer))
        {
            if (hit.transform == currentTarget)
            {
                piercingTargetNumber--;

                SwitchTargetTrigger();

                GameManager.instance.DamageCheck.PlayerSingleDamage(hit.point, 0.1f, damage, null, targetLayer, BuffTrigger.SkillAttack, 0f);

                if (hitEffect)
                {
                    GameObject hitEffectParticle = Instantiate(hitEffect.gameObject, hit.point, Quaternion.identity);
                    Destroy(hitEffectParticle, 0.5f);
                }

                if (piercingTargetNumber <= 0) Destroy(gameObject, 2.0f);
            }
        }

        if (currentTarget)
        {
            Quaternion rotateTowradsTarget = Quaternion.LookRotation((currentTarget.position + Vector3.up * 2) - transform.position);

            if (!newTargetTransition) rb.rotation = rotateTowradsTarget;
        }

        if(rb) rb.velocity = rb.transform.forward * speed;

        previousPosition = transform.position;
    }

    private void SwitchTargetTrigger()
    {
        if (switchTarget != null) StopCoroutine(switchTarget);

        switchTarget = SwitchToNewTarget();

        StartCoroutine(switchTarget);
    }

    IEnumerator SwitchToNewTarget()
    {
        currentTarget = NewTarget();

        newTargetTransition = true;

        float lerp = 0;
        float slowSpeed = defaultSpeed / 5;

        Quaternion endRotation = Quaternion.LookRotation(Vector3.up);
        Quaternion startRotation = rb.rotation;

        while (lerp < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            lerp += Time.deltaTime * 3;

            lerp = Mathf.Clamp01(lerp);

            rb.rotation = Quaternion.Lerp(startRotation, endRotation, lerp);
            speed = Mathf.Lerp(defaultSpeed, slowSpeed, lerp);
        }

        if (!currentTarget)
        {
            Destroy(gameObject, 5.0f);
            yield break;
        }

        lerp = 0;

        while (lerp < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            lerp += Time.deltaTime * 3;

            lerp = Mathf.Clamp01(lerp);

            endRotation = Quaternion.LookRotation((currentTarget.position + Vector3.up * 2) - transform.position);
            rb.rotation = Quaternion.Lerp(rb.rotation, endRotation, lerp);
            speed = Mathf.Lerp(slowSpeed, defaultSpeed, lerp);
        }

        newTargetTransition = false;
    }

    private Transform NewTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius, targetLayer);

        if (hitColliders.Length == 0) return null;

        float smallestDistance = Mathf.Infinity;

        Transform target = null;

        foreach (var hit in hitColliders)
        {
            if (hit.transform.TryGetComponent(out IDamageTaken _takeDamage))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (distance < smallestDistance)
                {
                    if (targetList.Contains(hit.transform)) continue;

                    smallestDistance = distance;
                    target = hit.transform;
                }
            }
        }

        targetList.Add(target);

        return target;
    }
}
