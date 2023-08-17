using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherTrigger : MonoBehaviour
{
    [SerializeField] private WeatherSO weatherToTrigger;
    [SerializeField] private float lerpTime;

    public void TriggerThisWeather()
    {
        GameManager.instance.WeatherManager.TriggerLerpbetweenWeathers(weatherToTrigger, lerpTime);
    }
}
