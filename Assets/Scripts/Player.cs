using System;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Movement / Jump")]
    public float jumpForce = 8f;
    public LayerMask groundLayer;
    public float rayLength = 1.4f;

    [Header("Audio")]
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip hitClip;

    [Header("Stamina / Sprint")]
    public float staminaMax = 100f;
    public float sprintDrainPerSecond = 30f;
    public float staminaRegenPerSecond = 18f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public bool allowLeftMouseSprint = true;

    [Tooltip("If true, a short Space tap jumps; holding Space (>holdThreshold) sprints.")]
    public bool holdSpaceToSprint = false;
    public float holdThreshold = 0.15f; // seconds to treat Space as hold

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private int score;

    private float stamina;
    private bool isSprinting;
    private float spaceHeldTime;

    public float Stamina01 => Mathf.Clamp01(stamina / staminaMax);
    public bool IsSprinting => isSprinting;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stamina = staminaMax;
    }

    void Update()
    {
        // ground check
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
        // debug
        Debug.DrawRay(transform.position, Vector3.down * rayLength, isGrounded ? Color.green : Color.red, 0f);
        
        // Input: Jump (tap Space)
        HandleJump();

        // Input: Sprint (Shift / LMB / hold Space)
        HandleSprint();

        // Stamina drain/regen
        if (isSprinting && stamina > 0f)
        {
            stamina -= sprintDrainPerSecond * Time.deltaTime;
            if (stamina <= 0f) { stamina = 0f; isSprinting = false; }
        }
        else
        {
            stamina = Mathf.Min(staminaMax, stamina + staminaRegenPerSecond * Time.deltaTime);
        }

        // Notify GameManager
        GameManager.Instance.SetSprinting(isSprinting && stamina > 0f);
    }

    private void HandleJump()
    {
        // “Tap vs Hold Space” support without double-firing sprint
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceHeldTime = 0f; // start measuring a potential hold
            if (isGrounded) DoJump();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            spaceHeldTime += Time.deltaTime;
        }
    }

    private void HandleSprint()
    {
        bool sprintButton =
            Input.GetKey(sprintKey) ||
            (allowLeftMouseSprint && Input.GetMouseButton(0)) ||
            (holdSpaceToSprint && Input.GetKey(KeyCode.Space) && spaceHeldTime >= holdThreshold);

        // Only sprint on ground (feels better for a runner)
        isSprinting = sprintButton && isGrounded && stamina > 0f;
    }

    private void DoJump()
    {
        // clear vertical velocity then jump
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (jumpClip) AudioManager.instance?.PlaySFX(jumpClip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            // Hit an obstacle
            if (hitClip) AudioManager.instance?.PlaySFX(hitClip);
            GameManager.Instance.OnPlayerHitObstacle(); // lose or knockback based on toggle

            // Optional: brief grace period or tiny hop to sell impact
            // rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.4f);
        }
        else
        {
            if (isGrounded && landClip) AudioManager.instance?.PlaySFX(landClip);
        }
    }
}
