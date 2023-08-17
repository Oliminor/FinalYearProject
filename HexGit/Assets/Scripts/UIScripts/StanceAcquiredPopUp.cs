using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StanceAcquiredPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;


    /// <summary>
    ///  Changes the text and icon image on the stance acquired pop up
    /// </summary>
    public void TriggerStanceAcquired(string _text, string _imagePath)
    {
        text.text = _text;
        icon.sprite = Resources.Load<Sprite>(_imagePath);
    }
}
