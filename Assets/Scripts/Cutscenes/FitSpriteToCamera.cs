using UnityEngine;

[ExecuteAlways]
public class FitSpriteToCamera : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // assign (or auto-find)
    public Camera cam;                    // assign (or will use Camera.main)
    public bool fill = true;              // Fill (no letterbox). If false: Fit (may letterbox).

    void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    void Start() { Apply(); }
    void OnEnable() { Apply(); }
    void OnValidate() { Apply(); }

    public void Apply()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer || !spriteRenderer.sprite) return;
        if (!cam) cam = Camera.main;
        if (!cam || !cam.orthographic) return;

        var s = spriteRenderer.sprite;
        float ppu = s.pixelsPerUnit;
        Vector2 spriteWorld = s.rect.size / ppu;              // sprite size in world units
        float worldH = cam.orthographicSize * 2f;             // view height in world units
        float worldW = worldH * cam.aspect;                   // view width in world units

        float sx = worldW / spriteWorld.x;
        float sy = worldH / spriteWorld.y;
        float scale = fill ? Mathf.Max(sx, sy) : Mathf.Min(sx, sy);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}