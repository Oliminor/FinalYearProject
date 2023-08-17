using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------
// CURRENTLY UNUSED SCRIPT but the ide was good
//------------------------

public class ArmaturePhysics : MonoBehaviour
{
    [SerializeField] Transform baseObject;
    [SerializeField] List<Transform> bones;
    [SerializeField] float strength;
    [SerializeField] float lerpSpeed;
    [SerializeField] float freq;
    [SerializeField] float sinMultiplayer;

    Vector3 tempP;
    List<Vector3> defaultT = new();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < bones.Count; i++)
        {
            defaultT.Add(bones[i].localRotation.eulerAngles);
        }
    }

    // Update is called once per frame
    void Update()
    {
        FakeHingePhysics();
    }

    private void FakeHingePhysics()
    {
        Vector3 position = baseObject.worldToLocalMatrix.GetPosition();

        Vector3 posDif = position - tempP;

        Vector3 additionalAngle = new Vector3(posDif.x, posDif.y, posDif.z).normalized;

        additionalAngle *= 10 * strength;

        float sin = Mathf.Sin(3.14f * freq * Time.time) * sinMultiplayer;

        for (int i = 0; i < bones.Count; i++)
        {
            float angleDiff = Quaternion.Angle(bones[0].rotation, bones[i].rotation);

            Quaternion posDest;

            if (angleDiff > 90)
            {
                posDest = Quaternion.Euler(new Vector3(defaultT[i].x + additionalAngle.z + sin, defaultT[i].y - additionalAngle.x - additionalAngle.y + sin, defaultT[i].z));
            }
            else
            {
                posDest = Quaternion.Euler(new Vector3(defaultT[i].x, defaultT[i].y - additionalAngle.x - additionalAngle.y + sin, defaultT[i].z - additionalAngle.z + sin));
            }

            bones[i].localRotation = Quaternion.Lerp(bones[i].localRotation, posDest, lerpSpeed);
        }

        tempP = position;
    }
}
