using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class FireFlickering : MonoBehaviour
{

    [SerializeField] float flickerSpeed;
    [SerializeField] float lerpSpeed;
    [SerializeField] float minIntensity;
    [SerializeField] float maxIntensity;
    float intensity;
    float changeRate;
    HDAdditionalLightData pointLight;

    // Start is called before the first frame update
    void Start()
    {
        changeRate = flickerSpeed;
        pointLight = GetComponent<HDAdditionalLightData>();
    }

    // Update is called once per frame
    void Update()
    {
        LightUpdate();
    }

    /// <summary>
    /// Randomly changes the light intensity to create fire flicker effect
    /// </summary>
    private void LightUpdate()
    {
        pointLight.lightDimmer = Mathf.Lerp(pointLight.lightDimmer, intensity, lerpSpeed);

        changeRate -= Time.deltaTime;

        if (changeRate <= 0)
        {
            changeRate = flickerSpeed;
            intensity = Random.Range(minIntensity, maxIntensity);
        }
    }
}
