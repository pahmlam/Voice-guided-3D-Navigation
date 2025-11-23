using UnityEngine;

public class RotationController : MonoBehaviour
{
    public static RotationController Instance;
    public Transform planetsRoot;
    public float globalSpeed = 1.0f; // mặc định

    void Awake() { Instance = this; }

    void Update()
    {
        // Quay từng hành tinh quanh mặt trời (giả lập đơn giản)
        foreach (Transform p in planetsRoot)
        {
            // quay quanh trục riêng
            p.Rotate(Vector3.up, 10f * Time.deltaTime * globalSpeed, Space.Self);
            // nếu muốn quay quanh mặt trời, parent cấu trúc riêng sẽ handle
        }
    }

    public void ChangeSpeed(float factor)
    {
        globalSpeed *= factor;
        Debug.Log("New speed: " + globalSpeed);
    }
}
