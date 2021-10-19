using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class ConnectionHelper : MonoBehaviour
{
    static GameMode m_GameMode = 0;

    NetworkDebugStart m_DebugStart;

    private void Awake()
    {
        m_DebugStart = GetComponent<NetworkDebugStart>();
    }
    private void Start()
    {
        DoStart();
    }

    public void StartHost()
    {
        ChangeConnection( GameMode.Host );
    }
    public void StartClient()
    {
        ChangeConnection( GameMode.Client );
    }

    public void ChangeConnection( GameMode gameMode )
    {
        Debug.Log( "Shutdown. Change connection to " + gameMode.ToString() );
        m_GameMode = gameMode;

        StartCoroutine( ShutdownRoutine() );
    }

    IEnumerator ShutdownRoutine()
    {
        Camera.main.cullingMask = 0;
        Camera.main.clearFlags = CameraClearFlags.Color;
        Camera.main.backgroundColor = Color.black;
        yield return null;

        m_DebugStart.Shutdown();
    }

    void DoStart()
    {
        Debug.Log( "Start game " + m_GameMode.ToString() );

        switch( m_GameMode )
        {
        case GameMode.Client:
            m_DebugStart.StartClient();
            break;
        case GameMode.Host:
            m_DebugStart.StartHost();
            break;
        case GameMode.Single:
            m_DebugStart.StartSinglePlayer();
            break;
        case GameMode.Shared:
            m_DebugStart.StartSharedClient();
            break;
        }
    }
}
