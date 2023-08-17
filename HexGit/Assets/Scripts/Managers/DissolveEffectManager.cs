using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffectManager : MonoBehaviour
{
    MeshRenderer meshRenderer;

    Material dissolveMat;

    IEnumerator dissolveIE;

    private void Awake()
    {
        Initalize();
    }

    private void Initalize()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        dissolveMat = meshRenderer.material;
    }

    public void DissolveRate(float _dissolveAmount)
    {
        if (dissolveIE != null) StopCoroutine(dissolveIE);
        dissolveMat.SetFloat("_DissolveAmount", _dissolveAmount);
    }

    public void DissolveOff()
    {
        if (dissolveIE != null) StopCoroutine(dissolveIE);
        dissolveIE = Dissolve(1, 0);
        StartCoroutine(dissolveIE);
    }

    public void DissolveOn()
    {
        if (dissolveIE != null) StopCoroutine(dissolveIE);
        dissolveIE = Dissolve(0, 1);
        StartCoroutine(dissolveIE);
    }

    IEnumerator Dissolve(float start, float end)
    {
        float lerp = 0;

        while (lerp < 1)
        {
            yield return new WaitForSeconds(0.005f);
            lerp += 0.005f;

            float dissolveValue = Mathf.Lerp(start, end, lerp);
            dissolveMat.SetFloat("_DissolveAmount", dissolveValue);
        }
    }
}
