using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float rotateSpeed = 160f;
    [SerializeField] private float lifetime = 5f;

    private Transform target;
    private ProjectileShoot projectileShoot;

    private static readonly string[] enemyTags =
    {
        "EnemyPhasatron", "EnemyWall", "EnemyKamikaze", "CrazyKamikaze",
        "EnemyShip", "1stBoss", "2ndBoss", "3rdBoss", "Meteorito"
    };

    private void Start()
    {
        Destroy(gameObject, lifetime);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) projectileShoot = player.GetComponent<ProjectileShoot>();

        FindNearestEnemy();
    }

    private void Update()
    {
        if (target == null) FindNearestEnemy();

        if (target != null)
        {
            Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void FindNearestEnemy()
    {
        float closestSqrDist = Mathf.Infinity;
        target = null;

        foreach (string tag in enemyTags)
        {
            try
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
                foreach (var e in enemies)
                {
                    if (e == null) continue;
                    float sqrDist = ((Vector2)e.transform.position - (Vector2)transform.position).sqrMagnitude;
                    if (sqrDist < closestSqrDist)
                    {
                        closestSqrDist = sqrDist;
                        target = e.transform;
                    }
                }
            }
            catch { /* tag no registrado en esta escena */ }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool hit = false;

        if (collision.CompareTag("EnemyPhasatron"))
        {
            var e = collision.GetComponent<Phasatron>();
            var e2 = collision.GetComponent<Phasatron2>();
            var e3 = collision.GetComponent<Phasatron3>();
            if (e != null)  { e.TakeDamage(e.maxHealth * 0.21f);   hit = true; }
            if (e2 != null) { e2.TakeDamage(e2.maxHealth * 0.16f); hit = true; }
            if (e3 != null) { e3.TakeDamage(e3.maxHealth * 0.13f); hit = true; }
        }
        else if (collision.CompareTag("EnemyWall"))
        {
            var e = collision.GetComponent<EnemyWall>();
            var e2 = collision.GetComponent<EnemyWall2>();
            if (e != null)  { e.TakeDamage(e.maxHealth * 0.40f);   hit = true; }
            if (e2 != null) { e2.TakeDamage(e2.maxHealth * 0.35f); hit = true; }
        }
        else if (collision.CompareTag("EnemyKamikaze"))
        {
            var e = collision.GetComponent<Kamikaze>();
            var e2 = collision.GetComponent<Kamikaze2>();
            if (e != null)  { e.TakeDamage(60f);  hit = true; }
            if (e2 != null) { e2.TakeDamage(55f); hit = true; }
        }
        else if (collision.CompareTag("CrazyKamikaze"))
        {
            var e = collision.GetComponent<Kamikaze2>();
            if (e != null) { e.TakeDamage(100f); hit = true; }
        }
        else if (collision.CompareTag("EnemyShip"))
        {
            var e = collision.GetComponent<EnemyFlyThrough>();
            if (e != null) { e.TakeDamage(100f); hit = true; }
        }
        else if (collision.CompareTag("1stBoss"))
        {
            var e = collision.GetComponent<BossMovement>();
            if (e != null) { e.TakeDamage(e.maxHealth * 0.04f); hit = true; }
        }
        else if (collision.CompareTag("2ndBoss"))
        {
            var e = collision.GetComponent<Boss_2_movement>();
            if (e != null) { e.TakeDamage(e.maxHealth * 0.017f); hit = true; }
        }
        else if (collision.CompareTag("3rdBoss"))
        {
            var e = collision.GetComponent<Boss3>();
            if (e != null) { e.TakeDamage(e.maxHealth * 0.008f); hit = true; }
        }
        else if (collision.CompareTag("Meteorito"))
        {
            var e = collision.GetComponent<Meteorito>();
            if (e != null) { e.TakeDamage(e.life * 0.34f); hit = true; }
        }
        else if (collision.CompareTag("Boundary"))
        {
            hit = true;
        }

        if (hit)
        {
            if (projectileShoot != null) projectileShoot.IncreaseCharge();
            Destroy(gameObject);
        }
    }
}
