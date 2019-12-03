using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject controls;
    public GameObject credits;
    public GameObject main;
    public GameObject all;
    public GameObject title;
    public int scene;
    public int nextSceneNumber;
    public bool passScene;
    public menuController menuControl;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip buttonSound, backButtonSound;

    public void PlayGame()
    {
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
        if (scene > 0 || scene > 1)
        {
            SceneManager.LoadScene(scene);
        }
    }

    public void Start()
    {
        menuControl = gameObject.GetComponentInParent<menuController>();
        audioSource = GetComponent<AudioSource>();
        scene = SaveSystem.GetInt("sceneCount");
        if (all != null)
        {
            all.SetActive(true);
        }
        if (main != null)
        {
            main.SetActive(true);
        }
        if (credits != null)
        {
            credits.SetActive(false);
        }
        if (controls != null)
        {
            controls.SetActive(false);
        }
        Cursor.visible = true;
        if (menuControl != null)
        {
            menuControl.resetButton();
        }

    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("PassScene");
            if (passScene)
            {
                SceneManager.LoadScene(nextSceneNumber);
            }
        }
    }

    public void NextScene()
    {
        SceneManager.LoadScene(nextSceneNumber);
    }

    public void Controls()
    {
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound, 0.5f);
        }
        main.SetActive(false);
        controls.SetActive(true);
        title.SetActive(false);
        menuControl.defaultButton = menuControl.returnButton;
        menuControl.resetButton();
    }

    public void Credits()
    {
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound, 0.5f);
        }
        main.SetActive(false);
        credits.SetActive(true);
        title.SetActive(false);
        menuControl.defaultButton = menuControl.returnButton2;
        menuControl.resetButton();
    }

    public void back()
    {
        if (backButtonSound != null)
        {
            audioSource.PlayOneShot(backButtonSound, 0.5f);
        }
        main.SetActive(true);
        controls.SetActive(false);
        credits.SetActive(false);
        title.SetActive(true);
        menuControl.defaultButton = menuControl.resumeButton;
        menuControl.resetButton();
    }

    public void QuitGame()
    {
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound, 0.5f);
        }
        Debug.Log("QUIT!"); 
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }
}

