using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ObjectRegistry : MonoBehaviour
{
    List<ObjectAnchor> anchors;

    void Awake()
    {
        anchors = Object.FindObjectsByType<ObjectAnchor>(FindObjectsSortMode.None).ToList();
    }

    public ObjectAnchor FindMatch(string text, Transform player)
    {
        var matches = anchors.Where(a => a.Matches(text)).ToList();
        if (matches.Count == 0) return null;

        return matches.OrderBy(a =>
            Vector3.Distance(a.transform.position, player.position)
        ).First();
    }
}
