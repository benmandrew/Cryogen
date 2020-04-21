using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool gamePaused = false;
    public bool inTurretMenu = false;

    public bool playerDead = false;
    public bool heartDead = false;

    private bool _cursorLocked = false;
    public bool playerLocked = false;

    public WaveManager wm;
    public PlayerController pc;
    
    // Start is called before the first frame update
    void Start()
    {
        wm = GameObject.FindWithTag("WaveManager").GetComponent<WaveManager>();
        Time.timeScale = 1.0f;
        instance = this;
        LockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel")) && !wm.hasWon)
        {
            if (gamePaused || inTurretMenu)
            {
                gamePaused = false;
                inTurretMenu = false;
            }
            else if (!gamePaused)
            {
                gamePaused = true;
                inTurretMenu = false;
            }
        }

        if (PlayerLocked() && _cursorLocked)
        {
            UnlockCursor();
        }
        else if (!PlayerLocked() && !_cursorLocked && !playerDead)
        {
            LockCursor();
        }
    }

    public static bool Paused()
    {
        if (instance == null)
        {
            return false;
        }
        
        return instance.gamePaused;
    }

    public static bool InTurretMenu()
    {
        if (instance == null)
        {
            return false;
        }
        
        return instance.inTurretMenu;
    }

    public static bool PlayerLocked()
    {
        if (instance == null)
        {
            return false;
        }
        return instance.gamePaused || instance.inTurretMenu || instance.playerLocked;
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        instance._cursorLocked = true;
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        instance._cursorLocked = false;
    }
}
