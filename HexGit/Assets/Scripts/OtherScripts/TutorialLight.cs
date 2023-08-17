using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLight : MonoBehaviour
{
    private LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();

        lr.SetPosition(0, transform.position);

        lr.SetPosition(1, transform.position + Vector3.up * 8);
    }
}
