using UnityEngine;

public class BoarChaseVisual : MonoBehaviour
// {
//     public Vector3 farPos;   // where it sits when distance is 0
//     public Vector3 nearPos;  // where it sits when distance is 1
//     public float smooth = 6f;
//
//     private void Update()
//     {
//         float t = GameManager.Instance.GetDistance01();
//         Vector3 target = Vector3.Lerp(farPos, nearPos, t);
//         transform.position = Vector3.Lerp(transform.position, target, smooth * Time.deltaTime);
//     }
// }



// NEW SCRIPT: BOAR LOCKED TO CAMERA; meet player half way

{
    [Header("Viewport X (0..1)")]
    public float farX = 0.86f;   // where boar sits when far
    public float nearX = 0.66f;  // where boar sits when near

    public float smooth = 6f;

    void LateUpdate()
    {
        var cam = Camera.main;
        if (!cam || !cam.orthographic) return;

        float t = GameManager.Instance.GetDistance01();
        float targetX = Mathf.Lerp(farX, nearX, t);

        float worldX = WorldXFromViewport(cam, targetX);

        Vector3 p = transform.position;
        p.x = Mathf.Lerp(p.x, worldX, Time.deltaTime * smooth);
        transform.position = p;
    }

    float WorldXFromViewport(Camera cam, float vx)
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        return cam.transform.position.x + (vx - 0.5f) * (halfW * 2f);
    }
}