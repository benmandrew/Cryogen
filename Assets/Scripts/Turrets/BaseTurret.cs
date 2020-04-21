using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the base turret class that other turrets will inherit from
public class BaseTurret : MonoBehaviour
{

    [HideInInspector] public float turretHP;
    public float maxHP;
    [HideInInspector] List<GameObject> enemies;
    [HideInInspector] public List<GameObject> bulletSpawns;
    [HideInInspector] public List<GameObject> enemiesHit;
    [HideInInspector] public float cooldownTimer = 0;
    public float cooldown;
    public float rotateSpeed;
    GameObject turretHead;
    public float wpnDmg;
    Animation shootAnim;
    public Animation destroyed;
    [HideInInspector] bool playing = false;

    public ParticleSystem[] shootParticles;
    public AudioSource shootAudio;
    public AudioClip shootClip;

    private float timeCount = 0.0f;

    private Vector3 _startRot;
    private bool _startRotSet;
    private float _currentAngle;

    bool isIdle = true;
    float idleCounter = 2.0f;
    public Vector2 timeRange = new Vector2(1, 3);
    Vector3 randomPos = new Vector3(0, 0, 0);

    public void setHP()
    {
        turretHP = maxHP;
    }

    public void aimTurretAtEnemies()
    {
        if (!_startRotSet)
        {
            _startRot = turretHead.transform.localEulerAngles;
            _startRotSet = true;
        }

        findEnemies();
        if (enemies.Count > 0)
        {
            GameObject closestEnemy = findClosestEnemy();
            // Debug.Log(closestEnemy);

            // Vector3 targetDirection = closestEnemy.transform.position - transform.position;
            // Vector2 dist = new Vector2(targetDirection.x, targetDirection.z);
            // Vector2 forw = new Vector2(transform.forward.x, transform.forward.z);
            // int mult = -1;
            // if (dist.x > 0) 
            // {
            //     mult = 1;
            // }
            // float absA = Mathf.Sqrt((dist.x * dist.x) + (dist.y * dist.y));
            // float absB = Mathf.Sqrt((forw.x * forw.x) + (forw.y * forw.y));
            // float yVal = Mathf.Acos(Vector2.Dot(dist, forw) / (absA * absB)) * 180 / Mathf.PI;
            // Vector3 currentRot = new Vector3(-90, turretHead.transform.rotation.y, 0);
            // Vector3 targetRot = new Vector3(-90, mult * yVal, 0);
            // //print("Curr:" + currentRot.y);
            // // print("Next:" + targetRot.y);
            // turretHead.transform.rotation = Quaternion.Lerp(Quaternion.Euler(currentRot), Quaternion.Euler(targetRot), timeCount);
            // timeCount = timeCount + Time.deltaTime;

            if (closestEnemy != null)
            {
                isIdle = false;
                // Debug.Log("AIMING");
                doRotate(closestEnemy.transform.position);
            }
            else
            {
                isIdle = true;
            }
        }
        else
        {
            isIdle = true;
        }
        if (isIdle)
        {
            //Want to randomly rotate the turret towards a direction
            doRotate(randomPos);
            idleCounter -= Time.deltaTime;
            if (idleCounter < 0)
            {
                idleCounter = UnityEngine.Random.Range(timeRange.x, timeRange.y);
                randomPos = new Vector3(UnityEngine.Random.Range(-100, 100), 0, UnityEngine.Random.Range(-100, 100));
            }
        }
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            tryToFire();
        }
        if (turretHP <= 0)
        {
            if (destroyed != null && !playing)
            {
                destroyed.Play();
                playing = true;
            }
            else
            {
                if (destroyed == null)
                {
                    FindObjectOfType<TurretManager>().destroyTurret(this.gameObject);
                }
            }
            if (playing)
            {
                if (!destroyed.isPlaying)
                {
                    FindObjectOfType<TurretManager>().destroyTurret(this.gameObject);
                }
            }

        }
    }

    public void doRotate(Vector3 pos)
    {
        Vector3 targetDirection = pos - transform.position;

        float targetAngle = (-Mathf.Atan2(targetDirection.z, targetDirection.x) * Mathf.Rad2Deg + 270f) % 360f;

        float deltaAngle = targetAngle - _currentAngle;

        float shortestAngle = Mathf.Abs(deltaAngle);
        if (shortestAngle > 180f)
        {
            shortestAngle = 360f - shortestAngle;
        }

        _currentAngle = Mathf.LerpAngle(_currentAngle, targetAngle, rotateSpeed / shortestAngle * Time.deltaTime) + 360f;

        _currentAngle %= 360f;

        turretHead.transform.localEulerAngles = _startRot + Vector3.up * (_currentAngle + 180f);
    }

    public void findEnemies()
    {
        enemies = new List<GameObject>();
        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyArray)
        {
            enemies.Add(enemy);
        }

    }

    float findDistance(GameObject enemy)
    {
        return Vector3.Distance(this.transform.position, enemy.transform.position);
    }

    GameObject findClosestEnemy()
    {
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float dist = findDistance(enemy);
            // foreach (GameObject point in bulletSpawns)
            // {
            //     //Debug.Log("Shooting raycast");
            //     RaycastHit hit;
            //     if (Physics.Raycast(point.transform.position, enemy.transform.position - point.transform.position, out hit))
            //     {
            //         Debug.DrawRay(point.transform.position, (enemy.transform.position - point.transform.position) * 100, Color.red);
            //         // Debug.Log(hit.transform.name);
            //         if (hit.transform.CompareTag("Enemy"))
            //         {
            //             Debug.Log("ENEMY IS IN SIGHT!!!" + hit.transform.name);
            //             if (dist < shortestDistance) 
            //             {
            //                 shortestDistance = dist;
            //                 closest = enemy;
            //             }
            //         }
            //     }
            // }
            Ray ray = new Ray();
            ray.origin = transform.position + Vector3.up * 1.5f;
            ray.direction = (enemy.transform.position - ray.origin).normalized;

            //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

            float closestDistInRay = Mathf.Infinity;
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;

            foreach (RaycastHit hit in Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                if (hit.distance < closestDistInRay)
                {
                    closestDistInRay = hit.distance;
                    closestHit = hit;
                }
            }

            if (closestHit.distance < shortestDistance)
            {
                if (closestHit.transform.CompareTag("Enemy"))
                {
                    shortestDistance = closestDistInRay;
                    closest = enemy;
                }
            }
        }
        return closest;
    }

    public void setTurret(GameObject head)
    {
        turretHead = head;
        foreach (Transform child in turretHead.transform) if (child.CompareTag("BulletSpawn"))
            {
                bulletSpawns.Add(child.gameObject);
            }
    }

    public void setAnim(Animation anim)
    {
        shootAnim = anim;
    }

    void tryToFire()
    {
        // Debug.Log("Try to fire");
        // return;
        if (targetWithinRange())
        {
            // Debug.Log("Target in range");
            foreach (GameObject enemy in enemiesHit)
            {
                enemy.GetComponent<Enemy>().damage(wpnDmg);
            }
            enemiesHit = new List<GameObject>();
            if (shootAnim != null)
            {
                shootAnim.Play();
            }
            cooldownTimer = cooldown;

            if (shootAudio != null)
            {
                shootAudio.PlayOneShot(shootClip);
            }

            foreach (ParticleSystem system in shootParticles)
            {
                system.Emit(1);
            }
        }
    }

    bool targetWithinRange()
    {
        bool targetWithinRange = false;
        foreach (GameObject point in bulletSpawns)
        {
            RaycastHit hit;
            if (Physics.Raycast(point.transform.position, point.transform.up * -1, out hit))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    //Debug.Log("HIT: " + hit.transform.gameObject);
                    enemiesHit.Add(hit.transform.gameObject);
                    targetWithinRange = true;
                }
            }
        }
        return targetWithinRange;
    }

    public void takeDamage(float amount)
    {
        turretHP -= amount;
        if (turretHP <= 0) 
        {
            destroyTurret();
        }
    }

    public void setCooldown()
    {
        cooldownTimer = cooldown;
    }

    public float getMaxHP()
    {
        return maxHP;
    }

    public float getHP()
    {
        return turretHP;
    }

    public void destroyTurret() 
    {
        FindObjectOfType<TurretManager>().destroyTurret(this.gameObject);
    }
}
