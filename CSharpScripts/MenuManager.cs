using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource clickAudioSource;
    public static float effectVolume = 0.5f;
    public static bool laser = false;
    public static bool isHost = false;
    public static bool isJoin = false;
    Button swordBtn, laserBtn;
    Button hostBtn, joinBtn, offlineBtn;

    private void Start()
    {
        hostBtn = GameObject.Find("Canvas/Main/Host").GetComponent<Button>();
        joinBtn = GameObject.Find("Canvas/Main/Join").GetComponent<Button>();
        offlineBtn = GameObject.Find("Canvas/Main/Offline").GetComponent<Button>();
        swordBtn = GameObject.Find("Canvas/Main/Sword").GetComponent<Button>();
        laserBtn = GameObject.Find("Canvas/Main/Laser").GetComponent<Button>();
        isHost = false;
        isJoin = false;
        hostBtn.interactable = false;
        joinBtn.interactable = false;
        offlineBtn.interactable = false;
        swordBtn.interactable = laser;
        laserBtn.interactable = !laser;
        audioMixer.SetFloat("masterVolume", effectVolume);
    }
    public void LoadStoryMode()
    {
        Sound();
        Manager.isStory = true;
        SceneManager.LoadScene("StoryMode");
    }
    public void LoadEndlessMode()
    {
        Sound();
        Manager.isStory = false;
        SceneManager.LoadScene("EndlessMode");
    }
    public void ExitGame()
    {
        Sound();
        Application.Quit();
    }
    public void Sound(bool isOptions = false)
    {
        clickAudioSource.Play();
        if (isOptions)
            GameObject.FindGameObjectWithTag("EffectSoundGUI").GetComponent<Slider>().value = effectVolume;
    }
    public void SetEffectsVolume(float level)
    {
        Sound();
        effectVolume = level;
        audioMixer.SetFloat("masterVolume", effectVolume);
    }
    public void SelectLaser()
    {
        Sound();
        laser = true;
        laserBtn.interactable = false;
        swordBtn.interactable = true;
    }
    public void SelectSword()
    {
        Sound();
        laser = false;
        laserBtn.interactable = true;
        swordBtn.interactable = false;
    }
    public void Host()
    {
        isHost = true;
        isJoin = false;
        hostBtn.interactable = false;
        joinBtn.interactable = true;
        offlineBtn.interactable = true;
    }
    public void Join()
    {
        isHost = false;
        isJoin = true;
        hostBtn.interactable = true;
        joinBtn.interactable = false;
        offlineBtn.interactable = true;
    }
    public void Offline()
    {
        isHost = false;
        isJoin = false;
        hostBtn.interactable = true;
        joinBtn.interactable = true;
        offlineBtn.interactable = false;
    }
}
