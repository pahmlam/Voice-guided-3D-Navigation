using UnityEngine;

public class FixedAngleCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    private Vector3 _offset;
    private float _initialXRotation; // Lưu lại góc nghiêng ban đầu

    void Start()
    {
        if (target != null)
        {
            // 1. Lưu khoảng cách vị trí ban đầu
            _offset = transform.position - target.position;

            // 2. Lưu lại góc nghiêng (độ chúi) ban đầu của Camera
            // Ví dụ: Bạn để camera chúi xuống 30 độ, nó sẽ nhớ số 30 này mãi mãi
            _initialXRotation = transform.eulerAngles.x;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- XỬ LÝ VỊ TRÍ (POSITION) ---
        // Chỉ lấy góc xoay ngang (Y) của nhân vật
        Quaternion currentRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);

        // Tính vị trí mới dựa trên góc xoay đó
        Vector3 desiredPosition = target.position + (currentRotation * _offset);

        // Di chuyển camera đến vị trí đó
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);


        // --- XỬ LÝ GÓC XOAY (ROTATION) - KHÔNG DÙNG LOOKAT ---
        // Tạo ra một góc xoay mới:
        // - X (Góc nghiêng): Lấy lại góc cũ (_initialXRotation) -> CỐ ĐỊNH GÓC DỌC
        // - Y (Góc ngang): Lấy theo nhân vật (target.eulerAngles.y) -> XOAY THEO NHÂN VẬT
        // - Z (Góc nghiêng đầu): Luôn là 0
        Quaternion newRotation = Quaternion.Euler(_initialXRotation, target.eulerAngles.y, 0);

        // Áp dụng góc xoay này cho camera (có làm mượt nhẹ để đỡ giật)
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, smoothSpeed);
    }
}