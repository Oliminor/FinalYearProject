using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowArrowProjectile : EnemyProjectile
{
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Disable the shadow arrow effects
    /// </summary>
    protected override void DestroyThis()
    {
        base.DestroyThis();

        if (transform.TryGetComponent(out DissolveEffectManager _dissolve))
        {
            _dissolve.DissolveOff();
        }

        if (transform.GetChild(0).TryGetComponent(out ParticleSystem _particle))
        {
            _particle.Stop();
        }
    }
}
