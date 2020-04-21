using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public GameObject playerPrefab;
    
    // Start is called before the first frame update
    void Awake()
    {
        var t = transform;
        Instantiate(playerPrefab, t.position, t.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
