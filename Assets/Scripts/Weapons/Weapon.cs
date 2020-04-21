using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    
    public float baseDamage;
    public AnimationCurve falloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    public float maxEffectiveDistance = 100f;

    public int clipSize;
    public float reloadTime;

    // Shots per second
    public float fireRate = 1f;
    public bool autoFire = false;
    public ParticleSystem bulletExplosion;
    public RuntimeAnimatorController controller;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public AudioSource source;

    public GameObject bloodSplatter;

    public float recoilAngle = 1f;
    public float recoilResetTime = 0.5f;

    private const float recoilTime = 0.1f;
    
    internal Animator playerAnimator;
    internal GameObject bulletSpawn;
    internal Text ammoInfoText;
    internal Transform playerCamera;
    private int _currentAmmo;
    private float _lastFired;
    private bool _reloading;
    private Text reloadWarningText;
    private bool _weaponEnabled = true;
    private const float ReloadWarningPercent = 0.25f;
    private PlayerController playerController;

    void Start()
    {
        _lastFired = Time.time;
        _currentAmmo = clipSize;

        reloadWarningText = PlayerUIController.instance.reloadWarningText;

        playerAnimator.runtimeAnimatorController = controller;

        playerController = playerAnimator.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!(ammoInfoText is null))
        {
            ammoInfoText.text = _currentAmmo + "/" + clipSize;
        }

        reloadWarningText.gameObject.SetActive(clipSize * ReloadWarningPercent > _currentAmmo);

        if (!_weaponEnabled || GameManager.PlayerLocked())
        {
            return;
        }
        
        if (!autoFire)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - _lastFired > 1.0f / fireRate && _currentAmmo > 0 && !_reloading)
                {
                    FireBullet();
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time - _lastFired > 1.0f / fireRate && _currentAmmo > 0 && !_reloading)
                {
                    FireBullet();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !_reloading)
        {
            if (_currentAmmo < clipSize)
            {
                StartCoroutine(ReloadGun());
            }
        }

        if (_currentAmmo == 0 && !_reloading)
        {
            StartCoroutine(ReloadGun());
        }
    }
    
    private void FireBullet()
    {
        playerAnimator.SetTrigger("Shoot");
        bulletExplosion.Emit(1);

        if (shotClip != null && source != null)
        {
            source.PlayOneShot(shotClip);
        }

        StopCoroutine("RecoilGun");
        StartCoroutine(RecoilGun());
        
        _lastFired = Time.time;

        _currentAmmo -= 1;
        
        Ray bulletRay = new Ray(bulletSpawn.transform.position, bulletSpawn.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(bulletRay, out hit, maxEffectiveDistance, LayerMask.GetMask("Default")))
        {
            if (!hit.transform.CompareTag("Enemy")) {
                return;
            }
            
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            float distancePercent = hit.distance / maxEffectiveDistance;
            if (distancePercent >= 1)
            {
                return;
            }
            
            float damage = baseDamage * falloffCurve.Evaluate(distancePercent);
            
            enemy.damage(damage, hit.collider);

            StartCoroutine(BloodEffect(hit.point));
        }
    }

    private IEnumerator BloodEffect(Vector3 position)
    {
        GameObject splatter = Instantiate(bloodSplatter, position, Quaternion.identity);
        
        yield return new WaitForSeconds(1f);
        
        Destroy(splatter);
    }

    private IEnumerator ReloadGun()
    {
        playerAnimator.SetTrigger("Reload");

        if (reloadClip != null && source != null)
        {
            source.clip = reloadClip;
            source.PlayDelayed(0.2f);
        }
        
        _reloading = true;
        
        yield return new WaitForSeconds(reloadTime);

        _currentAmmo = clipSize;
        _reloading = false;
    }

    IEnumerator RecoilGun()
    {
        // float startRot = playerCamera.localEulerAngles;
        // Quaternion targetRot = startRot * Quaternion.Euler(recoilAngle, 0, 0);

        float elapsed = 0f;

        while (elapsed < recoilTime)
        {
            // Vector3 rot = playerCamera.localEulerAngles;
            //
            // rot.x += Time.deltaTime * recoilAngle / recoilTime;
            //
            // playerCamera.localEulerAngles = rot;
            playerController.AddMouseY(-Time.deltaTime * recoilAngle / recoilTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

         while (elapsed < recoilResetTime)
         {
             playerController.AddMouseY(Time.deltaTime * recoilAngle / recoilResetTime);

             elapsed += Time.deltaTime;
             yield return null;
         }
    }

    public void Switch()
    {
        playerAnimator.SetTrigger("Switch");
        _weaponEnabled = false;
    }

    public void Unsheath()
    {
        playerAnimator.SetTrigger("Reset");
        _weaponEnabled = true;
        playerAnimator.runtimeAnimatorController = controller;
        _reloading = false;
    }
}
