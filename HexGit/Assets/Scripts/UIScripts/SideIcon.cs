using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideIcon : MonoBehaviour
{
    [SerializeField] private RectTransform sideIconSelect;
    [SerializeField] private List<IconData> iconList;

    private void Start()
    {
        ActivateActiceStanceIcons();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIconCooldown();
    }

    // Activates the current stance icon
    private void ActivateActiceStanceIcons()
    {
        foreach (var icon in iconList) icon.icon.gameObject.SetActive(false);

        for (int i = 0; i < iconList.Count; i++)
        {
            if (GameManager.instance.StanceManager.GetStances()[i].GetComponent<StanceSkillBase>().GetIsStanceActive()) iconList[i].icon.gameObject.SetActive(true);
        }

        ChangeSelectedIcon(GameManager.instance.StanceManager.GetActiveStanceIndex());
    }

    /// <summary>
    /// Change to the selected Icon (lerp)
    /// </summary>
    public void ChangeSelectedIcon(int _index)
    {
        StartCoroutine(SelectorLerp(_index));
    }

    private IEnumerator SelectorLerp(int _index)
    {
        float lerpTime = 0;
        float lerpStep = 0.1f;
        Vector3 fromPosition = sideIconSelect.position;

        while (lerpTime < 1)
        {

            yield return new WaitForSecondsRealtime(0.01f);
            lerpTime += lerpStep;

            sideIconSelect.position = Vector3.Lerp(fromPosition, iconList[_index].icon.position, lerpTime);

            lerpStep -= 0.1f;
            lerpStep = Mathf.Clamp(lerpStep, 0.1f, 1.0f);
        }
    }

    /// <summary>
    /// Activate the new icon when the stance acquired
    /// </summary>
    public void ActivateIcon(int _index)
    {
        iconList[_index].icon.gameObject.SetActive(true);
    }

    /// <summary>
    /// Update the cooldown on the icon when the buff is active
    /// </summary>
    private void UpdateIconCooldown()
    {
        foreach (var icon in iconList)
        {
            icon.icon.GetComponent<Image>().fillAmount = GameManager.instance.CooldownManager.GetCooldownTImeLeftNormalized(icon.index);
        }
    }

    [System.Serializable]
    public class IconData
    {
        public RectTransform icon;
        public int index;
    }
}

