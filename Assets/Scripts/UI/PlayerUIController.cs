using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public Text ammoInfoText;
    public Text moneyText;
    public Text deltaMoneyText;
    public Text reloadWarningText;
    public Text nextWaveText;
    public Text infoText;
    public Text enemiesLeftText;
    public RectTransform playerHealthBar;
    public RectTransform heartHealthBar;
    public GameObject deathScreen;
    public GameObject winScreen;
    public GameObject crosshair;
    public GameObject ammoinfo;
    public GameObject money;
    public GameObject enemiesLeft;
    public GameObject playerHealth;
    public GameObject heartHealth;

    public static PlayerUIController instance
    {
        get
        {
            if (_instance is null)
            {
                Debug.LogError("No Player UI is set in this scene.");
            }
            return _instance;
        }
        private set => _instance = value;
    }

    private static PlayerUIController _instance;

    void Awake()
    {
        instance = this;
    }

    public void SetPlayerHealthPercent(float percent)
    {
        playerHealthBar.localScale = new Vector3(Mathf.Clamp01(percent), 1, 1);
    }

    public void SetHeartHealthPercent(float percent)
    {
        heartHealthBar.localScale = new Vector3(Mathf.Clamp01(percent), 1, 1);
    }

    public void win() {
        winScreen.SetActive(true);
        GameManager.UnlockCursor();
        GameManager.instance.playerDead = true;
        GameManager.instance.playerLocked = true;
        crosshair.SetActive(false);
        ammoinfo.SetActive(false);
        money.SetActive(false);
        playerHealth.SetActive(false);
        heartHealth.SetActive(false);
        enemiesLeft.SetActive(false);
    }
}
