using UnityEngine;
using UnityEngine.UI;

public class EnemyNormal : MonoBehaviour
{
    public GameObject enemyProjectile;
    public Transform target;
    public Slider shootStamina;
    public float shootTime = 0.5f;
    public float shootCooldown;
    public float speed = 1;
    public float escapeRange;
    public float incomingRange;
    readonly string tripleMethod = "TripleShoot";
    Transform shootPos;
    private void Start()
    {
        shootCooldown = shootTime;
        shootPos = transform.Find("ShootPos");
        target = GameObject.FindGameObjectWithTag("Player").transform;
        shootStamina.maxValue = shootTime;
    }
    void Update()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle-180, Vector3.forward);
        float distance = Vector2.Distance(transform.position, target.position);
        if (escapeRange > distance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
            CancelInvoke();
            return;
        }
        else if (incomingRange < distance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            CancelInvoke();
            return;
        }
        if (shootCooldown < 0)
        {
            shootCooldown = shootTime;
            shootStamina.maxValue = shootTime;
            int rand = Random.Range(0, 7);
            switch (rand)
            {
                case 0:
                    shootCooldown += 2f;
                    shootStamina.maxValue = shootCooldown;
                    TripleShoot();
                    break;
                case 5:
                case 4:
                case 6:
                case 1:
                    TripleShoot();
                    break;
                case 2:
                    shootCooldown += 2.5f;
                    shootStamina.maxValue = shootCooldown;
                    Invoke(tripleMethod, 0f);
                    Invoke(tripleMethod, 0.3f);
                    Invoke(tripleMethod, 0.6f);
                    break;
                case 3:
                    shootCooldown += 4f;
                    shootStamina.maxValue = shootCooldown;
                    Invoke(tripleMethod, 0f);
                    Invoke(tripleMethod, 0.3f);
                    Invoke(tripleMethod, 0.6f);
                    Invoke(tripleMethod, 0.9f);
                    Invoke(tripleMethod, 1.2f);
                    Invoke(tripleMethod, 1.8f);
                    Invoke(tripleMethod, 2.2f);
                    break;
            }
            //Instantiate(enemyProjectile, shootPos.position,shootPos.rotation);
        }
        else
        {
            shootCooldown -= Time.deltaTime;
            shootStamina.value = shootCooldown;
        }
    }
    void TripleShoot()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0:
                Instantiate(enemyProjectile, shootPos.position, shootPos.rotation * Quaternion.Euler(0, 0, -15));
                break;
            case 1:
                Instantiate(enemyProjectile, shootPos.position, shootPos.rotation);
                break;
            case 2:
                Instantiate(enemyProjectile, shootPos.position, shootPos.rotation * Quaternion.Euler(0, 0, 15));
                break;
        }
    }
}
