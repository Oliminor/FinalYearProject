using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpTextManager : MonoBehaviour
{
    [SerializeField] GameObject damageText;
    [SerializeField] int InstantiateNumber;
    private List<GameObject> damageTextList = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < InstantiateNumber; i++)
        {
            GameObject go = Instantiate(damageText,transform.position, Quaternion.identity, transform);
            go.SetActive(false);
            damageTextList.Add(go);
        }
    }

    /// <summary>
    /// Instantiate the damage text
    /// </summary>
    public void DamageTextInstiate(int damage, Vector3 _position, bool isCritical, bool isHeal)
    {
        if (damage <= 0) return;

        for (int i = 0; i < damageTextList.Count; i++)
        {
            if(damageTextList[i].activeSelf == false)
            {
                damageTextList[i].SetActive(true);
                damageTextList[i].GetComponentInChildren<DamagePopUp>().DamageNumber(damage, _position, isCritical, isHeal);
                break;
            }
        }
    }
}
