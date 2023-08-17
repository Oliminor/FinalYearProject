using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private List<CheckPoint> checkPoints = new();

    private CheckPoint currentCP;

    public List<CheckPoint> GetCheckPoints() { return checkPoints; }
    public CheckPoint GetCurrentCP() { return currentCP; }
    public Vector3 GetCurrentRespawnPoint() { return currentCP.position; }


    /// <summary>
    /// Sets the current checkpoint to the chosen one
    /// </summary>
    public void SetCurrentCP(CheckPoint _cp) 
    {
        foreach (var item in checkPoints)
        {
            item.isActive = false;
        }

        currentCP = _cp;
        currentCP.isActive = true;
    }

    /// <summary>
    /// Adds the newly discovered checkpoint to the list
    /// </summary>
    public void AddCheckPoint(CheckPoint _cp) 
    { 
        checkPoints.Add(_cp);

        foreach (var item in checkPoints)
        {
            if (item.isActive) currentCP = item;
        }
    }
}

[System.Serializable]
public class CheckPoint
{
    public string name;
    public Vector3 position;
    public bool isActive;
    public bool isDiscovered;

    public CheckPoint(string _name, Vector3 _position, bool _isActive, bool _isDiscovered)
    {
        name = _name;
        position = _position;
        isActive = _isActive;
        isDiscovered = _isDiscovered;
    }
}
