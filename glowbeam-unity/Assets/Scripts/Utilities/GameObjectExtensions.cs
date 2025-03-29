using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Sets the GameObject active (shows it).
    /// </summary>
    public static void Show(this GameObject go)
    {
        go.SetActive(true);
    }

    /// <summary>
    /// Sets the GameObject inactive (hides it).
    /// </summary>
    public static void Hide(this GameObject go)
    {
        go.SetActive(false);
    }

    public static List<GameObject> FindAllChildrenByName(this GameObject go, string name)
    {
        List<GameObject> result = new List<GameObject>();
        // GetComponentsInChildren includes the parent itself by default.
        Transform[] children = go.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.gameObject.name == name)
            {
                result.Add(child.gameObject);
            }
        }
        return result;
    }

}
