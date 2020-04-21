using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{

    Doors parent;
    // [HideInInspector] public PlayerUIController cont;
    public List<Material> mouseOn;
    public List<Material> mouseOff;
    public Collider collider;
    int clickDist = 6;
    MeshRenderer rend;

    private PlayerController _playerController;

    private bool hovering;
    
    private void Start()
    {
        parent = gameObject.GetComponentInParent<Doors>();
        // cont = PlayerUIController.instance;
        PlayerUIController.instance.infoText.enabled = false;
        rend = gameObject.GetComponent<MeshRenderer>();

        _playerController = FindObjectOfType<PlayerController>();
    }


    void Update()
    {
        Ray ray = new Ray(_playerController.playerCamera.transform.position, _playerController.playerCamera.transform.forward);

        bool found = false;

        float closest = Mathf.Infinity;
        float foundDist = -1f;
        
        foreach (RaycastHit hit in Physics.RaycastAll(ray, clickDist, LayerMask.GetMask("Default")))
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                parent.doAction();
            }

            if (!(parent.isUnlocked()))
            {
                //Debug.Log("SHOW PRICE");
                PlayerUIController.instance.infoText.text = "£" + parent.getCost();
                PlayerUIController.instance.infoText.enabled = true;
                int i = 0;
                foreach (Material mat in mouseOn)
                {
                    rend.materials[i] = mat;
                    i += 1;
                }

                //rend.material = mouseOn;
            }
        }

        if (!found && hovering)
        {
            hovering = false;
            
            exitRange();
        }
    }
    //
    // private void OnMouseOver()
    // {
    //     Vector3 playerPos = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
    //     if (Vector3.Distance(playerPos, transform.position) < clickDist)
    //     {
    //         if (Input.GetKeyDown(KeyCode.E))
    //         {
    //             parent.doAction();
    //         }
    //         if (!(parent.isUnlocked()))
    //         {
    //             //Debug.Log("SHOW PRICE");
    //             PlayerUIController.instance.infoText.text = "£" + parent.getCost();
    //             PlayerUIController.instance.infoText.enabled = true;
    //             int i = 0;
    //             foreach (Material mat in mouseOn)
    //             {
    //                 rend.materials[i] = mat;
    //                 i += 1;
    //             }
    //             //rend.material = mouseOn;
    //         }
    //     }
    //     else 
    //     {
    //         exitRange();
    //     }
    // }
    //
    // private void OnMouseExit()
    // {
    //     exitRange();
    // }

    void exitRange() 
    {
        PlayerUIController.instance.infoText.enabled = false;
        int i = 0;
        foreach (Material mat in mouseOff)
        {
            rend.materials[i] = mat;
            i += 1;
        }
        //rend.material = mouseOff;
    }
}
