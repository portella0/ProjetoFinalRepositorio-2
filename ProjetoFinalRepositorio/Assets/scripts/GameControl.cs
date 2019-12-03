using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
	static GameControl controller;
    public GameObject pauseScreen;
    public GameObject controlScreen;
     
    public float deathSequenceDuration = 1.5f;
    public int sceneCount;

    public float timeSlow;
    public bool paused = false;

    [Header("checkpoint")]
    public PlayerHealth checkpointStat;
    public PlayerMovement character;
    public GameObject checkpoint;
    public menuController menuControl;
    PlayerInput input;

    [Header("stats")]
    public int NDeaths;
    public float totalGameTime;						
	public bool isGameOver;

    [Header("stats")]
    AudioSource audioSource;
    public AudioClip buttonClick;

    void Awake()
	{
        character = GameObject.Find("Character").GetComponent<PlayerMovement>();
        checkpointStat = GameObject.Find("Character").GetComponent<PlayerHealth>();
        input = GameObject.Find("Character").GetComponent<PlayerInput>();
        menuControl = gameObject.GetComponentInChildren<menuController>();
        audioSource = GetComponent<AudioSource>();

        if (controller != null && controller != this)
		{
			Destroy(gameObject);
			return;
		}

        paused = false;
        Time.timeScale = 1f;

        controller = this;

        Cursor.visible = false;

        DontDestroyOnLoad(gameObject);
	}

    void Update()
    {
        if (Input.GetButtonDown("pause") && SceneManager.GetActiveScene().buildIndex != 0)
        {
            togglePause();
        }

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            character = GameObject.Find("Character").GetComponent<PlayerMovement>();
            checkpointStat = GameObject.Find("Character").GetComponent<PlayerHealth>();
            input = GameObject.Find("Character").GetComponent<PlayerInput>();
        }

        if (checkpointStat != null)
        {
            checkpoint = checkpointStat.checkpoint;

            if (isGameOver)
            {
                return;
            }

            //if (paused)
            //Time.timeScale = timeSlow;

            totalGameTime += Time.deltaTime;

            //if (Input.GetKey("escape"))
            //{
            //    Application.Quit();
            //}

            if (input.menu)
            {
                SceneManager.LoadScene(0);
            }
            if (input.respawn)
            {
                checkpointStat.IsDead();
            }
        }
    }

    public void Resume()
    {
        if (buttonClick != null)
        {
            audioSource.PlayOneShot(buttonClick, 0.2f);
        }
        togglePause();
    }

    public void controlsButton()
    {
        if (buttonClick != null)
        {
            audioSource.PlayOneShot(buttonClick, 0.2f);
        }
        controlScreen.SetActive(true);
        pauseScreen.SetActive(false);
        menuControl.defaultButton = menuControl.returnButton;
        menuControl.resetButton();
    }

    public void backButton()
    {
        if (buttonClick != null)
        {
            audioSource.PlayOneShot(buttonClick, 0.2f);
        }
        controlScreen.SetActive(false);
        pauseScreen.SetActive(true);
        menuControl.defaultButton = menuControl.resumeButton;
        menuControl.resetButton();
    }

    public void saveAndQuit()
    {
        if (buttonClick != null)
        {
            audioSource.PlayOneShot(buttonClick, 0.2f);
        }
        togglePause();
        sceneCount = SceneManager.GetActiveScene().buildIndex;
        SaveSystem.SetInt("sceneCount", sceneCount);
        SceneManager.LoadScene(0);
    }

    void togglePause()
    {
        if (Time.timeScale < 1f)
        {
            pauseScreen.SetActive(false);
            pauseScreen.SetActive(false);
            controlScreen.SetActive(false);
            paused = false;
            Time.timeScale = 1f;
            Cursor.visible = false;
        }
        else
        {
            paused = true;
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
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
        if (checkpointStat.skeletonSprite != null)
        {
            checkpointStat.skeletonSprite.color = new Color(1, 1, 1);
        }
        if (checkpointStat.archerHeadSprite != null)
        {
            checkpointStat.archerHeadSprite.color = new Color(1, 1, 1);
        }
        if (checkpointStat.golemSprite != null)
        {
            checkpointStat.golemSprite.color = new Color(1, 1, 1);
        }

        character.isHanging = false;
        character.transform.position = new Vector2(checkpoint.transform.position.x, checkpoint.transform.position.y);
        //character.transform.position = new Vector2(character.transform.position.x, character.transform.position.y);
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
