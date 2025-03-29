using UnityEngine;

public class RunInBackground : MonoBehaviour
{
    void Awake()
    {
        // Allows the game (and video player) to continue running when the editor loses focus.
        Application.runInBackground = true;
    }
}
