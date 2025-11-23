using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Camera mainCam;
    public float zoomSpeed = 40f;
    public float moveDuration = 1.0f;
    public Transform planetsRoot;

    private Coroutine moveCoroutine;

    void Awake()
    {
        Instance = this;
        if (mainCam == null) mainCam = Camera.main;
    }

    public void ZoomIn()
    {
        StopMove();
        float targetFov = Mathf.Max(10f, mainCam.fieldOfView - 10f);
        StartCoroutine(ChangeFOV(mainCam.fieldOfView, targetFov, 0.6f));
    }
    public void ZoomOut()
    {
        StopMove();
        float targetFov = Mathf.Min(90f, mainCam.fieldOfView + 10f);
        StartCoroutine(ChangeFOV(mainCam.fieldOfView, targetFov, 0.6f));
    }

    IEnumerator ChangeFOV(float from, float to, float dur)
    {
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            mainCam.fieldOfView = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        mainCam.fieldOfView = to;
    }

    public void MoveToPlanetByName(string planetName)
    {
        // map names (normalize)
        Transform target = FindPlanetTransform(planetName);
        if (target != null)
        {
            Vector3 targetPos = target.position + (target.forward * 2.0f) + new Vector3(0, 1.0f, 0); // offset
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveToPosition(mainCam.transform, targetPos, Quaternion.LookRotation(target.position - targetPos), moveDuration));
        }
        else
        {
            Debug.Log("Planet not found: " + planetName);
        }
    }

    Transform FindPlanetTransform(string name)
    {
        // Normalize name variants
        name = name.ToLower();
        foreach (Transform p in planetsRoot)
        {
            if (p.name.ToLower().Contains(name) || p.gameObject.tag.ToLower().Contains(name)) return p;
        }
        return null;
    }

    IEnumerator MoveToPosition(Transform camT, Vector3 pos, Quaternion rot, float dur)
    {
        Vector3 startPos = camT.position;
        Quaternion startRot = camT.rotation;
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
            camT.position = Vector3.Lerp(startPos, pos, t / dur);
            camT.rotation = Quaternion.Slerp(startRot, rot, t / dur);
            yield return null;
        }
        camT.position = pos;
        camT.rotation = rot;
        // Khi tới đích, gọi hiển thị thông tin
        PlanetInfo pi = camT.GetComponentInChildren<PlanetInfo>(); // optional
    }

    void StopMove()
    {
        if (moveCoroutine != null) { StopCoroutine(moveCoroutine); moveCoroutine = null; }
    }
}
