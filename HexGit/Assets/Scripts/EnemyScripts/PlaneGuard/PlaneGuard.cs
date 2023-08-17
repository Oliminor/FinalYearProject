using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Animations.Rigging;

public class PlaneGuard : MonoBehaviour, IDamageTaken
{
    private enum PlaneGuardAttackPattern { NormalArrow = 0, MultiShot = 1, ArrowRain = 2 }

    private PlaneGuardAttackPattern attackPattern;

    [SerializeField] private List<GameObject> activateGameObjectsWhenDie;
    [SerializeField] private Rig bossRig;

    [SerializeField] ActivateCutscene scene;
    [SerializeField] private Transform bossAreaBorder;

    [SerializeField] private LayerMask whatisGround;
    [SerializeField] private LayerMask target;
    [SerializeField] private Transform characterModel;
    [SerializeField] private Transform ElectroProjectile;
    [SerializeField] private Transform ProjectileElectroCircleEffect;
    [SerializeField] private Transform ProjectileNormalLandEffect;
    [SerializeField] private Transform ultimateElectroEffect;
    [SerializeField] private Transform teleportEffect;
    [SerializeField] private Transform lightningEffect;
    [SerializeField] private Transform melee360AttackEffect;
    [SerializeField] private Transform fakeProjectile;
    [SerializeField] private Transform bow;

    [SerializeField] private Transform aimObject;

    [SerializeField] private int maxHealth;
    [SerializeField] private float timeBetweenAttackpatterns;

    [SerializeField] private int ultimateDamage;
    [SerializeField] private int arrowNormalDamage;
    [SerializeField] private float normalCooldown;
    [SerializeField] private int arrowRainDamage;
    [SerializeField] private float arrowRainCooldown;
    [SerializeField] private float arrowRainHitRate;
    [SerializeField] private int arrowMultiShotDamage;
    [SerializeField] private float arrowMultiShotCooldown;
    [SerializeField] private float teleportCooldown;
    [SerializeField] private int melee360AttackDamage;

    private ModularController player;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Animator anim;

    private Vector3 middlePosition;

    private int health;
    private bool ultimaetAttack;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        anim.SetBool("Landed", false);

        gameObject.SetActive(false);

        foreach (var item in activateGameObjectsWhenDie) item.SetActive(false);

