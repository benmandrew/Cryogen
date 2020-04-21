using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{

    public int money;
    private Text moneyText;
    private Text deltaMoneyText;

    private Dictionary<EnemyType, int> enemyKillRewards = new Dictionary<EnemyType, int>(){
        {EnemyType.basic, 8},
        {EnemyType.fast, 8},
        {EnemyType.strong, 30}
    };

    void Start() {
        moneyText = PlayerUIController.instance.moneyText;
        deltaMoneyText = PlayerUIController.instance.deltaMoneyText;
        deltaMoneyText.gameObject.SetActive(false);
    }

    public int getMoney() {
        return money;
    }

    public void enemyKilled(EnemyType type) {
        money += enemyKillRewards[type];
        moneyText.text = "£" + money.ToString();
        deltaMoneyText.gameObject.SetActive(true);
        deltaMoneyText.color = Color.green;
        deltaMoneyText.text = "+£" + enemyKillRewards[type].ToString();
        StartCoroutine(fadeDeltaMoneyText());
    }

    public void subtractMoney(int amount) {
        money -= amount;
        moneyText.text = "£" + money.ToString();
        deltaMoneyText.gameObject.SetActive(true);
        deltaMoneyText.color = Color.red;
        deltaMoneyText.text = "-£" + amount.ToString();
        StartCoroutine(fadeDeltaMoneyText());
    }

    IEnumerator fadeDeltaMoneyText() {
        Color original = deltaMoneyText.color;
        float fadeTime = 1.0f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime) {
            deltaMoneyText.color = Color.Lerp(original, Color.clear, t);
            yield return null;
        }
        deltaMoneyText.gameObject.SetActive(false);
    }
}
