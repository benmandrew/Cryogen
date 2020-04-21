using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClickTurretSpot : MonoBehaviour
{

    public GameObject currentTurret;
    public int clickDistance = 4;

    public Collider[] hitboxes;

    bool canInteract = true;

    private PlayerController _playerController;

    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(_playerController.playerCamera.transform.position, _playerController.playerCamera.transform.forward);
            
            RaycastHit[] hits = Physics.RaycastAll(ray, clickDistance, LayerMask.GetMask("Turrets"));

            float closest = Mathf.Infinity;
            float foundDist = -1;

            foreach (RaycastHit hit in hits)
            {
                if (hit.distance < closest)
                {
                    closest = hit.distance;
                }
                if (hitboxes.Contains(hit.collider))
                {
                    foundDist = hit.distance;
                    
                    // doAction();
                    // break;
                }
            }

            if (foundDist >= 0f && foundDist <= closest)
            {
                doAction();
            }
        }
    }

    void doAction()
    {
        canInteract = false;
        FindObjectOfType<TurretManager>().setBuilding(this.gameObject);
        FindObjectOfType<RadialMenuController>().activate();
    }

    public void setTurret(GameObject turret)
    {
        currentTurret = turret;
    }

    public GameObject getTurret()
    {
        return this.currentTurret;
    }

    // public void OnMouseOver()
    // {
    //     doMouseOver();
    // }

    public void wasClosed()
    {
        canInteract = true;
    }

    public void turretHasBeenDestroyed() 
    {
        if (currentTurret == null) 
        {
            //It was this turret that has been destroyed
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    // public void doMouseOver() 
    // {
    //     if (Input.GetKeyDown(KeyCode.E) && canInteract)
    //     {
    //         Vector3 playerPos = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
    //         if (Vector3.Distance(playerPos, transform.position) < clickDistance)
    //         {
    //             doAction();
    //         }
    //     }
    // }
}
