using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//THIS SCRIPT IS ATTACHED TO THE TURRET BUILD MANAGER

public class RadialMenuController : MonoBehaviour
{

    GameObject radialMenu;

    public GameObject buttonParent;

    public GameObject radialButton;
    public List<GameObject> buttons;
    public List<GameObject> turrets;

    public Sprite repairSprite;

    TurretManager manager;

    GameObject turretToBuild;

    MoneyManager mm;
    int currentCost;

    public Text buildButtonText;

    public int repairCost;
    bool repairing = false;


    // Start is called before the first frame update
    void Start()
    {
        //Get handles to objects
        radialMenu = GameObject.Find("Turret Placement UI");
        radialMenu.SetActive(true);
        manager = FindObjectOfType<TurretManager>();
        buildButtonText = GameObject.FindGameObjectWithTag("BuildCost").GetComponent<Text>();
        radialMenu.SetActive(false);
        mm = FindObjectOfType<MoneyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (radialMenu.activeInHierarchy)
        {
            //Close menu if the esc button is clicked
            if (Input.GetButton("Cancel"))
            {
                unactivate(); //true
            }
        }
    }

    //ACTIVATE THE RADIAL MENU TO BUILD A TURRET
    public void activate()
    {
        GameManager.instance.gamePaused = false;
        GameManager.instance.inTurretMenu = true;
        
        // PlayerController p = FindObjectOfType<PlayerController>();
        //If this spot already has a turret, need to change its button to be a repair one
        GameObject existingTurret = manager.getBuilding().GetComponent<ClickTurretSpot>().getTurret();
        if (existingTurret != null) 
        {
            int pos = 0;
            int i = 0;
            foreach (GameObject turret in turrets) 
            {
                if (turret.name + "(Clone)" == existingTurret.name) 
                {
                    pos = i;
                }
                i += 1;
            }
            GameObject repairButton = Instantiate(radialButton);
            repairButton.transform.SetParent(buttonParent.transform, false);
            repairButton.transform.Find("Name").GetComponent<Text>().text = "Repair!";
            repairButton.GetComponentsInChildren<Image>()[1].sprite = repairSprite;
            repairButton.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            repairButton.tag = "RepairButton";
            float cost = existingTurret.GetComponent<TurretData>().getCost() * 0.75f;
            float currHP = existingTurret.GetComponent<BaseTurret>().getHP();
            float maxHP = existingTurret.GetComponent<BaseTurret>().getMaxHP();
            int price = (int) (((currHP * cost) / -maxHP) + cost);
            repairCost = price;
            repairButton.transform.Find("Cost").GetComponent<Text>().text = "£" + price.ToString();
            repairButton.transform.position = buttons[pos].transform.position;
            repairButton.GetComponent<Button>().onClick.AddListener(delegate { buttonCall(-1); });
            buttons[pos].SetActive(false);
        }

        //Open the menu
        radialMenu.SetActive(true);
        // p.GetComponent<PlayerController>().LockMovement();
        // p.UnlockCursor();
    }

    //CLOSE THE RADIAL MENU
    public void unactivate()
    {
        GameManager.instance.inTurretMenu = false;

        //Replace repair buttons with proper buttons
        foreach (GameObject button in buttons) 
        {
            button.SetActive(true);
        }

        GameObject[] repairButtons = GameObject.FindGameObjectsWithTag("RepairButton");
        foreach (GameObject repairB in repairButtons) 
        {
            Destroy(repairB);
        }
        
        // PlayerController p = FindObjectOfType<PlayerController>();
        buildButtonText.text = "0";
        radialMenu.SetActive(false);
        // if (!escapeUsed)
        // {
        //     p.LockCursor();
        // }
        // p.GetComponent<PlayerController>().UnlockMovement();
        turretToBuild = null;
        repairing = false;
        manager.getBuilding().GetComponent<ClickTurretSpot>().wasClosed();
    }

    //CONSTRUCT THE SELECTED TURRET
    public void buildTurret()
    {
        if (turretToBuild != null && !repairing)
        {
            if (mm.getMoney() >= currentCost)
            {
                mm.subtractMoney(currentCost);
                GameObject turret = manager.getBuilding();
                turret.GetComponent<MeshRenderer>().enabled = false;
                Destroy(turret.GetComponent<ClickTurretSpot>().getTurret());
                turret.GetComponent<ClickTurretSpot>().setTurret(Instantiate(turretToBuild, turret.transform.position, Quaternion.identity));
            }
            else
            {
                PlayerUIController.instance.infoText.text = "NOT ENOUGH MONEY";
                PlayerUIController.instance.infoText.color = Color.red;
                PlayerUIController.instance.infoText.enabled = true;
                StartCoroutine(redText());
            }
        }
        else if (repairing) 
        {
            //Repair the turret rather than place a new one
            if (mm.getMoney() >= currentCost)
            {
                //Repair turret
                mm.subtractMoney(currentCost);
                manager.getBuilding().GetComponent<ClickTurretSpot>().getTurret().GetComponent<BaseTurret>().setHP();
            }
            else
            {
                PlayerUIController.instance.infoText.text = "NOT ENOUGH MONEY";
                PlayerUIController.instance.infoText.color = Color.red;
                PlayerUIController.instance.infoText.enabled = true;
                StartCoroutine(redText());
            }
        }
        unactivate();
    }

    public void selectTurret(int ind)
    {
        //A turret has been selected
        repairing = false;
        turretToBuild = turrets[ind];
        currentCost = turrets[ind].GetComponent<TurretData>().getCost();
        buildButtonText.text = "£" + turrets[ind].GetComponent<TurretData>().getCost().ToString();
    }

    public void selectRepair()
    {
        //Repair has been selected
        repairing = true;
        currentCost = repairCost;
        buildButtonText.text = "£" + repairCost.ToString();
    }

    public void addButton(int cost, Sprite sprite, string name, GameObject turret)
    {
        //Add a new button for a new turret
        GameObject newButton = Instantiate(radialButton);
        newButton.transform.SetParent(buttonParent.transform, false);
        newButton.GetComponentsInChildren<Image>()[1].sprite = sprite;
        newButton.transform.Find("Cost").GetComponent<Text>().text = cost.ToString();
        newButton.transform.Find("Name").GetComponent<Text>().text = name;
        newButton.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        int count = buttons.Count;
        newButton.GetComponent<Button>().onClick.AddListener(delegate { buttonCall(count); });
        buttons.Add(newButton);
        turrets.Add(turret);

        //update button positions
        float angleIncrement = 2 * Mathf.PI / buttons.Count;
        float angleCount = 0;
        foreach (GameObject button in buttons)
        {
            button.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Sin(angleCount) * 45, Mathf.Cos(angleCount) * 45, 0);
            // button.GetComponent<RectTransform>().localScale = new Vector3(0.2f, 0.2f, 1);
            angleCount += angleIncrement;
        }
    }

    public void buttonCall(int pos)
    {
        //A button has been pressed
        if (pos == -1)
        {
            selectRepair();
        }
        else
        {
            selectTurret(pos);
        }
    }

    public IEnumerator redText()
    {
        yield return new WaitForSeconds(1.5f);
        PlayerUIController.instance.infoText.enabled = false;
        PlayerUIController.instance.infoText.color = Color.white;
    }
}
