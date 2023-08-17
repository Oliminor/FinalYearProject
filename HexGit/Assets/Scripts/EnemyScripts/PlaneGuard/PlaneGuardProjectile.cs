using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGuardProjectile : EnemyProjectile
{
    private bool isDissolve = false;

    public void SetDissolve(bool _bool) { isDissolve = _bool; }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Initailaze()
    {
        base.Initailaze();

        if (isDissolve)
        {
            if (transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveOn();
        }
    }

    /// <summary>
    /// When the projectile landed it canges the position randomly so it doesn't look like uniformed
    /// </summary>
    protected override void ProjectileLanded(RaycastHit hit)
    {
        base.ProjectileLanded(hit);

        if (isArrow)
        {
            float arrowSize = transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.size.z;
            float stickingOutPos = Random.Range(arrowSize / 4, arrowSize / 1.5f);
            transform.position = transform.position - transform.forward * stickingOutPos;
        }
    }

    /// <summary>
    /// Destroys the gameobject and the effects around it
    /// </summary>
    protected override void DestroyThis()
    {
        base.DestroyThis();

        if (transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolve))
        {
            _dissolve.DissolveOff();
        }

        if (transform.GetChild(1).TryGetComponent(out ParticleSystem _particle))
        {
            _particle.Stop();
        }
    }
}
