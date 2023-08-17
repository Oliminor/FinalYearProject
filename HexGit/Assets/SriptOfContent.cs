#if UNITY_EDITOR
public class SriptOfContent
{
    //(UPDATE) <- This means it requires Update / Change / Rework - it's just bad

    //-------------------
    //---EDITOR SCRIPTS--
    //-------------------

    PathFinder _PathFinder;                         // Pathfinder functions for the PathfinderEditor (Singleton)(UPDATE)
    PathFinderEditor _PathFinderEditor;             // Editor part of the PathFinder script

    PatrolPath _PatrolPath;                         // Creating path for the enemies to patrol in a custom shape
    PatrolPathEditor _PatrolPathEditor;             // Editor part of the PatrolPath script

    PrefabSpawner _PrefabSpawner;                   // Prefab spawner functions to "draw" GameObjects on the meshes or terrain (UPDATE)(JUST REDO WITH DIFFERENT IMPLEMENTATION)
    PrefabSpawnerEditor _PrefabSpawnerEditor;       // Editor part of the PrefabSpawner script

    //-------------------
    //---CUSTOM CLASSES--
    //-------------------

    Attributes _Attributes;                         // Custom class for All attribute data the player has
    PathNode _PathNode;                             // PathFinder class custome Node class
    CheckPoint _CheckPoint;                         // Custom class for the CheckpointManager
    ItemData _ItemData;                             // Custom class for the DataManager
    Inventory _Inventory;                           // Custom class for the DataManager
    StanceGem _StanceGem;                           // Custom class for the DataManager
    SkillTreeData _SkillTreeData;                   // Custom class for the DataManager
    RespawnBaseEnemyData _RespawnBaseEnemyData;     // Custom class for the EnemyManagers
    WeatherSettings _WeatherSettings;               // Custom class for the WeatherManager
    EquippeSkills _EquippeSkills;                   // Custom class for the SkillManager

    //-------------------
    //---SCRIPTABLE OBJ--
    //-------------------

    WeatherSO _WeatherSO;                           // Scriptable object to change weather (Environment ligts setting so far)

    //-------------------
    //---INTERFACES------
    //-------------------

    IDamageTaken _IDamageTaken;                     // Interfaces

    //-------------------
    //---SCRIPTS---------
    //-------------------

    InputManager _InputManager;                     // Input Manager (Singleton)(UPDATE)

    SkinnedMeshEffect _SkinnedMeshEffect;           // VFX effect to emit particles from body (Shadow enemies)

    CameraShake _CameraShake;                       // Camera Shake (Singleton)
    CameraZoom _CameraZoom;                         // Zoom functions for the player camera (Singleton)

    Chest _Chest;                                   // Chest related functions (effects when locked or unlocked)
    Collectable _Collectable;                       // Collactable objects for the collect type of Puzzle
    CollectPuzzle _CollectPuzzle;                   // When Collactable object ^ overlap with this, the puzzle is finished

    EnemyProjectile _EnemyProjectile;               // Enemy base projectile functions

    PlaneGuard _PlaneGuard;                         // Plane Guard boss (UPDATE)(JUST DELETE IT)
    PlaneGuardProjectile _PlaneGuardProjectile;     // Inherits from EnemyProjectile Plane Guard Projectile features

    BaseEnemy _BaseEnemy;                           // Base Enemy for every non-boss type of enemy (UPDATE)
    LandEnemy _LandEnemy;                           // Inherits from the BaseEnemy, land enemy type funtions (UPDATE)
    ArrowShadowEnemy _ArrowShadowEnemy;             // Inherits from the LandEnemy, ranged shadow enemy (UPDATE)
    UnarmedShadowEnemy _UnarmedShadowEnemy;         // Inherits from the LandEnemy, melee shadow enemy (UPDATE)
    ShadowArrowProjectile _ShadowArrowProjectile;   // Inherits from the EnemyProjectile, Projectile tailored for the ArrowShadowEnemy

