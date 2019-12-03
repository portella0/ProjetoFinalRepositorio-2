using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class menuController : MonoBehaviour
{
    public Button previousButton;
    public GameObject returnButton, resumeButton, returnButton2;
    public float scaleAmount = 1f;
    public GameObject defaultButton;
    public Vector3 defaultScale;
    public bool defaulDo;

    [Header("stats")]
    AudioSource audioSource;
    public AudioClip buttonChange;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        defaulDo = true;
        if (defaultButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }

    void Update()
    {
        var selectedObj = EventSystem.current.currentSelectedGameObject;

        if(selectedObj == null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }

        //defaultButton.transform.localScale = new Vector3(1, 1, 1); 

        if (selectedObj == null) return;
        var selectedAsButton = selectedObj.GetComponent<Button>();
        if (selectedAsButton != null && selectedAsButton != previousButton)
        {
            if (selectedAsButton.transform.name != "PauseButton")
                HighlightButton(selectedAsButton);
        }

        if (previousButton != null && previousButton != selectedAsButton)
        {
            UnHighlightButton(selectedAsButton);
        }
        previousButton = selectedAsButton;
    }
    void OnDisable()
    {
        if (previousButton != null) UnHighlightButton(previousButton);
    }

    public void resetButton()
    {
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    void HighlightButton(Button butt)
    {
        //if (SettingsManager.Instance.UsingTouchControls) return;
        if (buttonChange != null)
        {
            audioSource.PlayOneShot(buttonChange);
        }
        butt.transform.localScale = new Vector3(butt.transform.localScale.x * scaleAmount, butt.transform.localScale.y * scaleAmount, butt.transform.localScale.z * scaleAmount);
    }

    void UnHighlightButton(Button butt)
    {
        //if (SettingsManager.Instance.UsingTouchControls) return;
        butt.transform.localScale = new Vector3(1,1,1);
    }
}