        if (GameManager.instance.HUDManager.isActiveAndEnabled) GameManager.instance.HUDManager.UpdateBossHealth("Plane Guard", maxHealth, health);
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0) SmoothTurning(player.GetPlayerDamagePoint(), 0.1f);

        AimTarget();
    }

    private void Initialize()
    {
        health = maxHealth;
        bossRig.weight = 0;

        if (GameManager.instance.HUDManager.isActiveAndEnabled) GameManager.instance.HUDManager.UpdateBossHealth("Plane Guard", maxHealth, health);

        player =  GameManager.instance.ModularController;
        skinnedMeshRenderer = characterModel.GetComponent<SkinnedMeshRenderer>();
        middlePosition = transform.position;
        anim = GetComponent<Animator>();
        if (bow.TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(1);

        MainCharacterStats.onGameOver += RespawnBoss;
    }

    /// <summary>
    /// Respawns boss in case the player dies first
    /// </summary>
    private void RespawnBoss()
    {
        bossRig.weight = 0;
        anim.SetBool("Landed", false);

        GameManager.instance.HUDManager.ToggleBossHealthbar(false);

        scene.SetIsActivated(false);
        health = maxHealth;
        bossAreaBorder.gameObject.SetActive(false);
        transform.position = middlePosition;
        if (TryGetComponent(out WeatherTrigger _weatherTrigger)) _weatherTrigger.TriggerThisWeather();

        if (GameManager.instance.HUDManager.isActiveAndEnabled) GameManager.instance.HUDManager.UpdateBossHealth("Plane Guard", maxHealth, health);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Start the first Phase
    /// </summary>
    public void StartPhase()
    {
        bossRig.weight = 1; // Switch on animation rigging 
        GameManager.instance.HUDManager.ToggleBossHealthbar(true); // Switch on health bar
        anim.SetTrigger("ShootArrow"); 
    }

    private void RandomPattern()
    {
        float distance = Vector3.Distance(player.GetPlayerDamagePoint(), transform.position);
        if (distance < 8)
        {
            Trigger360Attack();
            return;
        }

        StartCoroutine(RigWeight(1));
        int randomAttack = Random.Range(0, 3);

        attackPattern = (PlaneGuardAttackPattern)randomAttack;
    }

    public void AimTarget()
    {
        switch (attackPattern)
        {
            case PlaneGuardAttackPattern.NormalArrow:
                ChangeAimTarget(player.GetPlayerDamagePoint());
                break;
            case PlaneGuardAttackPattern.MultiShot:
                ChangeAimTarget(player.GetPlayerDamagePoint());
                break;
            case PlaneGuardAttackPattern.ArrowRain:
                ChangeAimTarget(player.transform.position + new Vector3(0, 100, 0));
                break;
        }
    }

    /// <summary>
    /// Triggers the attack cooldown for different attack pattern
    /// </summary>
    public void TriggerCooldown()
    {
        switch (attackPattern)
        {
            case PlaneGuardAttackPattern.NormalArrow:
                StartCoroutine(Cooldown(normalCooldown));
                ChangeAimTarget(player.GetPlayerDamagePoint());
                break;
            case PlaneGuardAttackPattern.MultiShot:
                StartCoroutine(Cooldown(arrowMultiShotCooldown));
                ChangeAimTarget(player.GetPlayerDamagePoint());
                break;
            case PlaneGuardAttackPattern.ArrowRain:
                StartCoroutine(Cooldown(arrowRainCooldown));
                ChangeAimTarget(player.transform.position + new Vector3(0, 100, 0));
                break;
        }
    }

    /// <summary>
    /// Instantiate the right projectile (based on the attack pattern)
    /// </summary>
    private void InstantiateProjectile()
    {
        switch (attackPattern)
        {
            case PlaneGuardAttackPattern.NormalArrow:
                InstantiateNormalProjectile();
                break;
            case PlaneGuardAttackPattern.MultiShot:
                InstantiateMultiShot();
                break;
            case PlaneGuardAttackPattern.ArrowRain:
                InstantiateArrowRain();
                break;
        }
    }

    /// <summary>
    /// Checks if the boss landed to the ground or not
    /// </summary>
    private bool IsCharacterLanded()
    {
        if (Vector3.Distance(new Vector3(transform.position.x, GroundLevel(transform.position), transform.position.z), transform.position) < 4) return true;

        return false;
    }

    private void TriggerUltimatePlungeAttack()
    {
        bossRig.weight = 0;

        anim.SetTrigger("Falling");

        StartCoroutine(Teleport());

        if (fakeProjectile.transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(0);
        if (fakeProjectile.transform.GetChild(1).TryGetComponent(out ParticleSystem _particle)) _particle.Stop();
        if (bow.TryGetComponent(out BowString _string)) _string.SetDrawMode(false);

        Vector3 startPos = middlePosition + transform.up * 500;
        Vector3 endPos = middlePosition;

        transform.position = startPos;

        StartCoroutine(FallingLerp(2.0f, startPos, endPos));
        StartCoroutine(Landing());
    }

    // Lerp the boss between two position (when falling down from the sky)
    IEnumerator FallingLerp(float _lerpTime, Vector3 startPosition, Vector3 endPositon)
    {
        float radius = 50;

        GameObject go = GameManager.instance.DamageIndicatorManager.AddDamageIndicator(transform, new Vector3(90, 0, 0), new Vector2(radius * 2, radius * 2), 1000);
        go.GetComponent<DamageIndicatorLerp>().TriggerDamageIndicator();

        float lerp = 0;

        while(lerp < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            lerp += Time.deltaTime / _lerpTime;
            transform.position = Vector3.Lerp(startPosition, endPositon, lerp);
        }

        Destroy(go);
    }

    // Switch on effects after the character landed to the ground
    IEnumerator Landing()
    {
        yield return new WaitForSeconds(0.1f);

        while (!IsCharacterLanded())
        {
            yield return null;
        }

        anim.SetBool("Landed", true);

        StartCoroutine(SelfElectroTime(5));

        SetElectroTransition(1);

        StartCoroutine(UltimateAttack());

        GameManager.instance.CameraManager.CameraShake.TriggerCameraShake(3, 0.5f);

        bossRig.weight = 0;
    }

    // Ultimate attack (Light rings) When the boss hits the ground
    IEnumerator UltimateAttack()
    {
        int ringNumber = 1;

        while (ringNumber <= 5)
        {
            GameObject go = Instantiate(ultimateElectroEffect.gameObject, transform.position, Quaternion.identity);

            float radius = ringNumber * 10;

            ParticleSystem.ShapeModule ps = go.GetComponent<ParticleSystem>().shape;
            ps.radius = radius;

            ParticleSystem.EmissionModule emission = go.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = ringNumber * 400;

            GameManager.instance.DamageCheck.EnemySingleDamage(transform.position, radius, ultimateDamage, target);

            Destroy(go, 1);
            yield return new WaitForSeconds(0.2f);
            ringNumber++;
        }
    }


    /// <summary>
    /// Changes the aim target (animation riggint) where the boss visually aiom with her hands
    /// </summary>
    private void ChangeAimTarget(Vector3 target)
    {
        if (target == aimObject.transform.position) return;
        aimObject.transform.position = target;
    }

    // Cooldown between attacks
    IEnumerator Cooldown(float _time)
    {
        anim.SetBool("isOnCooldown", true);
        yield return new WaitForSeconds(_time);
        anim.SetBool("isOnCooldown", false);
    }

    /// <summary>
    /// Draw the arrow visual effect
    /// </summary>
    public void DrawArrow()
    {
        RandomPattern();

        if (bow.TryGetComponent(out BowString _string)) _string.SetDrawMode(true);
        if (fakeProjectile.transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveOn();
        if (fakeProjectile.transform.GetChild(1).TryGetComponent(out ParticleSystem _particle)) _particle.Play();
        TriggerCooldown();
    }

    /// <summary>
    /// Release arrow visual effects + instantiate the projectile
    /// </summary>
    public void RelesaseArrow()
    {
        if (bow.TryGetComponent(out BowString _string)) _string.SetDrawMode(false);
        if (fakeProjectile.transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(0);
        if (fakeProjectile.transform.GetChild(1).TryGetComponent(out ParticleSystem _particle)) _particle.Stop();
        InstantiateProjectile();
    }

    private Vector3 PreCalculatePlayerPosition(float _projectileSpeed)
    {
        Vector3 playerV = player.GetPlayerRigidbody().velocity;
        Vector3 playerPosition = player.GetPlayerDamagePoint();
        float timeToReachEnemy = Vector3.Distance(playerPosition, fakeProjectile.position) / _projectileSpeed;
        Vector3 targetPosition = playerPosition + playerV * timeToReachEnemy;

        Vector3 projectileDirection = (targetPosition - fakeProjectile.position).normalized;

        return projectileDirection;
    }

    /// <summary>
    /// Single attack projectile instantiate
    /// </summary>
    private void InstantiateNormalProjectile()
    {
        Quaternion rotation = Quaternion.LookRotation(PreCalculatePlayerPosition(100));
        GameObject go = Instantiate(ElectroProjectile.gameObject, fakeProjectile.position, rotation);
        go.GetComponent<PlaneGuardProjectile>().SetArrowValues(true, 100, arrowNormalDamage, 0, target, null, ProjectileNormalLandEffect, 15);

        if (go.transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(1);
    }

    /// <summary>
    /// Multi shot attack projectile instantiate
    /// </summary>
    private void InstantiateMultiShot()
    {
        for (int i = -3; i <= 3; i++)
        {
            GameObject go = Instantiate(ElectroProjectile.gameObject, fakeProjectile.position, Quaternion.Euler(fakeProjectile.rotation.eulerAngles + new Vector3(0, i*13, 0)));
            go.GetComponent<EnemyProjectile>().SetArrowValues(true, 80, arrowMultiShotDamage, 0, target, null, ProjectileNormalLandEffect, 15);

            if (go.transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(1);
            go.GetComponent<PlaneGuardProjectile>().TurnOnHomingMissle(0.075f);
        }
    }

    /// <summary>
    /// Arrow rain projectile instantiate
    /// </summary>
    private void InstantiateArrowRain()
    {
        GameObject go = Instantiate(ElectroProjectile.gameObject, fakeProjectile.position, fakeProjectile.rotation);
        go.GetComponent<PlaneGuardProjectile>().SetArrowValues(true, 120, arrowNormalDamage, 10.0f, target, null, ProjectileNormalLandEffect, 2);
        go.GetComponent<PlaneGuardProjectile>().SetDissolve(true);
        go.transform.localScale *= 2;

        if (go.transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(1);

        StartCoroutine(ArrowRain(1));
    }

    private void Trigger360Attack()
    {
        StartCoroutine(RigWeight(0));
        if (bow.TryGetComponent(out BowString _string)) _string.SetDrawMode(false);
        FaceToPlayer();
        anim.SetTrigger("360");
    }

    private void Attack360Damage()
    {
        GameManager.instance.DamageCheck.EnemySingleDamage(bow.position, 5, melee360AttackDamage, target);
        GameObject go = Instantiate(melee360AttackEffect.gameObject, bow.position, Quaternion.identity);
        Destroy(go, 2);
        StartCoroutine(SelfElectroTime(3.0f));
        StartCoroutine(TeleportEffectLerp());
        StartCoroutine(Teleport());
    }

    private IEnumerator RigWeight(float _weight)
    {
        yield return new WaitForEndOfFrame();
        bossRig.weight = _weight;
    }

    // Arrow rain Coroutine 
    IEnumerator ArrowRain(float _time)
    {
        float timer = 0;

        while (timer < _time)
        {
            yield return new WaitForSeconds(arrowRainHitRate);

            if (attackPattern != PlaneGuardAttackPattern.ArrowRain) yield break;

            Vector3 position = player.transform.position;
            InstantiateArrowRainProjectile(position);
            timer += arrowRainHitRate;
        }
    }


    // Teleport effect (create a clone from herself and left it behind for a split second)
    IEnumerator Teleport()
    {
        if (health > 1)
        {
            int cooldownIndex = 3432;

            if (GameManager.instance.CooldownManager.IsCooldownFinished(cooldownIndex)) GameManager.instance.CooldownManager.AddCooldown(cooldownIndex, teleportCooldown);
            else yield break;
        }

        Mesh _mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(_mesh); // Create the new mesh

        GameObject go = new GameObject("TeleportClone");
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.AddComponent<SkinnedMeshRenderer>().sharedMesh = _mesh; // add SkinnedMeshRenrerer component to the clone

        int length = skinnedMeshRenderer.materials.Length;

        Material mat = new Material(skinnedMeshRenderer.material);
        Material[] mats = new Material[length];

        for (int i = 0; i < length; i++)
        {
            mats[i] = mat;
        }

        go.GetComponent<SkinnedMeshRenderer>().sharedMaterials = mats; // set the clone material

        for (int i = 0; i < length; i++)
        {
            go.GetComponent<SkinnedMeshRenderer>().sharedMaterials[i].SetFloat("_Transition", 1);
        }

        transform.position = RandomTeleportPoint();
        FaceToPlayer();

        StartCoroutine(TeleportEffectLerp());

        yield return new WaitForSeconds(0.2f);

        GameObject effect = Instantiate(teleportEffect.gameObject, go.transform.position, go.transform.rotation); // add the teleport electro effect to the clone

        Destroy(go);

        // Changes the particle system mesh object to the clone mesh object for more accurate particle effect
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule sh = ps.shape;
        sh.enabled = true;
        sh.shapeType = ParticleSystemShapeType.Mesh;
        sh.mesh = _mesh;

        Destroy(effect.gameObject, 1);
    }

   /// <summary>
   /// Sets the electro transition effect to 1 (visible)
   /// </summary>
    private void SetElectroTransition(float _transition)
    {
        int length = skinnedMeshRenderer.materials.Length;

        for (int i = 0; i < length; i++)
        {
            transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().sharedMaterials[i].SetFloat("_Transition", _transition);
        }
    }

    // Electro effect time
    IEnumerator SelfElectroTime(float time)
    {
        float timer = 0;
        float timeStep = 0.5f;

        while (timer < time)
        {
            SelfElectroEffect();
            yield return new WaitForSeconds(timeStep);
            timer += timeStep;
        }
    }

    private void SelfElectroEffect()
    {
        Mesh _mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(_mesh);

        GameObject effect = Instantiate(teleportEffect.gameObject, transform.position, transform.rotation);

        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule sh = ps.shape;
        sh.enabled = true;
        sh.shapeType = ParticleSystemShapeType.Mesh;
        sh.mesh = _mesh;

        Destroy(effect.gameObject, 1);
    }

    /// <summary>
    /// Starts the teleprot effect lerp
    /// </summary>
    public void TriggerTeleportEffectLerp()
    {
        StartCoroutine(TeleportEffectLerp());
    }

    // Slowly turns of the teleport effect (fade out)
    IEnumerator TeleportEffectLerp()
    {
        float lerp = 0;
        int length = skinnedMeshRenderer.materials.Length;

        while (lerp < 1)
        {
            yield return new WaitForSeconds(0.01f);
            lerp += 0.01f;

            for (int i = 0; i < length; i++)
            {
                skinnedMeshRenderer.sharedMaterials[i].SetFloat("_Transition", Mathf.Lerp(1, 0, lerp));
            }
        }

        StartCoroutine(SelfElectroTime(1.5f));
    }

    /// <summary>
    /// Instantiates the rain arrow projectile (random position)
    /// </summary>
    private void InstantiateArrowRainProjectile(Vector3 _position)
    {
        Vector2 randomPos = Random.insideUnitCircle * 60;
        Vector3 Position = new Vector3(_position.x + randomPos.x, _position.y + 100, _position.z + randomPos.y);

        GameObject go = Instantiate(ElectroProjectile.gameObject, Position, Quaternion.Euler(new Vector3(90, 0 ,0)));
        go.GetComponent<EnemyProjectile>().SetArrowValues(true, 120, arrowRainDamage, 10.0f, target, null, ProjectileElectroCircleEffect, 15);
        go.GetComponent<PlaneGuardProjectile>().AddDamageIndicator();
        go.transform.localScale *= 3;
    }

    /// <summary>
    /// Get a new random position to telepor to
    /// </summary>
    private Vector3 RandomTeleportPoint()
    {
        Vector3 teleportPos = player.transform.position;

        while (DistanceFromPlayer(teleportPos) < 10)
        {
            Vector2 randomPos = Random.insideUnitCircle * 70;
            teleportPos = CalculatePositionRelativeToMiddle(randomPos);
        }

        return teleportPos;
    }

    /// <summary>
    /// Calculates the random position relative to the boss position offset
    /// </summary>
    private Vector3 CalculatePositionRelativeToMiddle(Vector2 _XZPosition)
    {
        Vector2 Pos = new Vector2(middlePosition.x + _XZPosition.x, middlePosition.z + _XZPosition.y);
        return new Vector3(Pos.x, GroundLevel(new Vector3(Pos.x, transform.position.y, Pos.y)), Pos.y);
    }

    /// <summary>
    /// Returns the ground position from a certain point
    /// </summary>
    private float GroundLevel(Vector3 _checkPos)
    {
        float positionY = 0;

        Vector3 raycastPoint = _checkPos + Vector3.up * 50;
        float raySize = Mathf.Infinity;

        Vector3 direction = Vector3.down;

        if (Physics.Raycast(raycastPoint, direction, out RaycastHit hitinfo, raySize, whatisGround))
        {
            positionY = hitinfo.point.y;
        }

        return positionY;
    }

    /// <summary>
    /// Distance between the boss and the player
    /// </summary>
    private float DistanceFromPlayer(Vector3 _pos)
    {
        return Vector3.Distance(_pos, player.transform.position);
    }

    /// <summary>
    /// Boss rotates towards the player
    /// </summary>
    private void FaceToPlayer()
    {
        Quaternion lookRotation = Quaternion.LookRotation((player.transform.position - transform.position).normalized);
        Vector3 angle = lookRotation.eulerAngles;
        angle = new Vector3(0, angle.y, 0);
        transform.rotation = Quaternion.Euler(angle);
    }

    /// <summary>
    /// Smooth turn when the player moves around the boss
    /// </summary>
    private void SmoothTurning(Vector3 _targetPosition, float turnSpeed)
    {
        Quaternion lookRotation = Quaternion.LookRotation((_targetPosition - transform.position).normalized);
        Vector3 angle = lookRotation.eulerAngles;
        angle = new Vector3(0, angle.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(angle), turnSpeed);
    }

    /// <summary>
    /// Death call when the boss dies (from animator)
    /// </summary>
    private void Death()
    {
        if (fakeProjectile.transform.GetChild(0).TryGetComponent(out DissolveEffectManager _dissolveArrow)) _dissolveArrow.DissolveRate(0); // switches off the projectile effect on the boss hand
        if (fakeProjectile.transform.GetChild(1).TryGetComponent(out ParticleSystem _particle)) _particle.Stop(); // switches off the projectile particle effect
        if (bow.TryGetComponent(out DissolveEffectManager _dissolveBow)) _dissolveBow.DissolveRate(0); // switches off the bow shader effect
        if (bow.TryGetComponent(out BowString _string)) _string.SetDrawMode(false); // sets the draw mode off (the bow string)
        if (TryGetComponent(out WeatherTrigger _weatherTrigger)) _weatherTrigger.TriggerThisWeather(); // Triggers the new weather (environment light settings)
        GameManager.instance.HUDManager.ToggleBossHealthbar(false);

        foreach (var item in activateGameObjectsWhenDie) item.SetActive(true);

        bossAreaBorder.GetChild(0).GetComponent<ParticleSystem>().Stop();
        bossAreaBorder.GetComponent<MeshCollider>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        Destroy(bossAreaBorder.gameObject, 10);
        MainCharacterStats.onGameOver -= RespawnBoss;
        Destroy(transform.root.gameObject);
    }

    /// <summary>
    /// Calls the lightning effect from the animator after the enemy dies
    /// </summary>
    private void LightningEffect()
    {
        GameObject go = Instantiate(lightningEffect.gameObject, transform.position, Quaternion.identity);
        go.transform.position += Vector3.up * 50;

        Destroy(go, 1);
    }

    // Damage taken Interface called when the player deals damage
    public void DamageTaken(int _damageNumber)
    {
        health -= _damageNumber;
        if (GameManager.instance.HUDManager.isActiveAndEnabled) GameManager.instance.HUDManager.UpdateBossHealth("Plane Guard", maxHealth, health);

        if (health < 2 && !ultimaetAttack)
        {
            ultimaetAttack = true;
            health = 1;
            TriggerUltimatePlungeAttack();
        }

        if (health < 0) anim.SetTrigger("Death");
        else StartCoroutine(Teleport());
    }

    // The damage is nullified when the enemy is vulnarable 
    public int DamageCalculation(int _damageReturn)
    {
        return _damageReturn;
    }
}
