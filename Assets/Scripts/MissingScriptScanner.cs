using UnityEngine;

public class MissingScriptScanner : MonoBehaviour
{
    void Start()
    {
        var gos = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in gos)
        {
            var comps = go.GetComponents<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i] == null)
                    Debug.LogError($"[Missing Script] {GetPath(go)} (component index {i})");
            }
        }
    }

    static string GetPath(GameObject go)
    {
        string path = go.name;
        var t = go.transform;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
        return path;
    }
}
