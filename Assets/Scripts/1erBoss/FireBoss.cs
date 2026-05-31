using System.Collections;
using UnityEngine;

public class FireBoss : MonoBehaviour
{
    [Header("Radial")]
    [SerializeField] private int bulletsAmount = 10;
    [SerializeField] private float startAngle = 90f, endAngle = 270f;

    [Header("Refs")]
    public Transform firePoint;

    [Header("Laser")]
    [SerializeField] private float laserSpeed = 1f;
    [SerializeField] public float lifeTimeLaser = 3f; 

    [Header("ZigZag (placeholder)")]
    public GameObject zigzagPrefab;
    [SerializeField] public float speedZigZag = 5f;
    [SerializeField] public float lifeTimeZigZag = 3f;

    [Header("Anim/Audio")]
    private Animator animator;
    public float animationDuration = 0.30f;
    public AudioSource clipAttack;

    private bool isCooldownActive = false;

    // Cache
    private Transform player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        FindPlayer();
        StartAttackCycle();
    }

    private void FindPlayer()
    {
        var go = GameObject.FindWithTag("Player");
        if (go == null) go = GameObject.Find("Player") ?? GameObject.Find("Player Fast");
        player = go != null ? go.transform : null;
    }

    private void OnDisable()
    {
        CancelInvoke();
        StopAllCoroutines();
        ClearAllProjectiles();
    }

    private void StartAttackCycle()
    {
        InvokeRepeating(nameof(FireBossBullet), 1f, 4.5f);
        InvokeRepeating(nameof(ShootLaser), 4f, 6f);
        InvokeRepeating(nameof(FireCircle), 9f, 10f);

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            CancelInvoke(nameof(FireCircle));
            CancelInvoke(nameof(FireBossBullet));
            CancelInvoke(nameof(ShootLaser));
            isCooldownActive = true;

            ClearAllProjectiles();

            yield return new WaitForSeconds(3f); // cooldown

            isCooldownActive = false;
            StartAttackCycle();
        }
    }

    private void FireCircle()
    {
        if (isCooldownActive) return;

        if (animator) animator.SetTrigger("DisparoRadial");

        float angleStep = (endAngle - startAngle) / (float)bulletsAmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmount + 1; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector2 bulDir = (bulMoveVector - transform.position).normalized;

            var bul = BulletPoolBoss.bulletPoolInstanse.GetBullet();
            if (bul != null)
            {
                bul.transform.SetPositionAndRotation(transform.position, transform.rotation);
                bul.SetActive(true);
                bul.GetComponent<BulletBoss>().SetMoveDirection(bulDir);
            }

            angle += angleStep;
        }
    }

    private void FireBossBullet()
    {
        if (isCooldownActive) return;

        if (player == null) FindPlayer();
        if (player == null) return;

        if (clipAttack) clipAttack.Play();

        var bul = BulletPoolBoss.bulletPoolInstanse.GetBullet();
        if (bul != null)
        {
            bul.transform.position = transform.position;
            bul.transform.rotation = Quaternion.identity;
            bul.SetActive(true);

            Vector2 dir = (player.position - bul.transform.position);
            bul.GetComponent<BulletBoss>().SetMoveDirection(dir);
        }
    }

    private void ShootLaser()
    {
        if (isCooldownActive) return;

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        var laser = LaserPoolBoss.Instance != null ? LaserPoolBoss.Instance.GetLaser() : null;
        if (laser != null)
        {
            laser.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            laser.SetActive(true);

            var lb = laser.GetComponent<LaserBoss>();
            if (lb != null)
            {
                lb.SetMoveDirection(Vector3.down * laserSpeed);
                StartCoroutine(AutoDisable(laser, lifeTimeLaser));
            }
        }

    }

    private IEnumerator AutoDisable(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        if (go != null) go.SetActive(false);
    }


    public void ClearAllProjectiles()
    {
        if (BulletPoolBoss.bulletPoolInstanse != null)
            BulletPoolBoss.bulletPoolInstanse.ClearPool();

        if (LaserPoolBoss.Instance != null)
            LaserPoolBoss.Instance.ClearPool();

    }
}