    DataManager _DataManager;                       // Manages every game data (items so far) Load and Save functions (Singleton)
    BuffManager _BuffManager;                       // Manages every player buffs (Singleton)
    CameraManager _CameraManager;                   // Manages the player cameras (Singleton)
    CheckpointManager _CheckpointManager;           // Manages the checkpoints (save places) (Singleton)
    CooldownManager _CooldownManager;               // Manages the cooldowns (every cooldown has it's own index and time) (Singleton)
    DamageIndicatorManager _DamageIndicatorManager; // "Manages" the damage indicators (projectors) (Singleton)
    EnemyManagers _EnemyManagers;                   // Manages the enemies (store them and respawn them if the player eliminated) (Singleton) (UPDATE)
    WeatherManager _WeatherManager;                 // Manages the weather (Environment lights settings so far) (Singleton)

    ActivateCutscene _ActivateCutscene;             // Activates the cutscenes (all 1)
    AoeDamage _AoeDamage;                           // Depricated but still in use (frogot to change) (UPDATE)
    BillBoardForSprites _BillBoardForSprites;       // Blittboard effect for sprites (UPDATE)
    DamageIndicatorLerp _DamageIndicatorLerp;       // Lerps the damage indcator overtime (projector)
    FireFlickering _FireFlickering;                 // Fire flicker light effect for campfire and tources
    LineRendererMovement _LineRendererMovement;     // Line rendrerer tail movement
    WeatherTrigger _WeatherTrigger;                 // Triggers the weatherlerp from WeatherManager (UPDATE)
    DamageCheck _DamageCheck;                       // Damage collider check for anything (player or enemy)

    MainCharacterStats _MainCharacterStats;         // Manages the player character attributes/stats
    ModularController _ModularController;           // Player Controller functions
    StanceManager _PlayerStanceManager;             // Manages the Player stances
    StashManager _StashManager;                     // Maanges the stash (small inventory behind the character (visually)) for puzzles
    StanceSkillBase _StanceSkillBase;               // Base class of the stances
    MeleeStanceSkill _MeleeStanceSkill;             // inherits from StanceSkillBase, Melee stance functions
    RangedStanceSkill _RangedStanceSkill;           // inherits from StanceSkillBase, Ranged stance functions
    PlayerBaseProjectile _PlayerBaseProjectile;     // player projectile base class
    CheckPointObj _CheckPointObj;                   // Checkpoint object funtions

    DialogueManager _DialogueManager;               // Manages the dialogues (singleton)
    CutsceneDialogue _CutsceneDialogue;             // Cutscene dialogue for Timeline
    NpcDialogue _NpcDialogue;                       // Stores the NPC dialogues (UPDATE)
    BaseIconUI _BaseIconUI;                         // Icon UI elements base functions
    GemIcon _GemIcon;                               // Gem icons functions (UPDATE)
    DisplayItemDetails _DisplayItemDetails;         // Display the item details when the player hover the mouse on any inventory item
    InventoryItems _InventoryItems;                 // Manages the inventory items (UPDATE)
    ItemAquiredManager _ItemAquiredManager;         // Manages the UI elements when the player added any item to the inventory
    SkillManagerUI _SkillManager;                   // Manages the skills (UI)
    StanceManagerUI _StanceManager;                 // Manages the stances (UI)
    AreaTransitionText _AreaTransitionText;         // Text pop up when the player enters different area (Singleton)
    HUDManager _HUDManager;                         // Manages some of the HUD UI elements (singleton)
    DamagePopUp _DamagePopUp;                       // Damage pop up Number
    PopUpTextManager _PopUpTextManager;             // Manages the DamagePopUps (singleton)(UPDATE)
    SideIcon _SideIcon;                             // Manages the stance side icons
    StanceAcquiredPopUp _StanceAcquiredPopUp;       // Customize the stance acquired pop up text
    Transition _Transition;                         // Activates and deactivates the black screen transition
    TutorialPopUp _TutorialPopUp;                   // Tutorial pop up text functions
    BowString _BowString;                           // Bow string visual functions (Linerenderer for the bow)

}
#endif