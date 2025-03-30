using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    public float xMin;
    public float zMin;
    public float xMax;
    public float zMax;
    public float y;
    public float itemSpawnTime;
    private float lastItemSpawnTime;

    public Player player;
    public Enemy boss;

    public GameObject shurikenPrefab;

    public GameObject gameOverScreen;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public Button endCreditButon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject p = GameObject.Find("Player");
        GameObject b = GameObject.Find("Incenoth");

        player = p.gameObject.GetComponent<Player>();
        boss = b.gameObject.GetComponent<Enemy>();
        gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastItemSpawnTime > itemSpawnTime)
        {
            lastItemSpawnTime = Time.time;
            SpawnItem(shurikenPrefab);
        }
        if (player.charHp <= 0)
        {
            gameOverText.text = "You Lose!!!";
            gameOverText.color = Color.red;
            gameOverScreen.SetActive(true);
        }
        if (boss.charHp <= 0)
        {
            gameOverText.text = "You Win!!!";
            gameOverText.color = Color.green;
            gameOverScreen.SetActive(true);
        }
    }

    void SpawnItem(GameObject prefab)
    {
        float x = Random.Range(xMin, xMax);
        float z = Random.Range(zMin, zMax);
        Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
    }

    public void OnRestartClick()
    {
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
    public void OnEndCreditClick()
    {
        SceneManager.LoadScene("EndCredit");
    }

    
}
