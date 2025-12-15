using UnityEngine;
using System.Linq;

public class VoiceCommandProcessor : MonoBehaviour
{
    public ObjectRegistry registry;
    public NavigationController navigator;
    public Transform player;

    // UI in kết quả
    public ConsoleUI consoleUI;

    string[] goPatterns = { "go to", "move to", "go near", "approach" };

    public void Process(string text)
    {
        text = text.ToLower().Trim();
        Debug.Log("[VoiceCommand] " + text);

        // Bỏ noise: "i", "uh", "huh", "the", v.v.
        if (text.Length <= 2)
            return;

        // STOP
        if (text.Contains("stop"))
        {
            consoleUI?.AddMessage("stop");
            navigator.Stop();
            return;
        }

        // Kiểm tra có chứa "go to" không?
        bool isGo = goPatterns.Any(p => text.Contains(p));

        // CASE 1 — Không có go to, nhưng trùng tên object
        if (!isGo)
        {
            foreach (var anchor in Object.FindObjectsByType<ObjectAnchor>(FindObjectsSortMode.None))
            {
                if (anchor.Matches(text))
                {
                    // Thay anchorName bằng tên object
                    consoleUI?.AddMessage(anchor.gameObject.name);

                    var pos = anchor.GetFrontPosition();
                    navigator.GoTo(pos, anchor.transform);
                    return;
                }
            }

            return;
        }

        // CASE 2 — Có "go to ...": tìm đối tượng
        var match = registry.FindMatch(text, player);

        if (match == null)
        {
            Debug.Log("No matching object.");
            return;
        }

        // Dùng tên GameObject để hiển thị cho UI
        consoleUI?.AddMessage(match.gameObject.name);

        var targetPos = match.GetFrontPosition();
        navigator.GoTo(targetPos, match.transform);
    }
}
