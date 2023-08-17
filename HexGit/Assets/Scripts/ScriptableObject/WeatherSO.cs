using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeatherSettings", menuName = "ScriptableObjects/WeatherSettings", order = 1)]
public class WeatherSO : ScriptableObject
{
    [SerializeField] private WeatherSettings weatherSettings;

    public WeatherSettings WeatherSettings { get { return weatherSettings; } set { weatherSettings = value; } }
}
