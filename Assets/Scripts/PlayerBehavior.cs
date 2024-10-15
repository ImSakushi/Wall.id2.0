using System.Collections; // Ajouté pour les coroutines non génériques
using System.Collections.Generic;
using UnityEngine;

public enum CardinalDirections { CARDINAL_S, CARDINAL_N, CARDINAL_W, CARDINAL_E };

public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float m_speed = 5f;
    public Animator animator;
    public AudioSource shootingAudioSource;
    public AudioSource dashAudioSource;

    [Header("Dash Audio Clips")]
    public AudioClip[] dashClips;

    public GameObject turretBoisPrefab;
    public GameObject turretPierrePrefab;
    public GameObject turretMetalPrefab;
    private bool canInput = true;

    public GameObject signPrefab;
    public GameObject signPanel;
    private bool hasDisplayedMessage = false;

    public BoisDisplayUI BoisReference;
    public PierreDisplayUI PierreReference;
    public MetalDisplayUI MetalReference;

    public float fireballCooldown = 2.0f;
    private float timeSinceLastFireball = 0.0f;

    private CardinalDirections m_direction;

    public Sprite m_frontSprite = null;
    public Sprite m_leftSprite = null;
    public Sprite m_rightSprite = null;
    public Sprite m_backSprite = null;

    [Header("Missiles")]
    public GameObject m_fireBall = null;
    public GameObject m_droiteBall = null;
    public GameObject m_map = null;
    public DialogManager m_dialogDisplayer;
    private Dialog m_closestNPCDialog;
    private Rigidbody2D m_rb2D;
    private SpriteRenderer m_renderer;

    [Header("Dash Settings")]
    public float dashSpeedMultiplier = 2.0f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    private float dashCooldownTimer = 0.0f;
    private bool isDashing = false;
    private float dashTimeRemaining = 0.0f;
    private Vector2 dashDirection = Vector2.zero;

    [Header("Squash Effect")]
    private Vector3 originalScale;
    public Vector3 horizontalDashScale = new Vector3(1.2f, 0.8f, 1f);
    public Vector3 verticalDashScale = new Vector3(0.8f, 1.2f, 1f);
    // Nouvelle échelle pour les diagonales
    public Vector3 diagonalDashScale = new Vector3(1.0f, 1.0f, 1f); // Ajustez les valeurs selon vos besoins
    public float squashSpeed = 0.05f;

    [Header("Shooting Sounds")]
    public AudioClip fireballFiringSound;
    public AudioClip droiteFiringSound;

    [Header("Droite Missile Settings")]
    public float droiteCooldown = 3.0f;
    private float timeSinceLastDroite = 0.0f;

    [Header("Ghost Effect Settings")]
    private TrailRenderer trailRenderer;

    [Header("Camera Settings")]
    public CameraZoom cameraZoom;

    [Header("Glow Settings")]
    public SpriteRenderer glowRenderer;
    public float glowRadius = 1.5f;
    private Camera mainCamera;

    [Header("Dash Effects")]
    public GameObject smokeTrailPrefab; // Ajouté

    void Awake()
    {
        mainCamera = Camera.main;
        m_rb2D = GetComponent<Rigidbody2D>();
        m_renderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer != null) trailRenderer.enabled = false;
        if (cameraZoom == null) Debug.LogWarning("CameraZoom non assigné dans PlayerBehavior.");
        if (glowRenderer == null) Debug.LogWarning("Glow Renderer is not assigned dans PlayerBehavior.");
    }

    void FixedUpdate()
    {
        if (m_dialogDisplayer.IsOnScreen() || m_map.activeSelf) return;
        Move();
        if (isDashing)
        {
            dashTimeRemaining -= Time.fixedDeltaTime;
            if (dashTimeRemaining <= 0)
            {
                isDashing = false;
                dashDirection = Vector2.zero;
                StopAllCoroutines();
                StartCoroutine(ResetScaleSmoothly());
                if (trailRenderer != null) trailRenderer.enabled = false;
            }
        }
    }

    private void Move()
    {
        if (!canInput) return;
        float horizontalOffset = 0f;
        float verticalOffset = 0f;
        bool isMovementKeyPressed = false;

        if (Input.GetKey(KeyCode.Z)) { verticalOffset += 1f; isMovementKeyPressed = true; }
        if (Input.GetKey(KeyCode.S)) { verticalOffset -= 1f; isMovementKeyPressed = true; }
        if (Input.GetKey(KeyCode.D)) { horizontalOffset += 1f; isMovementKeyPressed = true; }
        if (Input.GetKey(KeyCode.Q)) { horizontalOffset -= 1f; isMovementKeyPressed = true; }

        Vector2 movementVector = new Vector2(horizontalOffset, verticalOffset).normalized;

        if (isDashing && dashDirection != Vector2.zero && !isMovementKeyPressed)
            movementVector = dashDirection.normalized;

        bool isMoving = movementVector != Vector2.zero;

        animator.SetFloat("moveX", movementVector.x);
        animator.SetFloat("moveY", movementVector.y);
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            if (Mathf.Abs(movementVector.x) > Mathf.Abs(movementVector.y))
                m_direction = movementVector.x > 0 ? CardinalDirections.CARDINAL_E : CardinalDirections.CARDINAL_W;
            else
                m_direction = movementVector.y > 0 ? CardinalDirections.CARDINAL_N : CardinalDirections.CARDINAL_S;
        }
        else
        {
            switch (m_direction)
            {
                case CardinalDirections.CARDINAL_E:
                    animator.SetFloat("moveX", 1f);
                    animator.SetFloat("moveY", 0f);
                    break;
                case CardinalDirections.CARDINAL_W:
                    animator.SetFloat("moveX", -1f);
                    animator.SetFloat("moveY", 0f);
                    break;
                case CardinalDirections.CARDINAL_N:
                    animator.SetFloat("moveX", 0f);
                    animator.SetFloat("moveY", 1f);
                    break;
                case CardinalDirections.CARDINAL_S:
                    animator.SetFloat("moveX", 0f);
                    animator.SetFloat("moveY", -1f);
                    break;
            }
        }

        float currentSpeed = isDashing ? m_speed * dashSpeedMultiplier : m_speed;
        if (isMoving)
        {
            Vector2 movement = movementVector * currentSpeed * Time.fixedDeltaTime;
            m_rb2D.MovePosition(m_rb2D.position + movement);
        }
    }

    private void Update()
    {
        if (!canInput || m_dialogDisplayer.IsOnScreen() || m_map.activeSelf || signPanel.activeSelf)
        {
            SetIdleAnimation();
            return;
        }

        dashCooldownTimer = Mathf.Max(0, dashCooldownTimer - Time.deltaTime);
        timeSinceLastDroite = Mathf.Max(0, timeSinceLastDroite - Time.deltaTime);
        timeSinceLastFireball = Mathf.Min(fireballCooldown, timeSinceLastFireball + Time.deltaTime);

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && dashCooldownTimer <= 0 && !isDashing)
            StartDash();

        if (signPanel.activeSelf && canInput)
            canInput = false;
        else if (!signPanel.activeSelf && !canInput)
            canInput = true;

        if (!canInput || m_dialogDisplayer.IsOnScreen() || m_map.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        ChangeSpriteToMatchDirection();

        if (Input.GetMouseButtonDown(0) && timeSinceLastFireball >= fireballCooldown)
        {
            ShootProjectile(m_fireBall, fireballFiringSound);
            timeSinceLastFireball = 0.0f;
        }

        if (Input.GetMouseButtonDown(1) && timeSinceLastDroite <= 0)
        {
            ShootProjectile(m_droiteBall, droiteFiringSound);
            timeSinceLastDroite = droiteCooldown;
        }

        if (m_closestNPCDialog != null && Input.GetKeyDown(KeyCode.Return))
            m_dialogDisplayer.SetDialog(m_closestNPCDialog.GetDialog());

        if (Input.GetKeyDown(KeyCode.B) && (BoisReference.nombreBois >= 2))
            PlaceSign();

        if (Input.GetKeyDown(KeyCode.N))
            PlaceTurret();

        if ((BoisReference.nombreBois >= 10 || PierreReference.nombrePierre >= 10 || MetalReference.nombreMetal >= 10) && !hasDisplayedMessage)
        {
            m_dialogDisplayer.SetDialog(new List<DialogPage> {
                new DialogPage { text = "Tu devrais utiliser ces ressources en faisant une tourelle en appuyant sur [N] !", color = Color.black }
            });
            hasDisplayedMessage = true;
        }

        UpdateGlowPosition();
    }

    private void StartDash()
    {
        bool isMovingDuringDash = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        if (!isMovingDuringDash)
        {
            dashDirection = m_direction switch
            {
                CardinalDirections.CARDINAL_E => Vector2.right,
                CardinalDirections.CARDINAL_W => Vector2.left,
                CardinalDirections.CARDINAL_N => Vector2.up,
                CardinalDirections.CARDINAL_S => Vector2.down,
                _ => Vector2.right,
            };
        }
        else
        {
            // Si le joueur change de direction pendant le dash, utilisez la nouvelle direction
            float horizontal = 0f;
            float vertical = 0f;
            if (Input.GetKey(KeyCode.Z)) vertical += 1f;
            if (Input.GetKey(KeyCode.S)) vertical -= 1f;
            if (Input.GetKey(KeyCode.D)) horizontal += 1f;
            if (Input.GetKey(KeyCode.Q)) horizontal -= 1f;
            Vector2 newDirection = new Vector2(horizontal, vertical).normalized;
            if (newDirection != Vector2.zero)
                dashDirection = newDirection;
            else
                dashDirection = Vector2.zero;
        }

        // Instancier la traînée de fumée
        InstantiateSmokeTrail();

        isDashing = true;
        dashTimeRemaining = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Déterminer si le dash est diagonal
        bool isDiagonal = Mathf.Abs(dashDirection.x) > 0 && Mathf.Abs(dashDirection.y) > 0;

        Vector3 targetScale;
        if (isDiagonal)
        {
            targetScale = diagonalDashScale;
        }
        else if (dashDirection.x != 0)
        {
            targetScale = horizontalDashScale;
        }
        else
        {
            targetScale = verticalDashScale;
        }

        StartCoroutine(ApplySquashSmoothly(targetScale));
        PlayRandomDashSound();
        if (trailRenderer != null) trailRenderer.enabled = true;
        if (timeSinceLastFireball < fireballCooldown) timeSinceLastFireball = fireballCooldown;
        cameraZoom?.TriggerZoom();
    }

    private void InstantiateSmokeTrail()
    {
        if (smokeTrailPrefab == null)
        {
            Debug.LogWarning("SmokeTrailPrefab n'est pas assigné dans l'inspecteur.");
            return;
        }

        Vector3 spawnPosition = transform.position;

        // Déterminer la rotation en fonction de la direction du dash
        float rotationZ = 0f;
        if (dashDirection != Vector2.zero)
        {
            rotationZ = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg + 180f; // Ajusté pour l'orientation
        }
        else
        {
            // Si dashDirection est zéro, utilisez la direction actuelle du personnage
            rotationZ = m_direction switch
            {
                CardinalDirections.CARDINAL_N => 90f,
                CardinalDirections.CARDINAL_E => 180f,
                CardinalDirections.CARDINAL_S => 270f,
                CardinalDirections.CARDINAL_W => 0f,
                _ => 0f,
            };
        }

        Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);
        Instantiate(smokeTrailPrefab, spawnPosition, rotation);
    }

    private void PlayRandomDashSound()
    {
        if (dashClips.Length > 0 && dashAudioSource != null)
            dashAudioSource.PlayOneShot(dashClips[Random.Range(0, dashClips.Length)]);
        else
            Debug.LogWarning("Dash clips non assignés ou dashAudioSource est manquant.");
    }

    private IEnumerator ApplySquashSmoothly(Vector3 targetScale)
    {
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;
        while (elapsed < squashSpeed)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / squashSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }

    private IEnumerator ResetScaleSmoothly()
    {
        float elapsed = 0f;
        Vector3 initialScale = transform.localScale;
        while (elapsed < squashSpeed)
        {
            transform.localScale = Vector3.Lerp(initialScale, originalScale, elapsed / squashSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }

    private void ShootProjectile(GameObject projectilePrefab, AudioClip firingSound)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab est non assigné.");
            return;
        }
        GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        FireBehavior projectileBehavior = newProjectile.GetComponent<FireBehavior>();
        if (projectileBehavior != null)
        {
            if (firingSound != null) shootingAudioSource.PlayOneShot(firingSound);
            else Debug.LogWarning("Firing sound est non assigné.");
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            Vector2 direction = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            projectileBehavior.Launch(direction);
            projectileBehavior.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            Debug.LogError("FireBehavior script n'est pas trouvé sur le prefab du projectile.");
        }
    }

    void PlaceSign()
    {
        BoisReference.nombreBois -= 2;
        BoisReference.UpdateWoodDisplay();
        signPanel.SetActive(true);
    }

    private void SetIdleAnimation()
    {
        animator.SetBool("IsMoving", false);
        switch (m_direction)
        {
            case CardinalDirections.CARDINAL_N:
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 1);
                break;
            case CardinalDirections.CARDINAL_S:
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", -1);
                break;
            case CardinalDirections.CARDINAL_E:
                animator.SetFloat("moveX", 1);
                animator.SetFloat("moveY", 0);
                break;
            case CardinalDirections.CARDINAL_W:
                animator.SetFloat("moveX", -1);
                animator.SetFloat("moveY", 0);
                break;
        }
    }

    void PlaceTurret()
    {
        Vector3 offset = GetDirectionOffset();
        GameObject newTurret = null;

        if (MetalReference.nombreMetal >= 10)
        {
            newTurret = Instantiate(turretMetalPrefab, transform.position + offset, Quaternion.identity);
            MetalReference.nombreMetal -= 10;
            MetalReference.UpdateIronDisplay();
        }
        else if (PierreReference.nombrePierre >= 10)
        {
            newTurret = Instantiate(turretPierrePrefab, transform.position + offset, Quaternion.identity);
            PierreReference.nombrePierre -= 10;
            PierreReference.UpdateStoneDisplay();
        }
        else if (BoisReference.nombreBois >= 10)
        {
            newTurret = Instantiate(turretBoisPrefab, transform.position + offset, Quaternion.identity);
            BoisReference.nombreBois -= 10;
            BoisReference.UpdateWoodDisplay();
        }
        else
        {
            m_dialogDisplayer.SetDialog(new List<DialogPage>
            {
                new DialogPage { text = "Vous n'avez pas assez de ressources. Il en faut au moins 10.", color = Color.black }
            });
            return;
        }

        TurretController turretController = newTurret.GetComponent<TurretController>();
        if (turretController != null)
        {
            Vector2 directionVector = GetDirectionVector();
            turretController.SetDirection(directionVector);
        }
    }

    // Méthode de conversion de CardinalDirections en Vector2
    private Vector2 GetDirectionVector()
    {
        return m_direction switch
        {
            CardinalDirections.CARDINAL_N => Vector2.up,
            CardinalDirections.CARDINAL_S => Vector2.down,
            CardinalDirections.CARDINAL_E => Vector2.right,
            CardinalDirections.CARDINAL_W => Vector2.left,
            _ => Vector2.right, // Valeur par défaut
        };
    }

    Vector3 GetDirectionOffset()
    {
        float offsetDistance = 1.0f;
        return m_direction switch
        {
            CardinalDirections.CARDINAL_N => Vector3.up * offsetDistance,
            CardinalDirections.CARDINAL_S => Vector3.down * offsetDistance,
            CardinalDirections.CARDINAL_E => Vector3.right * offsetDistance,
            CardinalDirections.CARDINAL_W => Vector3.left * offsetDistance,
            _ => Vector3.right * offsetDistance,
        };
    }

    private void ChangeSpriteToMatchDirection()
    {
        m_renderer.sprite = m_direction switch
        {
            CardinalDirections.CARDINAL_N => m_backSprite,
            CardinalDirections.CARDINAL_S => m_frontSprite,
            CardinalDirections.CARDINAL_E => m_rightSprite,
            CardinalDirections.CARDINAL_W => m_leftSprite,
            _ => m_renderer.sprite,
        };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
            m_closestNPCDialog = collision.GetComponent<Dialog>();
        else if (collision.CompareTag("InstantDialog"))
            m_dialogDisplayer.SetDialog(collision.GetComponent<Dialog>()?.GetDialog());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
            m_closestNPCDialog = null;
        else if (collision.CompareTag("InstantDialog"))
            Destroy(collision.gameObject);
    }

    private void UpdateGlowPosition()
    {
        if (glowRenderer == null || mainCamera == null) return;
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector2 direction = (mousePosition - transform.position).normalized;
        glowRenderer.transform.localPosition = direction * glowRadius;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        glowRenderer.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
