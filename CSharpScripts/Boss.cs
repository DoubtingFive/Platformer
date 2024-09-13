using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public GameObject enemyProjectile;
    public GameObject enemyObject;
    public GameObject shadow;
    public Transform[] points;
    public AudioClip[] sounds;
    public Slider bossBar;
    public float speed = 1;
    public float shootOffset = 2;
    public float phaseTime = 5f;
    public float phaseCooldown;
    AudioSource audioSound;
    Transform player;
    readonly string[] phases = { "ShootAround", "MoveAround", "SummonEnemy", "Jump" };
    int isWait = 0;
    int positionPoint = -1;
    bool isJump = false;
    private void Start()
    {
        player = Manager.Instance.player;
        audioSound = Camera.main.GetComponent<AudioSource>();
        transform.position = points[0].position;
        phaseCooldown = 2;
    }
    private void Update()
    {
        if (positionPoint >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, points[positionPoint].position, speed * Time.deltaTime);
            if (transform.position == points[positionPoint].position)
            {
                if (isWait == 0) { Invoke("AllowWait", 0.32f); isWait = 2; }
                if (isWait == 2) return;
                if (positionPoint == 0)
                {
                    positionPoint = -1;
                    return;
                }
                positionPoint++;
                if (positionPoint == 3)
                {
                    transform.localScale = Vector3.one * 3;
                }
                if (positionPoint == 5)
                {
                    transform.localScale = Vector3.one*2;
                    positionPoint = 0;
                }
                isWait = 0;
            }
        }
        else if (isJump)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime / 3);
        }
        else if (phaseCooldown <= 0)
        {
            phaseCooldown = phaseTime;
            Invoke(phases[Random.Range(0, phases.Length)], 0);
        }
        else { phaseCooldown -= Time.deltaTime; transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 0), speed * Time.deltaTime / 2); }
    }
    void ShootAround()
    {
        phaseCooldown -= 1;
        audioSound.clip = sounds[0];
        audioSound.Play();
        Instantiate(enemyProjectile, transform.position + new Vector3(shootOffset, 0), Quaternion.Euler(0, 0, 180));
        Instantiate(enemyProjectile, transform.position + new Vector3(-shootOffset, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(enemyProjectile, transform.position + new Vector3(0, shootOffset), Quaternion.Euler(0, 0, -90));
        Instantiate(enemyProjectile, transform.position + new Vector3(0, -shootOffset), Quaternion.Euler(0, 0, 90));
        Instantiate(enemyProjectile, transform.position + new Vector3(shootOffset, shootOffset), Quaternion.Euler(0, 0, -135));
        Instantiate(enemyProjectile, transform.position + new Vector3(-shootOffset, shootOffset), Quaternion.Euler(0, 0, -45));
        Instantiate(enemyProjectile, transform.position + new Vector3(shootOffset, -shootOffset), Quaternion.Euler(0, 0, 135));
        Instantiate(enemyProjectile, transform.position + new Vector3(-shootOffset, -shootOffset), Quaternion.Euler(0, 0, 45));
    }
    void MoveAround()
    {
        positionPoint = 2;
    }
    void AllowWait()
    {
        audioSound.clip = sounds[1];
        audioSound.Play();
        isWait = 1;
    }
    void SummonEnemy()
    {
        phaseCooldown += 3;
        audioSound.clip = sounds[2];
        audioSound.Play();
        Instantiate(enemyObject, transform.position + new Vector3(2,1), Quaternion.identity);
        Instantiate(enemyObject, transform.position + new Vector3(-2,1), Quaternion.identity);
    }
    void Jump()
    {
        audioSound.clip = sounds[3];
        audioSound.Play();
        isJump = true;
        shadow.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Invoke("StopJump", 2f);
    }
    void StopJump()
    {
        audioSound.clip = sounds[3];
        audioSound.Play();
        isJump = false;
        shadow.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
