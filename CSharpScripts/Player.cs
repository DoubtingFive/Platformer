using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] bool god = false;
    [SerializeField] Color fullHeart;
    [SerializeField] Color outlineHeart;
    List<Image> hearts = new List<Image>();
    public int hp = 3;
    private void Start()
    {
        foreach (GameObject x in GameObject.FindGameObjectsWithTag("Heart"))
        {
            hearts.Add(x.GetComponent<Image>()); // usun serca w inspektorze
        }
        Image _heart = hearts[0];
        hearts[0] = hearts[1];
        hearts[1] = _heart;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && !god)
        {
            hp--;
            Debug.Log("hp: "+hp);
            hearts[hp].color = Color.clear;
            hearts[hp].transform.Find("Outline").GetComponent<Image>().color = Color.clear;
            if (hp <= 0 )
            {
                Heal();
                GetComponent<Weapon>().ResetWeaponStatistics();
                Manager.Instance.PressContinueDead();
            }
        }
    }
    public void Heal()
    {
        hp = 3;
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].transform.Find("Outline").GetComponent<Image>().color = outlineHeart;
            hearts[i].color = fullHeart;
        }
    }
    public void Damage()
    {
        if (!god)
        {
            hp--;
            hearts[hp].color = Color.clear;
            hearts[hp].transform.Find("Outline").GetComponent<Image>().color = Color.clear;
            if (hp <= 0)
            {
                Heal();
                GetComponent<Weapon>().ResetWeaponStatistics();
                Manager.Instance.PressContinueDead();
            }
        }
    }
}
