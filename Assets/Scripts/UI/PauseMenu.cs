using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public Button resumeButton;
    public Button quitButton;
    public Image backingImage;
    
    private bool _menuOpen;

    private float _returnTimescale = 1f;

    void Start() {
        _menuOpen = false;
        disable();
    }

    void enable() {
        resumeButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        backingImage.gameObject.SetActive(true);

        _menuOpen = true;
    }

    void disable() {
        resumeButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        backingImage.gameObject.SetActive(false);
        
        _menuOpen = false;
    }

    public void resumeCallback() {
        disable();
        Time.timeScale = _returnTimescale;
        GameManager.instance.gamePaused = false;
    }

    public void quitCallback() {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update() {
        bool paused = GameManager.Paused();

        if (paused && !_menuOpen)
        {
            enable();
            _returnTimescale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else if (!paused && _menuOpen)
        {
            disable();
            Time.timeScale = _returnTimescale;
        }
    }
}
