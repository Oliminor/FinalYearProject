/*
 Just a simple camera shake script, using cinemachine 6D Shake function
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineFreeLook vcam;


    void Start()
    {
        TriggerCameraShake(0, 0); // Set the camera shake to zero (safety)
    }

    /// <summary>
    /// Turns on the Camera shake for all axis
    /// </summary>
    public IEnumerator CameraAmplitude(int shakeStrength, float _time)
    {
        vcam = GameManager.instance.CameraManager.GetActiveCamera();
        vcam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain += shakeStrength;
        vcam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain += shakeStrength;
        vcam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain += shakeStrength;
        yield return new WaitForSecondsRealtime(_time);
        vcam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain -= shakeStrength;
        vcam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain -= shakeStrength;
        vcam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain -= shakeStrength;
    }

    /// <summary>
    /// Calls the Coroutine
    /// </summary>
    public void TriggerCameraShake(int shakeStrength, float _time)
    {
        StartCoroutine(CameraAmplitude(shakeStrength, _time));
    }
}
