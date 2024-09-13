using UnityEngine;
using UnityEngine.UI;

public class EnemyDefend : MonoBehaviour
{
    public Transform target;
    public GameObject enemyProjectile;
    public Slider shootStamina;
    public float shootCooldown;
    readonly float shootTime = 1.5f;
    readonly float speed = 1.8f;
    Transform shootPos;
    Transform player;
    private void Start()
    {
        shootCooldown = shootTime;
        shootPos = transform.Find("ShootPos");
        target = GameObject.FindGameObjectWithTag("Prism").transform;
        player = Manager.Instance.player;
        shootStamina.maxValue = shootTime;
    }
    void Update()
    {
        if (!target) target = GameObject.FindGameObjectWithTag("Prism").transform;
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime), Quaternion.AngleAxis(angle - 180, Vector3.forward));
        if (shootCooldown < 0)
        {
            dir = player.position - transform.position;
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            shootPos.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
            shootCooldown = shootTime;
            Instantiate(enemyProjectile, shootPos.position, shootPos.rotation);
        }
        else
        {
            shootCooldown -= Time.deltaTime;
            shootStamina.value = shootCooldown;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Prism"))
        {
            Manager.Instance.player.GetComponent<Player>().Damage();
            Health _health = GetComponent<Health>();
            _health.TakeDamage(_health.hp,0);
        }
    }
}
