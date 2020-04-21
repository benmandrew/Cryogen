using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blueprint : MonoBehaviour
{

    public GameObject turret;
    public int clickDistance = 2;

    public Collider collider;
    //public int button;

    RadialMenuController controller;

    bool interacted = false;

    private TurretData turretData;

    private PlayerController _playerController;

    private bool hovering = false;

    public void Start()
    {
        controller = FindObjectOfType<RadialMenuController>();
        turretData = turret.GetComponentInChildren<TurretData>();

        _playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        Ray ray = new Ray(_playerController.playerCamera.transform.position, _playerController.playerCamera.transform.forward);

        bool found = false;

        float closest = Mathf.Infinity;
        float foundDist = -1;
        
        foreach (RaycastHit hit in Physics.RaycastAll(ray, clickDistance, LayerMask.GetMask("Default")))
        {
            if (hit.distance < closest)
            {
                closest = hit.distance;
            }
            if (hit.collider == collider)
            {
                foundDist = hit.distance;
            }
        }

        if (foundDist >= 0f && foundDist <= closest)
        {
            found = true;
            hovering = true;

            if (!interacted)
            {
                PlayerUIController.instance.infoText.text = "Blueprint: " + turretData.getName();
                PlayerUIController.instance.infoText.enabled = true;
            }

            if (Input.GetKeyDown(KeyCode.E) && !interacted)
            {
                doAction();
            }
        }

        if (!found && hovering)
        {
            hovering = false;
            
            if (!interacted)
            {
                PlayerUIController.instance.infoText.enabled = false;
            }
        }
    }

    private void doAction()
    {
        interacted = true;
        //Set one of the buttons on the radial menu to the weapon in the blueprint
        /*if (button == 1) {
            controller.setTurret1(turret);
            controller.getButton1().GetComponentsInChildren<Image>()[1].sprite = turret.GetComponent<TurretData>().getSprite();
            controller.getButton1().GetComponentInChildren<Text>().text = turret.GetComponentInChildren<TurretData>().getCost().ToString();
        }
        if (button == 2)
        {
            controller.setTurret2(turret);
            controller.getButton2().GetComponentsInChildren<Image>()[1].sprite = turret.GetComponent<TurretData>().getSprite();
            controller.getButton2().GetComponentInChildren<Text>().text = turret.GetComponentInChildren<TurretData>().getCost().ToString();
        }
        if (button == 3)
        {
            controller.setTurret3(turret);
            controller.getButton3().GetComponentsInChildren<Image>()[1].sprite = turret.GetComponent<TurretData>().getSprite();
            controller.getButton3().GetComponentInChildren<Text>().text = turret.GetComponentInChildren<TurretData>().getCost().ToString();
        }
        if (button == 4)
        {
            controller.setTurret4(turret);
            controller.getButton4().GetComponentsInChildren<Image>()[1].sprite = turret.GetComponent<TurretData>().getSprite();
            controller.getButton4().GetComponentInChildren<Text>().text = turret.GetComponentInChildren<TurretData>().getCost().ToString();
        }*/
        controller.addButton(turretData.getCost(), turretData.getSprite(), turretData.getName(), turret);
        // Debug.Log("YOU HAVE UNLOCKED TURRET " + turretData.getName());

        StartCoroutine(ShowReceivedTurret());
    }

    // private void OnMouseOver()
    // {
    //     Vector3 playerPos = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
    //     if (Vector3.Distance(playerPos, transform.position) < clickDistance)
    //     {
    //         if (!interacted)
    //         {
    //             PlayerUIController.instance.infoText.text = "Blueprint: " + turretData.getName();
    //             PlayerUIController.instance.infoText.enabled = true;
    //         }
    //
    //         if (Input.GetKeyDown(KeyCode.E) && !interacted)
    //         {
    //
    //             doAction();
    //         }
    //     }
    // }
    //
    // private void OnMouseExit()
    // {
    //     if (!interacted)
    //     {
    //         PlayerUIController.instance.infoText.enabled = false;
    //     }
    // }

    IEnumerator ShowReceivedTurret()
    {
        PlayerUIController.instance.infoText.text = "Blueprint Acquired";
        PlayerUIController.instance.infoText.enabled = true;

        yield return new WaitForSeconds(3f);
        
        PlayerUIController.instance.infoText.enabled = false;

    }

}
