using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerChaseViewport : MonoBehaviour
{
    [Header("Viewport X (0..1)")]
    public float farX  = 0.92f;      // where Herc sits when distance=0 (left-ish)
    public float nearX = 0.1f;      // where he sits when distance=1 (slightly right of center)

    [Header("Feel")]
    [Tooltip("How quickly we correct to the target X (seconds to remove ~63% of the error).")]
    public float followTime = 0.18f; // 0.15–0.25 feels good
    public float maxXSpeed  = 7f;    // clamp horizontal speed
    public float sprintNudge = 0.06f;
    public bool snapOnEnable = true;

    [Header("Refs")]
    public Player player;            // optional; will auto-find
    
    // [Header("Air Carry")]
    // [Tooltip("Extra forward speed while airborne to extend leap length.")]
    // public float airCarryVx = 1.2f;   // 0 = off; 1.0–2.0 feels good
    
    [Header("Air Nudge")]
    [Range(0f, 0.2f)] public float airNudge = 0.20f; // 0.04–0.10


    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!player) player = GetComponent<Player>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;
    }

    void OnEnable()
    {
        if (snapOnEnable && GameManager.Instance) SnapTo(GameManager.Instance.GetDistance01());
    }

    void FixedUpdate()
    {
        var cam = Camera.main;
        if (!cam || !cam.orthographic || GameManager.Instance == null) return;
        

        
        // 0..1 distance → viewport X → world X target
        float t = GameManager.Instance.GetDistance01();
        if (player && player.IsSprinting) t = Mathf.Clamp01(t + sprintNudge);
        //new
        if (player && !player.IsGrounded) t = Mathf.Clamp01(t + airNudge);
        float targetX = WorldXFromViewport(cam, Mathf.Lerp(farX, nearX, t));

        // Desired horizontal speed to close the error within followTime
        float error     = targetX - rb.position.x;
        float desiredVx = Mathf.Clamp(error / Mathf.Max(0.001f, followTime), -maxXSpeed, maxXSpeed);

        // Smoothly steer current vx toward desired; leave vy entirely alone
        var v  = rb.linearVelocity;              // Unity 6 API
        float a = 1f - Mathf.Exp(-10f * Time.fixedDeltaTime);
        v.x = Mathf.Lerp(v.x, desiredVx, a);
        rb.linearVelocity = v;
        
        
    }

    float WorldXFromViewport(Camera cam, float vx)
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        return cam.transform.position.x + (vx - 0.5f) * (halfW * 2f);
    }

    void SnapTo(float t)
    {
        var cam = Camera.main; if (!cam || !cam.orthographic) return;
        float x = WorldXFromViewport(cam, Mathf.Lerp(farX, nearX, t));
        rb.position = new Vector2(x, rb.position.y);
        // reset horizontal speed so we don't lurch after snap
        var v = rb.linearVelocity; v.x = 0f; rb.linearVelocity = v;
    }
}