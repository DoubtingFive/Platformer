using UnityEngine;

public class Chest : MonoBehaviour
{
    readonly float minDamage = 1.27f;
    readonly float maxDamage = 1.31f;
    readonly float minReach = 1.11f;
    readonly float maxReach = 1.23f;
    readonly float minHitTime = 0.97f;
    readonly float maxHitTime = 0.93f;
    public AudioClip openSound;
    public LayerMask playerMask;
    public float range = 3f;
    AudioSource audioSound;
    GameObject player;
    private void Start()
    {
        audioSound = Camera.main.GetComponent<AudioSource>();
        player = Manager.Instance.player.gameObject;
    }
    private void Update()
    {
        if (Physics2D.CircleCast(transform.position, range, Vector2.up, range,playerMask) && Input.GetButtonDown("Interact")) {
            Debug.Log("Chest is in opening state");
            audioSound.clip = openSound;
            audioSound.Play();
            if (player.GetComponent<Player>().hp == 3)
                StatChange();
            else
                player.GetComponent<Player>().Heal();
            Debug.Log("Chest opened");
            Manager.Instance.EnemyDead();
            Destroy(gameObject);
        }
    }
    void StatChange()
    {
        Weapon _player = player.GetComponent<Weapon>();
        int rnd = Random.Range(0, 3);
        if (_player.laser) rnd = Random.Range(0, 2);
        switch (rnd)
        {
            case 0:
                int _dmg = _player.damage;
                _player.damage = (int)(_dmg * Random.Range(minDamage, maxDamage));
                Debug.Log("hitTime: " + _player.hitTime + " | damage: " + _dmg + " > " + _player.damage + " | reach: " + _player.reach);
                _player.ChangeStats(0,_dmg);
                break;
            case 1:
                float _time = _player.hitTime;
                _player.hitTime *= Random.Range(minHitTime, maxHitTime);
                Debug.Log("hitTime: " + _time + " > " + _player.hitTime + " | damage: " + _player.damage + " | reach: " + _player.reach);
                _player.ChangeStats(1, _time);
                break;
            case 2:
                float _reach = _player.reach;
                _player.reach *= Random.Range(minReach, maxReach);
                Debug.Log("hitTime: " + _player.hitTime + " | damage: " + _player.damage + " | reach: " + _reach + " > " + _player.reach);
                _player.ChangeStats(2, _reach);
                break;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
