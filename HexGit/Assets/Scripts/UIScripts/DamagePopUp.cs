using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    [SerializeField] GameObject parentObject;
    [SerializeField] private float destroyTime = 1;
    [SerializeField] private float randomNumber = 0.4f;
    
    private Vector3 position;
    private Vector2 randomPosition;
    private Animator anim;
    private RectTransform parentRectT;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        parentRectT = GetComponentInParent<RectTransform>();
    }

    void OnEnable()
    {
        destroyTime = 1;

        randomPosition.x = Random.Range(-randomNumber, randomNumber);
        randomPosition.y = Random.Range(-randomNumber, randomNumber);
    }

    void Update()
    {
        Vector3 selectedPosition = Camera.main.WorldToScreenPoint(new Vector3(position.x + randomPosition.x, position.y, position.z + randomPosition.y));
        parentRectT.position = new Vector3(selectedPosition.x, selectedPosition.y, selectedPosition.z);

        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0) parentObject.SetActive(false);
    }

    /// <summary>
    /// Set the Damage pop Up number text
    /// </summary>
    public void DamageNumber(int damage, Vector3 _position, bool isCritical, bool isHeal)
    {
        anim.SetFloat("DamageType", 0);
        if (isCritical) anim.SetFloat("DamageType", 1);

        float damagetype = 0;                  // normal damage
        if (isHeal) damagetype = 1;            // heal

        position = _position;

        TextMeshProUGUI textmesh = GetComponent<TextMeshProUGUI>();

        switch (damagetype)
        {
            case 0:
                textmesh.color = Color.white;
                textmesh.text = damage.ToString();
                break;

            case 1:
                textmesh.color = Color.green;
                textmesh.text = "+" + damage.ToString();
                break;
        }
    }
}