using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DamageIndicatorLerp : MonoBehaviour
{
    private DecalProjector projector;

    private void Awake()
    {
        projector = GetComponent<DecalProjector>();
    }

    public void TriggerDamageIndicator()
    {
        StartCoroutine(DamageIndicatorStarLerp());
    }

    IEnumerator DamageIndicatorStarLerp()
    {
        projector.fadeFactor = 0;
        float lerp = 0;

        while (lerp < 0.75f)
        {
            yield return new WaitForSeconds(0.01f);
            lerp += 0.01f;
            projector.fadeFactor = lerp;
        }
    }
}
