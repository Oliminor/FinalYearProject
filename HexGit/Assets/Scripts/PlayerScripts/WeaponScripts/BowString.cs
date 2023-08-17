using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowString : MonoBehaviour
{
    [SerializeField] List<Transform> stringPositions;
    [SerializeField] Transform rightHand;

    private LineRenderer lineR;
    [SerializeField] bool bowDraw = false;

    public Transform GetRightHand() { return rightHand; }

    void Start()
    {
        lineR = GetComponent<LineRenderer>();
        lineR.positionCount = stringPositions.Count;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetStringPositions();
    }

    // The bow string (Linerenderer) is attached to the bow top and bottom of the bow
    // when the bowDraw is true, it also attached to the hand position
    private void SetStringPositions()
    {
        lineR.SetPosition(0, stringPositions[0].position);

        if (bowDraw) lineR.SetPosition(1, stringPositions[1].position);
        else lineR.SetPosition(1, stringPositions[2].position);

        lineR.SetPosition(2, stringPositions[2].position);
    }

    public void SetDrawMode(bool _bool)
    {
        bowDraw = _bool;
    }

    public void DisableBow()
    {
        gameObject.SetActive(false);
    }
}
