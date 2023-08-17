using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHex : MonoBehaviour
{
    Transform player;
    Material mat;
    CapsuleCollider capsule;
    Vector3 playerMiddlePoint;
    void Start()
    {
        player =  GameManager.instance.ModularController.transform;
        mat = GetComponent<MeshRenderer>().material;
        capsule = player.GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMiddlePoint = capsule.bounds.center;
        mat.SetVector("_ObjectPos", transform.position);
        mat.SetVector("_CharacterPos", playerMiddlePoint);
    }
}
