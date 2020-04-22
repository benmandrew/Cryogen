using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public float maxHealth = 50f;
    public float healDelay = 3f;
    public float healRate = 5f;
    
    public float movementSpeed = 15f;
    public float accelerationRate = 0.2f;
    public float mouseSensitivity = 5f;
    public float jumpAmount = 6f;
    public float jumpControlFactor = 1f;
    public Rigidbody playerRigidbody;
    public Camera playerCamera;
    public Collider playerCollider;

    public Animator playerAnimator;

    public float damageFadeInTime = 0.2f;
    public float damageFadeOutTime = 2f;

    private Vignette vignetteEffect;
    private ColorAdjustments colourEffect;
    
    private const float MinX = 0f;
    private const float MaxX = 360f;
    private const float MinY = -90f;
    private const float MaxY = 90f;

    private const float LowerMovementSnapThreshold = 0.1f;
    
    private Vector3 _targetMoveDir;
    private Vector3 _velocity;
    
    private Vector2 _mousePos;
    
    private bool _pressedJump = false;
    // private bool _movementLocked = false;
    private float _currentHealth;
    
    private MoneyManager moneyManager;

    private bool _alive = true;

    private float _elapsedSinceDeath;

    private float _distToGround;
    private const float GroundDistThresh = 0.1f;

    private const float SweepTolerance = 0.1f;

    private Color vignetteDefault;
    private float intensityDefault;

    private float lastTookDamage;

    private bool _gameOver = false;

    // public bool locked
    // {
    //     get => _movementLocked;
    //     private set => _movementLocked = value;
    // }

    void Start()
    {
        moneyManager = GameObject.FindWithTag("MoneyManager").GetComponent<MoneyManager>();
        _currentHealth = maxHealth;

        _distToGround = playerCollider.bounds.extents.y;

        vignetteEffect = EffectsController.instance.vignetteEffect;
        colourEffect = EffectsController.instance.colourEffect;

        vignetteDefault = vignetteEffect.color.value;
        intensityDefault = vignetteEffect.intensity.value;

        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);
    }

    void Update()
    {
        if (!_alive || GameManager.instance.playerLocked)
        {
            return;
        }

        bool locked = GameManager.PlayerLocked();
        if (!locked)
        {
            _targetMoveDir.x = Input.GetAxisRaw("Horizontal");
            _targetMoveDir.y = 0f;
            _targetMoveDir.z = Input.GetAxisRaw("Vertical");
            _targetMoveDir.Normalize();
            
            _mousePos.x += Input.GetAxis("Mouse X") * mouseSensitivity + MaxX - MinX;
            _mousePos.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }
        else
        {
            _targetMoveDir = Vector3.zero;
        }

        _mousePos.x %= MaxX - MinX;
        _mousePos.x += MinX;

        _mousePos.y = Mathf.Clamp(_mousePos.y, MinY, MaxY);

        _pressedJump = Input.GetButton("Jump");

        if (Time.time - lastTookDamage > healDelay && _currentHealth < maxHealth)
        {
            _currentHealth = Mathf.Min(maxHealth, _currentHealth + healRate * Time.deltaTime);
            PlayerUIController.instance.SetPlayerHealthPercent(_currentHealth / maxHealth);
        }

        if (GameManager.instance.heartDead && !_gameOver)
        {
            _gameOver = true;
            StartCoroutine(HeartDeathAnim());
        }
    }

    public void AddMouseY(float amount)
    {
        _mousePos.y += amount;
        _mousePos.y = Mathf.Clamp(_mousePos.y, MinY, MaxY);
    }

    private void FixedUpdate()
    {
        // if (!_alive)
        // {
        //     return;
        // }

        var t = transform;
        // float a = -Mathf.Deg2Rad * t.eulerAngles.y;
        // float s = Mathf.Sin(a);
        // float c = Mathf.Cos(a);
        // Vector2 m = new Vector2(
        //     _moveDir.x * c - _moveDir.y * s,
        //     _moveDir.x * s + _moveDir.y * c
        //     ) * (movementSpeed * Time.fixedDeltaTime);
        // float accelerationAmount = accelerationRate;
        // if (_isJumping)
        // {
        //     accelerationAmount *= jumpControlFactor;
        // }
        // playerRigidbody.velocity = new Vector3(m.x, playerRigidbody.velocity.y, m.y);

        bool grounded = Grounded();
        Vector3 target = Vector3.zero;
        if (_alive)
        {
            target = (Quaternion.AngleAxis(t.eulerAngles.y, Vector3.up) * _targetMoveDir) * movementSpeed;
        }

        Vector3 delta = target - playerRigidbody.velocity;
        delta.y = 0;
        
        float dist = delta.magnitude;
        
        if (dist > LowerMovementSnapThreshold)
        {
            delta /= dist;
        
            float accelerationAmount = accelerationRate * Time.deltaTime;
            if (!grounded)
            {
                accelerationAmount *= jumpControlFactor;
            }
            // playerRigidbody.velocity += delta * Mathf.Min(accelerationAmount, dist);
            
            playerRigidbody.AddForce(delta * Mathf.Min(accelerationAmount, dist), ForceMode.VelocityChange);
        }
        else
        {
            // playerRigidbody.velocity = target + Vector3.up * playerRigidbody.velocity.y;
            
            playerRigidbody.AddForce(delta, ForceMode.VelocityChange);
        }

        // float vx = _velocity.x * Time.deltaTime * movementSpeed;
        // float vz = _velocity.z * Time.deltaTime * movementSpeed;
        //
        // t.position += t.forward * vz + t.right * vx;

        if (_pressedJump && grounded)
        {
            // playerRigidbody.velocity += Vector3.up * jumpAmount;
            playerRigidbody.AddForce(Vector3.up * jumpAmount, ForceMode.VelocityChange);
        }

        // RaycastHit hit;
        //
        // float frameMoveDist = playerRigidbody.velocity.magnitude;
        //
        // if (playerRigidbody.SweepTest(playerRigidbody.velocity / frameMoveDist, out hit,
        //     (frameMoveDist + SweepTolerance) * Time.fixedDeltaTime))
        // {
        //     playerRigidbody.velocity = new Vector3(0f, playerRigidbody.velocity.y, 0f);
        // }

        if (_alive)
        {
            t.rotation = Quaternion.Euler(0, _mousePos.x, 0f);
            playerCamera.transform.localRotation = Quaternion.Euler(_mousePos.y, 0f, 0f);
        }
    }

    private bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _distToGround + GroundDistThresh);
    }

    // public void LockMovement()
    // {
    //     locked = true;
    // }
    //
    // public void UnlockMovement()
    // {
    //     locked = false;
    // }

    public void Damage(float amount)
    {
        if (!_alive)
        {
            return;
        }

        lastTookDamage = Time.time;
        
        _currentHealth -= amount;
        
        PlayerUIController.instance.SetPlayerHealthPercent(_currentHealth / maxHealth);

        StopAllCoroutines();
        StartCoroutine(DamageEffect());

        if (_currentHealth < 0)
        {
            Die();
        }
    }

    public void notifyEnemyKilled(EnemyType type) {
        moneyManager.enemyKilled(type);
    }

    public void Die()
    {
        if (!_alive)
        {
            return;
        }
        
        // LockMovement();

        _alive = false;
        _gameOver = true;
        
        playerAnimator.SetTrigger("Switch");

        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        PlayerUIController.instance.crosshair.SetActive(false);
        
        Quaternion playerStartRotation = transform.rotation;
        Quaternion playerEndRotation = playerStartRotation * Quaternion.Euler(-90, 0, 0);

        Quaternion cameraStartRotation = playerCamera.transform.localRotation;
        Quaternion cameraEndRotation = Quaternion.identity;

        float elapsed = 0f;

        const float fallTime = 1f;

        while (elapsed < fallTime)
        {
            transform.rotation = Quaternion.Slerp(playerStartRotation, playerEndRotation, elapsed / fallTime);
            playerCamera.transform.localRotation =
                Quaternion.Slerp(cameraStartRotation, cameraEndRotation, elapsed / fallTime);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.rotation = playerEndRotation;
        playerCamera.transform.localRotation = cameraEndRotation;

        elapsed = 0f;

        const float fadeTime = 3f;

        float startVignetteIntensity = vignetteEffect.intensity.value;
        float endVignetteIntensity = 0.4f;
        Color startVignetteColour = vignetteEffect.color.value;
        Color endVignetteColour = Color.red;

        float startSaturation = colourEffect.saturation.value;
        float endSaturation = -80f;

        while (elapsed < fadeTime)
        {
            vignetteEffect.intensity.value =
                Mathf.Lerp(startVignetteIntensity, endVignetteIntensity, elapsed / fadeTime);
            vignetteEffect.color.value = Color.Lerp(startVignetteColour, endVignetteColour, elapsed / fadeTime);
            colourEffect.saturation.value = Mathf.Lerp(startSaturation, endSaturation, elapsed / fadeTime);

            elapsed += Time.unscaledTime;
            yield return null;
        }
        
        PlayerUIController.instance.deathScreen.SetActive(true);
        GameManager.UnlockCursor();
        // GameManager.instance.gamePaused = true;

        GameManager.instance.playerDead = true;
    }

    IEnumerator HeartDeathAnim()
    {
        float elapsed = 0f;

        const float fadeTime = 3f;

        float startVignetteIntensity = vignetteEffect.intensity.value;
        float endVignetteIntensity = 0.4f;
        Color startVignetteColour = vignetteEffect.color.value;
        Color endVignetteColour = Color.red;

        float startSaturation = colourEffect.saturation.value;
        float endSaturation = -80f;

        while (elapsed < fadeTime)
        {
            vignetteEffect.intensity.value =
                Mathf.Lerp(startVignetteIntensity, endVignetteIntensity, elapsed / fadeTime);
            vignetteEffect.color.value = Color.Lerp(startVignetteColour, endVignetteColour, elapsed / fadeTime);
            colourEffect.saturation.value = Mathf.Lerp(startSaturation, endSaturation, elapsed / fadeTime);

            Time.timeScale = Mathf.Lerp(1, 0.3f, elapsed / fadeTime);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        Die();
    }

    IEnumerator DamageEffect()
    {
        float elapsed = 0f;

        Color startVignette = vignetteEffect.color.value;
        Color targetColour = Color.red;

        float startIntensity = vignetteEffect.intensity.value;
        float finalIntensity = 0.4f;

        while (elapsed < damageFadeInTime)
        {
            vignetteEffect.color.value = Color.Lerp(startVignette, targetColour, elapsed / damageFadeInTime);
            vignetteEffect.intensity.value = Mathf.Lerp(startIntensity, finalIntensity, elapsed / damageFadeInTime);

            elapsed += Time.deltaTime;

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < damageFadeOutTime)
        {
            vignetteEffect.color.value = Color.Lerp(targetColour, vignetteDefault, elapsed / damageFadeOutTime);
            vignetteEffect.intensity.value = Mathf.Lerp(finalIntensity, intensityDefault, elapsed / damageFadeInTime);

            elapsed += Time.deltaTime;

            yield return null;
        }

        vignetteEffect.color.value = vignetteDefault;
        vignetteEffect.intensity.value = intensityDefault;
    }
}
