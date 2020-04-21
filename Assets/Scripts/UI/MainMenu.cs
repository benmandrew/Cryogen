using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public Button muteButton;
    public Sprite muteImage;
    public Sprite unmuteImage;
    public Slider mouseSensitivitySlider;
    public GameObject tutorialUI;
    public GameObject mainCanvas;

    void Start() {
        if (!PlayerPrefs.HasKey("startVolume")) {
            PlayerPrefs.SetFloat("startVolume", AudioListener.volume);
        }
        if (!PlayerPrefs.HasKey("muted")) {
            PlayerPrefs.SetInt("muted", 0);
        }
        if (PlayerPrefs.GetInt("muted") == 1) {
            mute();
        } else {
            unmute();
        }
        tutorialUI.SetActive(false);
        mainCanvas.SetActive(true);

        if (!PlayerPrefs.HasKey("MouseSensitivity"))
        {
            PlayerPrefs.SetFloat("MouseSensitivity", 5f);
        }

        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
    }

    public void startGame() {
        SceneManager.LoadScene("LevelMain", LoadSceneMode.Single);
    }

    public void quit() {
        Application.Quit();
    }

    public void openTutorial() 
    {
        mainCanvas.SetActive(false);
        tutorialUI.SetActive(true);
    }

    public void closeTutorial() 
    {
        tutorialUI.SetActive(false);
        mainCanvas.SetActive(true);
    }

    public void pressMuteButton() {
        if (PlayerPrefs.GetInt("muted") == 1) {
            unmute();
        } else {
            mute();
        }
        PlayerPrefs.SetInt("muted", 1 - PlayerPrefs.GetInt("muted", 0));
    }

    private void mute() {
        AudioListener.pause = true;
        AudioListener.volume = 0.0f;
        muteButton.transform.GetChild(0).GetComponent<Image>().sprite = muteImage;
    }

    private void unmute() {
        AudioListener.pause = false;
        AudioListener.volume = PlayerPrefs.GetFloat("startVolume");
        muteButton.transform.GetChild(0).GetComponent<Image>().sprite = unmuteImage;
    }

    public void ChangeSliderValue()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
    }
}
