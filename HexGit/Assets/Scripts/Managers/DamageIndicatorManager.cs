using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public enum DamageIndicator { Circle }

public class DamageIndicatorManager : MonoBehaviour
{
    [SerializeField] private Transform CircleDamageInducator;


    /// <summary>
    /// Instantiates the damage damage indicator to the certain position, rotation, size and depth (projection range)
    /// </summary>
    public GameObject AddDamageIndicator(Transform _transform, Vector3 _rotation, Vector2 _size, float _depth)
    {
        GameObject go = Instantiate(CircleDamageInducator.gameObject, _transform.position, Quaternion.identity, _transform);
        go.transform.localRotation = Quaternion.Euler(_rotation);
        go.GetComponent<DecalProjector>().size = new Vector3(_size.x, _size.y, _depth);

        return go;
    }
}
