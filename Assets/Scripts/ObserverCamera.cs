using UnityEngine;

public class ObserverCamera : MonoBehaviour
{
    static GameObject Instance;

    public static void DisableObserverCamera()
    {
        if ( Instance != null )
        {
            Instance.SetActive( false );
        }
    }
    private void Awake()
    {
        Instance = gameObject;
    }
}
