using System.Collections;
using UnityEngine;

public class Boss_2_Ataques : MonoBehaviour
{
    [Header("Pools")]
    public PoolProyectilesBoss2 projectilePool; 
    public PoolMisil_boss2 MisilPool;         

    [Header("Radial Attack")]
    [SerializeField] private int numProjectiles = 12;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float radialInterval = 3f;

    [Header("Missile Attack")]
    [SerializeField] private int missileCount = 5;       
    [SerializeField] private float timeBetweenShots = 0.5f;
    [SerializeField] private float attackHeight = 5f;
    [SerializeField] private float distanceOffScreen = 10f;
    [SerializeField] private float missileInterval = 3f;

    [Header("Warning Sprite")]
    [SerializeField] private GameObject missileWarningSprite;
    [SerializeField] private float warningDuration = 1.5f;

    [Header("Cycle & Cooldown")]
    [SerializeField] private float cycleDuration = 12f;   
    [SerializeField] private float cooldownDuration = 3f; 
    private bool isCooldownActive = false;

    [Header("Audio")]
    public AudioSource clipAttack;     
    public AudioSource clipTentaculos;  

    private void Start()
    {
        if (missileWarningSprite) missileWarningSprite.SetActive(false);
        StartAttackCycle();
    }

    private void OnDisable()
    {
        CancelInvoke();
        StopAllCoroutines();
        projectilePool?.ClearPool();
        if (missileWarningSprite) missileWarningSprite.SetActive(false);
    }

    private void StartAttackCycle()
    {
        InvokeRepeating(nameof(RadialAttack), 1f, radialInterval);
        InvokeRepeating(nameof(LaunchMissiles), 0f, missileInterval);
        Invoke(nameof(StartCooldownCycle), cycleDuration);
    }

    private void StartCooldownCycle()
    {
        CancelInvoke(nameof(RadialAttack));
        CancelInvoke(nameof(LaunchMissiles));
        isCooldownActive = true;

        projectilePool?.ClearPool();

        Invoke(nameof(EndCooldown), cooldownDuration);
    }

    private void EndCooldown()
    {
        isCooldownActive = false;
        StartAttackCycle();
    }

    private void LaunchMissiles()
    {
        if (isCooldownActive || MisilPool == null) return;
        StartCoroutine(LaunchMissileWithWarning());
    }

    private IEnumerator LaunchMissileWithWarning()
    {
        if (missileWarningSprite) missileWarningSprite.SetActive(true);
        yield return new WaitForSeconds(warningDuration);
        if (missileWarningSprite) missileWarningSprite.SetActive(false);

        for (int i = 0; i < missileCount; i++)
        {
            LaunchSingleMissile(i);
            if (timeBetweenShots > 0f)
                yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void LaunchSingleMissile(int index)
    {
        var missile = MisilPool.GetProjectile();
        if (missile == null) return;

        var comp = missile.GetComponent<Misil_boss_2>();
        if (comp == null)
        {
            missile.SetActive(false);
            return;
        }

        comp.SetPool(MisilPool);

        float side = (index % 2 == 0) ? -1f : 1f; 
        float y = Random.Range(-attackHeight, 2f);
        missile.transform.SetPositionAndRotation(new Vector3(side * distanceOffScreen, y, 0f), Quaternion.identity);
        missile.SetActive(true);
        comp.SetHorizontalMovement(side);

        if (clipTentaculos) clipTentaculos.Play();
    }

    private void RadialAttack()
    {
        if (isCooldownActive || projectilePool == null) return;

        if (clipAttack) clipAttack.Play(); 

        float angleStep = 360f / numProjectiles;
        float angle = 0f;
        Vector3 origin = transform.position;

        for (int i = 0; i < numProjectiles; i++)
        {
            var proj = projectilePool.GetProjectile();
            if (proj == null) continue;

            proj.transform.position = origin;

            var comp = proj.GetComponent<Boss_2_Proyectile>();
            if (comp != null)
            {
                float dirX = Mathf.Sin(angle * Mathf.Deg2Rad);
                float dirY = Mathf.Cos(angle * Mathf.Deg2Rad);
                Vector3 direction = new Vector3(dirX, dirY, 0f);

                comp.speed = projectileSpeed;

                comp.SetDirection(direction, projectilePool);
            }
            else
            {
                proj.SetActive(false);
            }

            angle += angleStep;
        }
    }
}
