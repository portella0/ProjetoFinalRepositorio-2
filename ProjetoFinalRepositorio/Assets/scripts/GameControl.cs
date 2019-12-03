using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
	static GameControl controller;

    public float deathSequenceDuration = 1.5f;	

    public float timeSlow;

    [Header("checkpoint")]
    public PlayerHealth checkpointStat;
    public PlayerMovement character;
    public GameObject checkpoint;

    [Header("stats")]
    public int NDeaths;
    public float totalGameTime;						
	public bool isGameOver;							

    void Awake()
	{
        character = GameObject.Find("Character").GetComponent<PlayerMovement>();
        checkpointStat = GameObject.Find("Character").GetComponent<PlayerHealth>();

        if (controller != null && controller != this)
		{
			Destroy(gameObject);
			return;
		}

        controller = this;

        Cursor.visible = false;

        DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
        checkpoint = checkpointStat.checkpoint;

        if (isGameOver)
        {
            return;
        }

        Time.timeScale = timeSlow;

		totalGameTime += Time.deltaTime;

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    public static bool IsGameOver()
	{
        if (controller == null)
        {
            return false;
        }

		return controller.isGameOver;
	}

	public static void PlayerDied()
	{
        if (controller == null)
        {
            return;
        }

        controller.NDeaths++;

		controller.Invoke("RespawnPlayer", controller.deathSequenceDuration);
	}

	public static void PlayerWon()
	{
        if (controller == null)
        {
            return;
        }

		controller.isGameOver = true;
	}

    public void RespawnPlayer()
    {
        character.transform.position = checkpoint.transform.position;
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, 0.1f);
        checkpointStat.resetStats();
        checkpointStat.isAlive = true;
        checkpointStat.characters[(PlayerPrefs.GetInt("Character", 1))-1].SetActive(true);
    }

    //void RestartScene()
    //{
    //	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //       RespawnPlayer();
    //   }
}
