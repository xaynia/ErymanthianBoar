using UnityEngine;

public class BoarChaseVisual : MonoBehaviour
{
    public Vector3 farPos;   // where it sits when distance is 0
    public Vector3 nearPos;  // where it sits when distance is 1
    public float smooth = 6f;

    private void Update()
    {
        float t = GameManager.Instance.GetDistance01();
        Vector3 target = Vector3.Lerp(farPos, nearPos, t);
        transform.position = Vector3.Lerp(transform.position, target, smooth * Time.deltaTime);
    }
}
