using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement / Jump")]
    public float jumpForce = 9f;
    public LayerMask groundLayer;
    public float rayLength = 1.4f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.08f;     // small forgiveness after leaving ground
    public float jumpBufferTime = 0.10f; // accept a press slightly early
    
    [Header("Better Jump")]
    [Tooltip("Extra gravity while FALLING, as a fraction of default gravity.")]
    public float extraFallPercent = 0f;  // 20% more gravity when falling   ; 0 for most airtime
    
    [Header("Audio")]
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip hitClip;

    [Header("Stamina / Sprint")]
    public float staminaMax = 100f;
    public float sprintDrainPerSecond = 30f;
    public float staminaRegenPerSecond = 18f;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public bool allowLeftMouseSprint = false; // keep off until stable
    public bool holdSpaceToSprint = false;
    public float holdThreshold = 0.15f;

    [Header("Sprint Input Guard")]
    public float sprintInputWarmup = 0.25f; // ignore sprint input right after load
    
    
    
    // refs
    Rigidbody2D rb;
    Animator animator;

    // state
    bool isGrounded;
    public bool IsGrounded => isGrounded;
    float stamina;
    bool isSprinting;
    float spaceHeldTime;

    // jump helpers
    float coyoteTimer;
    float bufferTimer;

    // sprint guards
    bool sprintReleasedSinceWarmup;
    int sprintBlockFrames = 2;

    public float Stamina01 => Mathf.Clamp01(stamina / staminaMax);
    public bool IsSprinting => isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stamina = staminaMax;
        sprintReleasedSinceWarmup = false;

        // (optional but recommended) rigidbody smoothing
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    void Update()
    {
        UpdateGrounded();
        HandleJumpInput();
        HandleSprintInput();

        // stamina
        if (isSprinting && stamina > 0f) stamina = Mathf.Max(0f, stamina - sprintDrainPerSecond * Time.deltaTime);
        else                              stamina = Mathf.Min(staminaMax, stamina + staminaRegenPerSecond * Time.deltaTime);

        // inform GameManager
        GameManager.Instance.SetSprinting(isSprinting && stamina > 0f);

        // sell speed visually via animator
        if (animator)
        {
            float mul = GameManager.Instance.ScrollSpeed / GameManager.Instance.baseScrollSpeed;
            animator.speed = Mathf.Clamp(mul, 0.8f, 1.6f);
        }
    }

    void UpdateGrounded()
    {
        // simple & reliable ray (like your original)
        Vector2 origin = transform.position;
        isGrounded = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
        
        
        Debug.DrawRay(origin, Vector3.down * rayLength, isGrounded ? Color.green : Color.red, 0f);

        // jump assist timers
        coyoteTimer = isGrounded ? coyoteTime : Mathf.Max(0f, coyoteTimer - Time.deltaTime);
        bufferTimer = Mathf.Max(0f, bufferTimer - Time.deltaTime);
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceHeldTime = 0f;
            bufferTimer = jumpBufferTime;
        }
        if (Input.GetKey(KeyCode.Space)) spaceHeldTime += Time.deltaTime;

        // buffered jump when coyote available
        if (bufferTimer > 0f && coyoteTimer > 0f)
        {
            DoJump();
            bufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }
    
    void FixedUpdate()
    {
        // Only add extra gravity on the way DOWN (keeps jump height intact)
        if (!isGrounded && rb.linearVelocity.y <= 0f)
        {
            var v = rb.linearVelocity;
            v.y += Physics2D.gravity.y * extraFallPercent * Time.fixedDeltaTime;
            rb.linearVelocity = v;
        }
    }
    
    void HandleSprintInput()
    {
        // prevent auto-sprint at scene start
        if (sprintBlockFrames > 0) { sprintBlockFrames--; isSprinting = false; return; }
        if (sprintInputWarmup > 0f) { sprintInputWarmup -= Time.unscaledDeltaTime; isSprinting = false; return; }

        // require one full release after warmup
        if (!sprintReleasedSinceWarmup)
        {
            if (!SprintHeldRaw()) sprintReleasedSinceWarmup = true;
            isSprinting = false;
            return;
        }

        bool held = SprintHeldRaw();
        isSprinting = held && isGrounded && stamina > 0f;
    }

    bool SprintHeldRaw()
    {
        return Input.GetKey(sprintKey)
            || (allowLeftMouseSprint && Input.GetMouseButton(0))
            || (holdSpaceToSprint && Input.GetKey(KeyCode.Space) && spaceHeldTime >= holdThreshold);
    }

    void DoJump()
    {
        // correct property for Rigidbody2D
        // rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        // rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        var v = rb.linearVelocity; // keep X, replace Y
        v.y = jumpForce;           // treat jumpForce as an upward speed
        rb.linearVelocity = v;

        if (jumpClip) AudioManager.instance?.PlaySFX(jumpClip);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            if (hitClip) AudioManager.instance?.PlaySFX(hitClip);
            GameManager.Instance.OnPlayerHitObstacle();
        }
        else
        {
            if (isGrounded && landClip) AudioManager.instance?.PlaySFX(landClip);
        }
    }
}


