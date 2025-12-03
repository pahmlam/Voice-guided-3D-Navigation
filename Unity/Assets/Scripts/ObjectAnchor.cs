using UnityEngine;

public class ObjectAnchor : MonoBehaviour
{
    public string[] keywords;       // ["table", "desk"]
    public float frontOffset = 1f;  // khoảng đứng trước object

    public Transform frontMarker;   // để trỏ thủ công, nếu cần

    public Vector3 GetFrontPosition()
    {
        if (frontMarker != null)
            return frontMarker.position;

        // mặc định đứng phía trước theo hướng forward
        Vector3 pos = transform.position + transform.forward * frontOffset;
        pos.y = transform.position.y;
        return pos;
    }

    public bool Matches(string text)
    {
        text = text.ToLower();
        foreach (var k in keywords)
            if (text.Contains(k.ToLower()))
                return true;
        return false;
    }
}
