using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Overall")]
    [SerializeField] AudioClip[] hitSound;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] LayerMask bulletMask;
    public int damage = 10;
    public float reach = 1;
    public float hitTime = 1;
    TextMeshProUGUI stats;
    float[] laserStats = { 10, 25, 0.1f, 0 };
    float[] swordStats = { 60, 1f, 0.25f, 0.8f };
    float stunTime = 0.4f;
    float hitCooldown = 1;
    AudioSource audioSound;
    Vector3 dir;

    Transform sword;
    Transform swordPos;
    readonly Vector3 swordScale = Vector3.one - Vector3.right * 0.8f;
    Vector3 swordDirection;
    public float swordSpeed;
    public ParticleSystem swordParticle;

    public bool laser = true;
    public LineRenderer line;
    public Transform laserParticle;
    private void Start()
    {
        laser = MenuManager.laser;
        stats = GameObject.FindGameObjectWithTag("Stats").GetComponent<TextMeshProUGUI>();
        if (laser)
            ResetWeaponStatistics();
        else
        {
            swordPos = transform.Find("SwordPos");
            sword = transform.Find("Sword");
            sword.gameObject.SetActive(true);
            sword.parent = null;
            ResetWeaponStatistics();
        }
        WellStats();
        audioSound = Camera.main.GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!laser) 
            swordDirection = transform.position + dir.normalized * (reach);
        if (dir.magnitude != 0 && hitCooldown < 0)
        {
            hitCooldown = hitTime;

            RaycastHit2D[] rangeEnts = null;
            RaycastHit2D[] rangeBullets = null;
            if (laser)
            {
                line.gameObject.SetActive(true);
                hitCooldown -= Time.deltaTime;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, transform.position + dir * reach);
                laserParticle.position = transform.position + dir * (reach/2);
                laserParticle.rotation = Quaternion.Euler(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90, 90, -90);
                rangeEnts = Physics2D.RaycastAll(transform.position, dir, reach, enemyMask);
            } else
            {
                rangeEnts = Physics2D.CircleCastAll(swordDirection, reach, dir, reach, enemyMask);
                rangeBullets = Physics2D.CircleCastAll(swordDirection, reach, dir, reach, bulletMask);
                sword.position = swordDirection;
                sword.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90);
                sword.localScale = swordScale * reach;
                Instantiate(swordParticle,sword.position,sword.rotation);
            }

            bool _hit = false;
            foreach (RaycastHit2D x in rangeEnts)
            {
                x.collider.GetComponent<Health>().TakeDamage(damage, stunTime);
                _hit = true;
            }
            if (!laser)
            {
                foreach (RaycastHit2D x in rangeBullets)
                {
                    x.collider.GetComponent<Bullet>().speed /= 8;
                }
            }
            if (audioSound.isVirtual && _hit)
            {
                audioSound.clip = hitSound[Random.Range(0, hitSound.Length)];
                audioSound.Play();
            }
        }
        else
        {
            if (laser)
            {
                line.SetPosition(0, transform.position);
                line.SetPosition(1, transform.position + dir.normalized * reach);
                laserParticle.position = transform.position + dir * (reach / 2);
                laserParticle.rotation = Quaternion.Euler(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90, 90, -90);
                if (hitCooldown < 0)
                {
                    line.gameObject.SetActive(false);
                }
            }
            else
            {
                if (dir.magnitude != 0) { 
                    sword.position = swordDirection;
                    sword.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg-90);
                    sword.localScale = swordScale * reach;
                } else { 
                    sword.position = Vector3.Lerp(sword.position, swordPos.position, swordSpeed * Time.deltaTime);
                    sword.rotation = Quaternion.identity;
                    sword.localScale = swordScale;
                }
            }
            hitCooldown -= Time.deltaTime;
        }
    }
    public void ResetWeaponStatistics()
    {
        int _damage = damage;
        float _hitTime = hitTime;
        float _reach = reach;
        if (laser)
        {
            damage = (int)laserStats[0];
            reach = laserStats[1];
            hitTime = laserStats[2];
            stunTime = laserStats[3];
        } else
        {
            damage = (int)swordStats[0];
            reach = swordStats[1];
            hitTime = swordStats[2];
            stunTime = swordStats[3];
        }
        stats.text = "Firerate: " + _hitTime + " > " + hitTime + "\nDamage: " + _damage + " > " + damage + "\nReach: " + _reach + " > " + reach;
        Invoke("WellStats", 2f);
        Debug.Log("hitTime: " + _hitTime + " > " + hitTime + " | damage: " + _damage + " > " + damage + " | reach: " + _reach + " > " + reach);
    }
    public void ChangeStats(int _id,float _stat)
    {
        if (_id == 0)
            stats.text = "Firerate: " + hitTime + "\nDamage: " + damage + " > " + _stat + "\nReach: " + reach;
        else if (_id == 1)
            stats.text = "Firerate: " + hitTime + " > " + _stat + "\nDamage: " + damage + "\nReach: " + reach;
        else if (_id == 2)
            stats.text = "Firerate: " + hitTime + "\nDamage: " + damage + "\nReach: " + reach + " > " + _stat;
        Invoke("WellStats", 2f);
    }
    public void WellStats()
    {
        stats.text = "Firerate: " + hitTime + "\nDamage: " + damage + "\nReach: " + reach;
    }
    public void GetInput(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (laser) Gizmos.DrawRay(transform.position, dir.normalized*reach);
        else Gizmos.DrawWireSphere(swordDirection, reach);
    }
}
