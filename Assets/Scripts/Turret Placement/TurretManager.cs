using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    GameObject currentlyBuilding = null;

    public void setBuilding(GameObject turret)
    {
        currentlyBuilding = turret;
    }

    public void clearBuilding()
    {
        currentlyBuilding = null;
    }

    public GameObject getBuilding()
    {
        return currentlyBuilding;
    }

    public void destroyTurret(GameObject turret) 
    {
        Destroy(turret);
        StartCoroutine(shortWait());
    }

    IEnumerator shortWait() 
    {
        yield return new WaitForSeconds(0.05f);
        foreach (ClickTurretSpot spot in GameObject.FindObjectsOfType<ClickTurretSpot>())
        {
            spot.turretHasBeenDestroyed();
        }
    }
}
