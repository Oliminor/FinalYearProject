using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    private CinemachineFreeLook freelook;
    private CinemachineFreeLook.Orbit[] baseOrbits;

    private float zoomPercent = 1;
    private float tempZoomPercent = 1;

    public void SetZoom(float _zoomPercent) 
    { 
        zoomPercent = _zoomPercent;
        tempZoomPercent = zoomPercent;
    }

    private void Awake()
    {
        freelook = GetComponentInChildren<CinemachineFreeLook>();
        baseOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];


        for (int i = 0; i < freelook.m_Orbits.Length; i++)
        {
            baseOrbits[i].m_Height = freelook.m_Orbits[i].m_Height;
            baseOrbits[i].m_Radius = freelook.m_Orbits[i].m_Radius;
        }
        SetZoom(2.0f);
    }

    private void Start()
    {
         GameManager.instance.InputManager.GetInputActions().UI.ScrollWheel.performed += ZoomCloser; // ScrollWheel actions added to the input manager
         GameManager.instance.InputManager.GetInputActions().UI.ScrollWheel.performed += ZoomBack;
    }

    /// <summary>
    /// Zoom closer to the player
    /// </summary>
    private void ZoomCloser(InputAction.CallbackContext context) { if (context.ReadValue<Vector2>().y < 0) tempZoomPercent += 0.1f; }

    /// <summary>
    /// Zoom further to the player
    /// </summary>
    private void ZoomBack(InputAction.CallbackContext context) { if (context.ReadValue<Vector2>().y > 0) tempZoomPercent -= 0.1f; }

    public void Update()
    {
        ZoomLerp();
    }

    // Both Cinemachine Height and Radius needs to be change to have the correct zoom effect
    // Only the radius moves the camera either higher or lower and changes the camera angle
    /// <summary>
    /// Lerps both camera Height and Radius changes
    /// </summary>
    private void ZoomLerp()
    {
        tempZoomPercent = Mathf.Clamp(tempZoomPercent, 0.5f, 3.0f);

        if (zoomPercent != tempZoomPercent) zoomPercent = Mathf.Lerp(zoomPercent, tempZoomPercent, 0.01f);

        /// Cinemachine has 3 orbit around the player, all of them needed to change
        for (int i = 0; i < freelook.m_Orbits.Length; i++)
        {
            freelook.m_Orbits[i].m_Height = baseOrbits[i].m_Height * zoomPercent;
            freelook.m_Orbits[i].m_Radius = baseOrbits[i].m_Radius * zoomPercent;
        }
    }
}
