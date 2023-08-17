using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=9hTnlp9_wX8 <- from here

public class LineRendererMovement : MonoBehaviour
{
    [SerializeField] private int length;
    [SerializeField] private float targetDistance;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float wiggleSpeed;
    [SerializeField] private float wiggleMagnitude;

    private LineRenderer lineRend;

    private Vector3[] segmentV;
    private Vector3 rotation;
    private Vector3[] segmentPoses;


    // Start is called before the first frame update
    void Start()    
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
        rotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        LineRendererUpdate();
    }

    private void LineRendererUpdate()
    {
        if (segmentPoses.Length > 0)
        {
            transform.localRotation = Quaternion.Euler(rotation.x, rotation.y + Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude, rotation.z); // Wiggle motion using Sinus

            segmentPoses[0] = transform.position;

            for (int i = 1; i < segmentPoses.Length; i++)
            {
                // Moves the segments torwards to the previous element of the array
                segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + transform.forward * targetDistance, ref segmentV[i], smoothSpeed); 
            }
            lineRend.SetPositions(segmentPoses);
        }
    }
}
