using UnityEngine;
using System.Linq;

public class VoiceCommandProcessor : MonoBehaviour
{
    public ObjectRegistry registry;
    public NavigationController navigator;
    public Transform player;

    string[] goPatterns = { "go to", "move to", "go near", "approach" };

    public void Process(string text)
    {
        text = text.ToLower();
        Debug.Log("[VoiceCommand] " + text);

        // STOP command
        if (text.Contains("stop"))
        {
            navigator.Stop();
            return;
        }

        // CHECK IF TEXT CONTAINS ANY "GO" PATTERN
        bool isGo = goPatterns.Any(p => text.Contains(p));

        if (!isGo)
        {
            foreach (var anchor in Object.FindObjectsByType<ObjectAnchor>(FindObjectsSortMode.None))
            {
                if (anchor.Matches(text))
                {
                    var pos = anchor.GetFrontPosition();
                    navigator.GoTo(pos, anchor.transform);
                    return;
                }
            }

            return;
        }

        // TÌM OBJECT KHỚP TỪ KHÓA
        var match = registry.FindMatch(text, player);

        if (match == null)
        {
            Debug.Log("No matching object.");
            return;
        }

        var targetPos = match.GetFrontPosition();
        navigator.GoTo(targetPos, match.transform);
    }
}
