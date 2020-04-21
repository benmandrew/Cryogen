using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{

    public int unlockCost;

    MoneyManager mm;

    public float openRot = 90;
    public float openSpeed = 0.5f;
    float targetRot;

    bool unlocked = false;
    bool doorOpening = false;
    float rotCount = 0.0f;
    public GameObject spawnerToActivate;

    // Start is called before the first frame update
    void Start()
    {
        mm = FindObjectOfType<MoneyManager>();
        targetRot = transform.eulerAngles.y + openRot;
    }

    private void Update()
    {
        if (doorOpening)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRot, 0), rotCount);
            rotCount += Time.deltaTime * openSpeed;
            if (rotCount > 5)
            {
                doorOpening = false;
            }
        }
    }

    public void doAction()
    {
        if (mm.getMoney() >= unlockCost && !unlocked)
        {
            mm.subtractMoney(unlockCost);
            unlocked = true;
            doorOpening = true;
            if (spawnerToActivate != null) 
            {
                spawnerToActivate.GetComponent<EnemyContainer>().activate();
            }
        }
        else if (mm.getMoney() < unlockCost && !unlocked)
        {
            PlayerUIController.instance.infoText.color = Color.red;
            StartCoroutine(redText());
        }
    }

    public bool isUnlocked()
    {
        return unlocked;
    }

    public int getCost()
    {
        return unlockCost;
    }

    public IEnumerator redText() 
    {
        yield return new WaitForSeconds(0.5f);
        PlayerUIController.instance.infoText.color = Color.white;
    }
}
