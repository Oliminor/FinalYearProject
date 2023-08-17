using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] Transform parentObject;
    [SerializeField] float returnTime;
    Vector3 localPosition;
    bool isCarried = false;
    bool isPuzzleFinished = false;

    private void Start()
    {
        localPosition = transform.localPosition;
    }

    /// <summary>
    /// Changes the state to finished (Set to the parent object)
    /// </summary>
    public void  FinishPuzzle(Transform _transform)
    {
        StartCoroutine(ChangePositionBezier(_transform));
        isPuzzleFinished = true;
    }

    // When the player walk through it, it picks up and place them inside the Player Stash (Empty objects behind the character, with 3 slot)
    private void OnTriggerEnter(Collider other)
    {
        if (isCarried) return;
        if (transform.position != parentObject.transform.position) return;
        if (other.tag != "Player") return;
        if (GameManager.playerStash.FreeSlotNumber() == 0) return;

        isCarried = true;

        transform.parent = GameManager.playerStash.FreeSlotTransform();
        transform.localPosition = Vector3.zero;

        StartCoroutine(Return(returnTime));
    }

    private IEnumerator ChangePositionBezier(Transform _parent)
    {
        List<Vector3> bezierList = new();

        transform.parent = _parent;

        bezierList.Add(transform.position);
        bezierList.Add(new Vector3(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(-1f, 1f), transform.position.z + Random.Range(-3f, 3f)));
        bezierList.Add(_parent.position);

        float bezierPosition = 0;

        while (bezierPosition < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            bezierPosition += Time.deltaTime;
            transform.position = drawCurve(bezierList, bezierPosition);
        }

        transform.localPosition = Vector3.zero;
    }

    private Vector3 drawCurve(List<Vector3> points, float t)
    {
        if (points.Count == 1)
        {
            return points[0];
        }
        else
        {
            List<Vector3> newpoints = new List<Vector3>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                float x = (1f - t) * points[i].x + t * points[i + 1].x;
                float y = (1f - t) * points[i].y + t * points[i + 1].y;
                float z = (1f - t) * points[i].z + t * points[i + 1].z;
                newpoints.Add(new Vector3(x, y, z));
            }
            return drawCurve(newpoints, t);
        }
    }


    // After N amount of time, if the puzzle is not solved, it will returns to the original position
    // (for example, the player walks away and not bother to finish the puzzle)
    private IEnumerator Return(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (!isPuzzleFinished)
        {
            StartCoroutine(ChangePositionBezier(parentObject));
            isCarried = false;
        }
    }
}
