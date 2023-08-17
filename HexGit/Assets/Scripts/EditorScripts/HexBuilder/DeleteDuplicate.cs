using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------
// CURRENTLY UNUSED SCRIPT
//------------------------

[ExecuteInEditMode]
public class DeleteDuplicate : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroyDuplicate());
    }
    // Start is called before the first frame update
    public IEnumerator DestroyDuplicate()
    {
        bool deleteThis = false;

        yield return new WaitForSecondsRealtime(0.01f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.5f);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Hex" && hitColliders[i].transform.position == transform.position && hitColliders[i].gameObject != gameObject) deleteThis = true;
        }

        if (deleteThis)
        {
            DestroyImmediate(gameObject);
        }
        if (!deleteThis) DestroyImmediate(GetComponent<DeleteDuplicate>());
    }
}
