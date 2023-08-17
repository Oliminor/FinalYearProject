using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] Volume volume;
    [SerializeField] Light sun;

    [SerializeField] private WeatherSO weatherSO;
    [SerializeField] private WeatherSettings testWeather;

    private VolumeProfile profile;
    private WeatherSO tempWeatherSO;

    void Start()
    {
        profile = volume.sharedProfile;
    }

    /// <summary>
    /// Triggers the weather lerp Coroutine
    /// </summary>
    public void TriggerLerpbetweenWeathers(WeatherSO _scriptableObj, float _time)
    {
        StartCoroutine(LerpBetweenWeather(_scriptableObj, _time));
    }

    // Lerp between the start (current) data and the scriptable object data
    IEnumerator LerpBetweenWeather(WeatherSO _scriptableObj, float _time)
    {
        float lerp = 0;

        profile.TryGet<VolumetricClouds>(out var _clouds);

        float startDensity = _clouds.densityMultiplier.value;
        float endDensity = _scriptableObj.WeatherSettings.cloudDensityMultiplier;

        float startShapeFactor = _clouds.shapeFactor.value;
        float endShapeFactor = _scriptableObj.WeatherSettings.cloudShapeFactor; ;

        Vector3 startRotation = new Vector2(sun.transform.eulerAngles.x, sun.transform.eulerAngles.y);
        Vector3 endRotation = _scriptableObj.WeatherSettings.sunDirection;

        float startIntensity = sun.intensity;
        float endIntensity = _scriptableObj.WeatherSettings.luxIntensity; ;

        float startColorTemperature = sun.colorTemperature;
        float endColorTemperature = _scriptableObj.WeatherSettings.temperatureKelvin;

        Color startColor = sun.color;
        Color endColor = _scriptableObj.WeatherSettings.filterColor;

        while (lerp < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime / _time);
            lerp += Time.deltaTime / _time;

            _clouds.densityMultiplier.value = Mathf.Lerp(startDensity, endDensity, lerp);
            _clouds.shapeFactor.value = Mathf.Lerp(startShapeFactor, endShapeFactor, lerp);

            sun.transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), lerp);
            sun.intensity = Mathf.Lerp(startIntensity, endIntensity, lerp);
            sun.colorTemperature = Mathf.Lerp(startColorTemperature, endColorTemperature, lerp);
            sun.color = Color.Lerp(startColor, endColor, lerp);
        }
    }

    // This function calls on editor mode, when the inspector values are changes
    // Nice way to Load in the new scriptable object values in an instant without playing the game
    private void OnValidate()
    {
        profile = volume.sharedProfile;

        profile.TryGet<VolumetricClouds>(out var _clouds);

        LoadScritableObject();

        _clouds.densityMultiplier.value = testWeather.cloudDensityMultiplier;
        _clouds.shapeFactor.value = testWeather.cloudShapeFactor;

        sun.transform.rotation = Quaternion.Euler(new Vector2(testWeather.sunDirection.x, testWeather.sunDirection.y));
        sun.intensity = testWeather.luxIntensity;
        sun.colorTemperature = testWeather.temperatureKelvin;
        sun.color = testWeather.filterColor;

        SaveOutScriptableObject();
    }

    /// <summary>
    /// Loads  in the scriptable object datas
    /// </summary>
    private void LoadScritableObject()
    {
        if (tempWeatherSO == weatherSO) return;

        testWeather.cloudDensityMultiplier = weatherSO.WeatherSettings.cloudDensityMultiplier;
        testWeather.cloudShapeFactor = weatherSO.WeatherSettings.cloudShapeFactor;

        testWeather.sunDirection = weatherSO.WeatherSettings.sunDirection;
        testWeather.luxIntensity = weatherSO.WeatherSettings.luxIntensity;
        testWeather.temperatureKelvin = weatherSO.WeatherSettings.temperatureKelvin;
        testWeather.filterColor = weatherSO.WeatherSettings.filterColor;

        tempWeatherSO = weatherSO;
    }

    /// <summary>
    /// Saves the datas into the scriptable object
    /// </summary>
    private void SaveOutScriptableObject()
    {
        if (!weatherSO) return;

        weatherSO.WeatherSettings.cloudDensityMultiplier = testWeather.cloudDensityMultiplier;
        weatherSO.WeatherSettings.cloudShapeFactor = testWeather.cloudShapeFactor;

        weatherSO.WeatherSettings.sunDirection = testWeather.sunDirection;
        weatherSO.WeatherSettings.luxIntensity = testWeather.luxIntensity;
        weatherSO.WeatherSettings.temperatureKelvin = testWeather.temperatureKelvin;
        weatherSO.WeatherSettings.filterColor = testWeather.filterColor;
    }
}

[System.Serializable]
public class WeatherSettings
{
    public Vector2 sunDirection;
    public float luxIntensity;
    public float temperatureKelvin;
    public float cloudDensityMultiplier;
    public float cloudShapeFactor;
    public Color filterColor;
}
