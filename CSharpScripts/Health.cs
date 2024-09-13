using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public ParticleSystem destroyParticle;
    public AudioClip destroySound;
    public int hp = 100;
    public bool fixedPosition = true;
    Slider healthSlider;
    AudioSource audioSound;
    //[SerializeField] float knockback = 1;
    //Rigidbody2D rb;
    private void Start()
    {
        audioSound = Camera.main.GetComponent<AudioSource>();
        healthSlider = null;
        if (fixedPosition)
            healthSlider = transform.Find("Canvas/Health Slider").GetComponent<Slider>();
        else
        {
            healthSlider = GetComponent<Boss>().bossBar;
            healthSlider.gameObject.SetActive(true);
        }
        healthSlider.maxValue = hp;
        healthSlider.value = hp;
        //rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (fixedPosition) healthSlider.transform.SetPositionAndRotation(transform.position+Vector3.up*0.8f,Quaternion.identity);
    }
    public void TakeDamage(int dmg,float stun)
    {
        hp -= dmg;
        healthSlider.value = hp;
        if (GetComponent<EnemyNormal>()) GetComponent<EnemyNormal>().shootCooldown += stun;
        if (GetComponent<EnemyDefend>()) GetComponent<EnemyDefend>().shootCooldown += stun;
        //rb.velocity = transform.right * knockback;
        if (hp <= 0)
        {
            audioSound.clip = destroySound;
            audioSound.Play();
            Instantiate(destroyParticle,transform.position,Quaternion.identity);
            if (!fixedPosition)
            {
                Debug.Log("BossDead()");
                healthSlider.gameObject.SetActive(false);
                Manager.Instance.BossDead();
            }
            Manager.Instance.EnemyDead();
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            int _damage = collision.collider.GetComponent<Weapon>().damage + 100;
            Debug.Log("Collision damage: " + _damage + " | Health Points: " + hp + " | Player Damage: " + (hp > _damage));
            if (hp > _damage)
            {
                collision.gameObject.GetComponent<Player>().Damage();
            }
            GetComponent<Health>().TakeDamage(_damage, 0);
        }
    }
}
