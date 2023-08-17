using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CameraZoom cameraZoom;

    [SerializeField] private CinemachineFreeLook playerCamera;
    [SerializeField] private CinemachineFreeLook aimCamera;

    private CameraShake cameraShake;

    private CinemachineFreeLook activeCamera;

    private Vector2 cameraMovementSpeed;

    public CinemachineFreeLook GetActiveCamera() { return activeCamera; }
    public CinemachineFreeLook GetPlayerCamera() { return playerCamera; }
    public CinemachineFreeLook GetAimCamera() { return aimCamera; }


    public CameraShake CameraShake { get { return cameraShake; } }
    public CameraZoom CameraZoom { get { return cameraZoom; } }

    void Awake()
    {
        activeCamera = playerCamera;
        SetCameraToDefault();
        MainCharacterStats.onGameOver += SetCameraToDefault;

        cameraMovementSpeed.x = activeCamera.m_XAxis.m_MaxSpeed;
        cameraMovementSpeed.y = activeCamera.m_YAxis.m_MaxSpeed;
        cameraShake = gameObject.GetComponent<CameraShake>();

        ResetCamera();
    }


    /// <summary>
    /// Sets all camera to zero (usually before camera change, to make sure there is no 2 activate camera at the same time)
    /// </summary>
    private void SetAllCameraPriorityZero()
    {
        playerCamera.Priority = 0;
        aimCamera.Priority = 0;
    }

    /// <summary>
    /// Activate the chosen cinemachine camera
    /// </summary>
    public void ActivateCamera(CinemachineFreeLook _camera)
    {
        if (activeCamera == _camera) return;

        SetAllCameraPriorityZero();

        _camera.m_XAxis.Value = activeCamera.m_XAxis.Value;
        _camera.m_YAxis.Value = activeCamera.m_YAxis.Value;

        _camera.Priority = 1;

        activeCamera = _camera;
    }

    /// <summary>
    /// Stops the cinemachine camera movement
    /// </summary>
    public void DisableCameraMovement()
    {
        activeCamera.m_YAxis.m_MaxSpeed = 0;
        activeCamera.m_XAxis.m_MaxSpeed = 0;
    }

    /// <summary>
    /// Reactivates the cinemachine camera movement
    /// </summary>
    public void ActivateCameraMovement()
    {
        activeCamera.m_YAxis.m_MaxSpeed = cameraMovementSpeed.y;
        activeCamera.m_XAxis.m_MaxSpeed = cameraMovementSpeed.x;
    }

    /// <summary>
    /// Sets the cinemachine camera to the default
    /// </summary>
    public void SetCameraToDefault()
    {
        ActivateCamera(playerCamera);
        activeCamera.m_XAxis.Value = 0;
        activeCamera.m_YAxis.Value = 0.5f;
    }

    public void ResetCamera()
    {
        playerCamera.m_RecenterToTargetHeading.m_enabled = true;
        playerCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0;
        playerCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        playerCamera.m_YAxis.Value = 0.5f;
        Invoke("DisableRecenter", 0.01f);
    }

    /// <summary>
    /// Rotates the cinemachine camera to look at the certain target direction (and the player too) for cutscene 
    /// </summary>
    public void LockPlayerCameraTo(Transform _looksPos)
    {
        GameManager.instance.ModularController.FacePlayerTo(_looksPos.position);

        playerCamera.m_RecenterToTargetHeading.m_enabled = true;
        playerCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0;
        playerCamera.m_RecenterToTargetHeading.m_WaitTime = 0;
        playerCamera.m_YAxis.Value = 0.6f;
        Invoke("DisableRecenter", 0.1f);
    }

    private void DisableRecenter()
    {
        playerCamera.m_RecenterToTargetHeading.m_enabled = false;
    }
}