// using UnityEngine;
//
// public class Player : MonoBehaviour
// {
//     [Header("Movement / Jump")]
//     public float jumpForce = 8f;
//     public LayerMask groundLayer;
//
//     [Tooltip("Half-width scale of the feet box vs collider width (0.9 = 90% of width).")]
//     public float feetWidthScale = 0.9f;
//     [Tooltip("Feet box height in world units.")]
//     public float feetHeight = 0.06f;
//     [Tooltip("Extra check distance below feet.")]
//     public float feetExtraDown = 0.04f;
//
//     [Header("Jump Assist")]
//     public float coyoteTime = 0.08f;     // forgiveness after leaving ground
//     public float jumpBufferTime = 0.10f; // accept key press slightly early
//
//     [Header("Audio")]
//     public AudioClip jumpClip;
//     public AudioClip landClip;
//     public AudioClip hitClip;
//
//     [Header("Stamina / Sprint")]
//     public float staminaMax = 100f;
//     public float sprintDrainPerSecond = 30f;
//     public float staminaRegenPerSecond = 18f;
//     public KeyCode sprintKey = KeyCode.LeftShift;
//     public bool allowLeftMouseSprint = false;
//     public bool holdSpaceToSprint = false;
//     public float holdThreshold = 0.15f;
//
//     [Header("Sprint Input Guard")]
//     public float sprintInputWarmup = 0.25f; // ignore sprint input just after load
//
//     // refs
//     Rigidbody2D rb;
//     Animator animator;
//     Collider2D col;
//
//     // state
//     bool isGrounded;
//     float stamina;
//     bool isSprinting;
//     float spaceHeldTime;
//
//     // jump helpers
//     float coyoteTimer;
//     float bufferTimer;
//
//     // sprint guards
//     bool sprintReleasedSinceWarmup;
//     int sprintBlockFrames = 2;
//
//     public float Stamina01 => Mathf.Clamp01(stamina / staminaMax);
//     public bool IsSprinting => isSprinting;
//
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         animator = GetComponent<Animator>();
//         col = GetComponent<Collider2D>();
//         stamina = staminaMax;
//         sprintReleasedSinceWarmup = false;
//     }
//
//     void Update()
//     {
//         UpdateGrounded();
//         HandleJumpInput();
//         HandleSprintInput();
//
//         // stamina drain/regen
//         if (isSprinting && stamina > 0f) stamina = Mathf.Max(0f, stamina - sprintDrainPerSecond * Time.deltaTime);
//         else stamina = Mathf.Min(staminaMax, stamina + staminaRegenPerSecond * Time.deltaTime);
//
//         // notify world
//         GameManager.Instance.SetSprinting(isSprinting && stamina > 0f);
//
//         // optional: sell speed with anim speed
//         if (animator)
//         {
//             float mul = GameManager.Instance.ScrollSpeed / GameManager.Instance.baseScrollSpeed;
//             animator.speed = Mathf.Clamp(mul, 0.8f, 1.6f);
//         }
//     }
//
//     void UpdateGrounded()
//     {
//         // robust feet check using collider bounds
//         if (!col)
//         {
//             // fallback ray
//             isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.4f, groundLayer);
//             return;
//         }
//
//         var b = col.bounds;
//         Vector2 boxSize = new Vector2(b.size.x * Mathf.Clamp01(feetWidthScale), feetHeight);
//         Vector2 boxCenter = new Vector2(b.center.x, b.min.y - feetExtraDown * 0.5f);
//
//         // OverlapBox is stable and cheap
//         isGrounded = Physics2D.OverlapBox(boxCenter, boxSize, 0f, groundLayer) != null;
//
//         // debug viz (green = grounded, red = air)
//         Color c = isGrounded ? Color.green : Color.red;
//         Debug.DrawLine(new Vector3(boxCenter.x - boxSize.x * 0.5f, boxCenter.y, 0), new Vector3(boxCenter.x + boxSize.x * 0.5f, boxCenter.y, 0), c, 0f);
//         Debug.DrawLine(new Vector3(boxCenter.x - boxSize.x * 0.5f, boxCenter.y + boxSize.y, 0), new Vector3(boxCenter.x + boxSize.x * 0.5f, boxCenter.y + boxSize.y, 0), c, 0f);
//
//         // coyote & buffer timers
//         coyoteTimer = isGrounded ? coyoteTime : Mathf.Max(0f, coyoteTimer - Time.deltaTime);
//         bufferTimer = Mathf.Max(0f, bufferTimer - Time.deltaTime);
//     }
//
//     void HandleJumpInput()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             spaceHeldTime = 0f;
//             bufferTimer = jumpBufferTime; // remember the press briefly
//         }
//         if (Input.GetKey(KeyCode.Space)) spaceHeldTime += Time.deltaTime;
//
//         // consume buffered jump when coyote available
//         if (bufferTimer > 0f && coyoteTimer > 0f)
//         {
//             DoJump();
//             bufferTimer = 0f;
//             coyoteTimer = 0f;
//         }
//     }
//
//     void HandleSprintInput()
//     {
//         // block a couple frames & warmup (prevents auto-sprint on first click)
//         if (sprintBlockFrames > 0) { sprintBlockFrames--; isSprinting = false; return; }
//         if (sprintInputWarmup > 0f) { sprintInputWarmup -= Time.unscaledDeltaTime; isSprinting = false; return; }
//
//         // require one clean release after warmup
//         if (!sprintReleasedSinceWarmup)
//         {
//             if (!SprintHeldRaw()) sprintReleasedSinceWarmup = true;
//             isSprinting = false;
//             return;
//         }
//
//         bool sprintHeld = SprintHeldRaw();
//         isSprinting = sprintHeld && isGrounded && stamina > 0f;
//     }
//
//     bool SprintHeldRaw()
//     {
//         return Input.GetKey(sprintKey)
//             || (allowLeftMouseSprint && Input.GetMouseButton(0))
//             || (holdSpaceToSprint && Input.GetKey(KeyCode.Space) && spaceHeldTime >= holdThreshold);
//     }
//
//     void DoJump()
//     {
//         // CORRECT property for 2D RBs (do NOT use linearVelocity)
//         rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
//         rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//         if (jumpClip) AudioManager.instance?.PlaySFX(jumpClip);
//     }
//
//     void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.gameObject.CompareTag("Damage"))
//         {
//             if (hitClip) AudioManager.instance?.PlaySFX(hitClip);
//             GameManager.Instance.OnPlayerHitObstacle();
//         }
//         else
//         {
//             if (isGrounded && landClip) AudioManager.instance?.PlaySFX(landClip);
//         }
//     }
// }

