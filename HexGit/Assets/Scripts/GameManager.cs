using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shouldn't exits anymore (UPDATE)
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Vector3 mousePosition;

    //Managers
    [SerializeField] private ModularController playerController;
    [SerializeField] private StanceManager stanceManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private HUDManager hUDManager;
    [SerializeField] private CooldownManager cooldownManager;
    [SerializeField] private DamageIndicatorManager damageIndicatorManager;
    [SerializeField] private BuffManager buffManager;
    [SerializeField] private CheckpointManager checkpointManager;
    [SerializeField] private DamageCheck damageCheck;
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private EnemyManagers enemyManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PathFinder pathFinder;

    public static StashManager playerStash;

    public ModularController ModularController { get { return playerController; } }
    public StanceManager StanceManager { get { return stanceManager; } }
    public DataManager DataManager { get { return dataManager; } }
    public CameraManager CameraManager { get { return cameraManager; } }
    public HUDManager HUDManager { get { return hUDManager; } }
    public CooldownManager CooldownManager { get { return cooldownManager; } }
    public DamageIndicatorManager DamageIndicatorManager { get { return damageIndicatorManager; } }
    public BuffManager BuffManager { get { return buffManager; } }
    public CheckpointManager CheckpointManager { get { return checkpointManager; } }
    public DamageCheck DamageCheck { get { return damageCheck; } }
    public WeatherManager WeatherManager { get { return weatherManager; } }
    public EnemyManagers EnemyManager { get { return enemyManager; } }
    public InputManager InputManager { get { return inputManager; } }
    public PathFinder PathFinder { get { return pathFinder; } }

    void Awake()
    {
        if (instance != null) Debug.LogError("We have a problem chief - singleton instace is already in use (GameManager)");
        instance = this;
    }
}   
