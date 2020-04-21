using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WeaponController : MonoBehaviour
{
    public GameObject rightHandHandle;
    public GameObject leftHandHandle;
    public GameObject bulletSpawnPoint;
    public GameObject playerCamera;

    public float switchSpeed = 1f;
    
    public Weapon[] weapons;
    public int startWeaponIndex = 0;

    public Animator playerAnimator;

    private Weapon _equipped;
    private Weapon[] _instances;
    
    private int _equippedIndex;
    
    private Text _ammoInfoText;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        _ammoInfoText = PlayerUIController.instance.ammoInfoText;
        playerController = GetComponent<PlayerController>();
        
        _instances = new Weapon[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            _instances[i] = Instantiate(weapons[i], rightHandHandle.transform);
            _instances[i].playerAnimator = playerAnimator;
            _instances[i].bulletSpawn = bulletSpawnPoint;
            _instances[i].ammoInfoText = _ammoInfoText;
            _instances[i].playerCamera = playerCamera.transform;
            
            _instances[i].gameObject.SetActive(false);
        }
        
        EquipWeapon(startWeaponIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !GameManager.PlayerLocked())
        {
            _equipped.Switch();
            StartCoroutine(SwitchWeapon());
        }
    }

    IEnumerator SwitchWeapon()
    {
        yield return new WaitForSeconds(switchSpeed);
        
        EquipWeapon((_equippedIndex + 1) % weapons.Length);
    }

    public void EquipWeapon(int weaponIndex)
    {
        // if (!(_equipped is null))
        // {
        //     Destroy(_equipped.gameObject);
        // }
        //
        // _equippedIndex = weaponIndex;
        //
        // _equipped = Instantiate(weapons[weaponIndex], rightHandHandle.transform);
        // _equipped.playerAnimator = playerAnimator;
        // _equipped.bulletSpawn = bulletSpawnPoint;
        // _equipped.ammoInfoText = _ammoInfoText;

        if (_equipped != null)
        {
            _equipped.gameObject.SetActive(false);
        }

        _equipped = _instances[weaponIndex];
        _equipped.gameObject.SetActive(true);
        
        _equipped.Unsheath();

        _equippedIndex = weaponIndex;
    }
}
