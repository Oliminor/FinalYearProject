using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshEffect : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinnedMesh;
    [SerializeField] VisualEffect vfx;
    float refreshRate = 0.05f;

    Material dissolveMat;
    bool isEnemyDead = false;

    float dissolveValue = 1;
    float spawnRateValue = 500;
    
    public void SetIsEnemyDead() { isEnemyDead = true; }

    void Start()
    {
        dissolveMat = skinnedMesh.transform.GetComponent<SkinnedMeshRenderer>().material;
        float scale = vfx.transform.localScale.x / transform.localScale.x;
        vfx.transform.localScale = new Vector3(scale, scale, scale);
        StartCoroutine(UpdateVFXGraph());
    }

    private void Update()
    {
        DeadDissolve();
    }

    // VFX graph need a mesh to emit from, and this function feed the current model to the VFX graph after every refreshRate.
    /// <summary>
    /// Updates the mesh for the VFX Graph
    /// </summary>
    IEnumerator UpdateVFXGraph()
    {
        Mesh _mesh = new Mesh(); // Create mesh once
        while (gameObject.activeSelf)
        {
            _mesh.Clear(); // Clear the current mesh vertexes
            skinnedMesh.BakeMesh(_mesh); // bake a new one
            vfx.SetMesh("Mesh", _mesh); // send it o the VFX Graph

            yield return new WaitForSeconds(refreshRate);
        }
    }

    /// <summary>
    /// Slows down the emitting rate to 0 (lerp) when the enemie is eliminiated
    /// </summary>
    private void DeadDissolve()
    {
        if (!isEnemyDead) return;
        dissolveValue = Mathf.Lerp(dissolveValue, 0, 0.005f);
        spawnRateValue = Mathf.Lerp(spawnRateValue, 0, 0.01f);

        DissolveRate(dissolveValue);
        VFXSPawnRate((int)spawnRateValue);
    }

    /// <summary>
    /// Sets the dissolve rate
    /// </summary>
    public void DissolveRate(float _dissolveAmount)
    {
        dissolveMat.SetFloat("_DissolveAmount", _dissolveAmount);
    }

    /// <summary>
    /// Sets the VFX spawn rate
    /// </summary>
    public void VFXSPawnRate(int _spawnRate)
    {
        vfx.SetInt("SpawnRate", _spawnRate);
    }
}
