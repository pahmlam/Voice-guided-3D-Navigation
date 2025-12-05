using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    [Header("Cài đặt")]
    public float sensitivity = 500f; // Tốc độ xoay, chỉnh to nhỏ tùy ý

    void Start()
    {
        // Ẩn con trỏ chuột và khóa nó vào giữa màn hình
        // Để khi bạn di chuột liên tục không bị văng ra ngoài cửa sổ game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Lấy tín hiệu di chuột theo chiều ngang (Trục X của chuột)
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // 2. Xoay nhân vật quanh trục Y (trục thẳng đứng)
        // Vector3.up tương đương với trục Y (0, 1, 0)
        transform.Rotate(Vector3.up * mouseX);
    }
}