using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardForSprites : MonoBehaviour
{
    // Should change this into a list and add the sprites to, instead of adding this to every sprite one by one (UPDATE)
    void FixedUpdate()
    {
        if (Camera.main) transform.rotation = Camera.main.transform.rotation;
    }
}
