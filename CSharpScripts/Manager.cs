using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    [HideInInspector] public Transform player;
    [HideInInspector] public int enemyCount=0;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] AudioClip deathSound;
    [SerializeField] GameObject[] Rooms;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject bossRoom;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject defendEnemy;
    [SerializeField] GameObject pressEText;
    [SerializeField] Image[] roomsGUI;
    [SerializeField] int enemyHp = 100;
    [SerializeField] float minHp = 0.995f;
    [SerializeField] float maxHp = 1.01f;
    AudioSource audioSound;
    GameObject room;
    int score = 0;
    int _enemyHp;
    int waveCount=0;
    int defenseRnd;
    int roomRandomStart = 0;
    bool isDefend = true;
    bool isReady = false;
    bool dead = true;
    bool exitConfirm = false;
    bool isBoss = false;
    [Header("Story")]
    public static bool isStory = false;
    int storyWaves = 7;
    int storyRoom = 1;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (MenuManager.isHost)
            NetworkManager.Singleton.StartHost();
        else if (MenuManager.isJoin)
            NetworkManager.Singleton.StartClient();
        else
            Instantiate(playerPrefab);
        FindPlayerTransform();
        audioSound = Camera.main.GetComponent<AudioSource>();
        PressContinueDead();
        ExitConfirmReset();
    }
    void FindPlayerTransform() {
        Debug.Log("FindPlayerTransform()");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log(player);
        if (player == null) Invoke("FindPlayerTransform",0.05f);
    }
    void DefendSpawnEnemy()
    {
        float x = Random.Range(-8.4f, 8.4f);
        float y = Random.Range(-4, 4.5f);
        if (x > -6 && x < 6)
        {
            y = Random.Range(3.5f, 4.5f);
        }
        Vector3 rnd = new(x, y);
        Health _health = Instantiate(defendEnemy, rnd, Quaternion.identity).GetComponent<Health>();
        _health.hp = enemyHp;
        if (defenseRnd != 0) _health.hp *= 2;
        enemyCount++;
    }
    void DefendWaveStart()
    {
        defenseRnd = Random.Range(0, 3);
        if (defenseRnd == 0) { DefendSpawnEnemy(); }
        DefendSpawnEnemy();
    }
    void WaveStart()
    {
        Destroy(room);
        player.position = new(-8, -4, 0);
        if (storyWaves == 0)
        {
            roomsGUI[storyRoom+1].color = Color.green;
            PressContinue();
            enemyHp = (int)(enemyHp*1.5f);
            storyWaves = 7;
            storyRoom++;
            Debug.Log("[HP] enemyHp: " + enemyHp);
            return;
        } else if (storyRoom == 6)
        {
            room = Instantiate(bossRoom, transform.position, Quaternion.identity);
            isBoss = true;
            return;
        } else if (storyRoom >= 7)
        {
            PressContinue();
            return;
        }
        if (isStory) storyWaves--;
        if (isDefend) enemyHp = _enemyHp;
        if (!isStory) enemyHp = (int)(enemyHp * Random.Range(minHp, maxHp));
        _enemyHp = enemyHp;
        byte roomRnd = (byte)Random.Range(roomRandomStart, Rooms.Length);
        if (isDefend) roomRnd = 0;
        if (isStory && roomRnd == 0) roomRnd = (byte)Random.Range(roomRandomStart, Rooms.Length);
        if (storyWaves == 2 || storyWaves == 6) roomRnd = 0;
        Room _room = Rooms[roomRnd].GetComponent<Room>();
        isDefend = _room.isDefend;
        enemyCount = _room.enemyCount;

        foreach (GameObject x in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            Destroy(x);
        }
        if (isDefend)
        {
            bool _laser = player.GetComponent<Weapon>().laser;
            if (_laser)
            {
                float _playerdps = player.GetComponent<Weapon>().damage / player.GetComponent<Weapon>().hitTime;
                Debug.Log("playerdps: " + _playerdps);
                enemyHp = (int)(_playerdps/2);
            }
            else enemyHp = player.GetComponent<Weapon>().damage;
            waveCount = _room.enemyCount; // in defend its wave count
            player.position = new(0, -4, 0);
            enemyCount = 0;
            DefendWaveStart();
        }
        room = Instantiate(_room.gameObject);
        foreach (GameObject x in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            x.GetComponent<Health>().hp = enemyHp;
        }
        Debug.Log("[Trigger] triggered GameManager.WaveStart() | enemyCount: " + enemyCount + " | isDefend: " + isDefend);
    }
    public void EnemyDead()
    {
        if (isBoss) return;
        Debug.Log("[Trigger] triggered GameManager.EnemyDead() | enemyCount: " + enemyCount);
        if (!isStory) ScoreAdd(1);
        enemyCount--;
        //Debug.Log("enemyCount: " + enemyCount + " | isDefend: " + isDefend + " | waveCount: " + waveCount);
        if (enemyCount <= 0 && isReady)
        {
            if (isDefend)
            {
                waveCount--;
                if (waveCount <= 0)
                    WaveStart();
                else
                    DefendWaveStart();
            } else
                WaveStart();
        }
    }
    public void BossDead()
    {
        foreach (GameObject x in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            Destroy(x);
        }
        foreach (GameObject x in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(x);
        }
    }
    public void GameReset(InputAction.CallbackContext _context)
    {
        if (_context.canceled && !isReady)
        {
            Debug.Log("Manager.Interact");
            roomsGUI[0].gameObject.SetActive(false);
            pressEText.SetActive(false);
            isReady = true;
            if (dead)
            {
                dead = false;
                if (isStory)
                {
                    roomRandomStart = 1;
                    storyRoom = 1;
                }
                else
                    ScoreAdd(-score);
                enemyHp = 100;
                _enemyHp = enemyHp;
            }
            if (isStory)
            {
                storyWaves = 7;
            }
            WaveStart();
        }
    }
    public void PressContinue()
    {
        foreach (GameObject x in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            Destroy(x);
        }
        foreach (GameObject x in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(x);
        }
        GameObject _x = GameObject.FindGameObjectWithTag("Boss");
        Destroy(_x);
        if (isStory) roomsGUI[0].gameObject.SetActive(true);
        isReady = false;
        pressEText.SetActive(true);
    }
    public void PressContinueDead()
    {
        audioSound.clip = deathSound;
        audioSound.Play();
        Destroy(room);
        dead = true;
        if (isStory)
        {
            for (int i = 2; i < roomsGUI.Length; i++)
            {
                roomsGUI[i].color = Color.white;
            }
        }
        PressContinue();
    }
    public void MainMenu(InputAction.CallbackContext context)
    {
        Debug.Log("triggered MainMenu Input: " + exitConfirm);
        if (context.canceled && exitConfirm) SceneManager.LoadScene("MainMenu");
        if (context.canceled) exitConfirm = true; Invoke(nameof(ExitConfirmReset), 2f);
    }
    void ExitConfirmReset()
    {
        exitConfirm = false;
    }
    void ScoreAdd(int _score)
    {
        score += _score;
        scoreText.text = score.ToString();
    }
}
