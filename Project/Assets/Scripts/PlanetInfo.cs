using UnityEngine;

[System.Serializable]
public class PlanetInfoData
{
    public string displayName;
    [TextArea] public string description;
    public float diameterKm;
    public int moons;
}

public class PlanetInfo : MonoBehaviour
{
    public PlanetInfoData info;

    public void ShowInfo()
    {
        // Hiển thị info – nếu bạn chưa muốn UI, có thể log ra console hoặc tạo 3D Text
        Debug.Log($"Planet: {info.displayName}\n{info.description}\nDiameter: {info.diameterKm} km\nMoons: {info.moons}");
        // TODO: gọi TTS để đọc out loud
    }
}
