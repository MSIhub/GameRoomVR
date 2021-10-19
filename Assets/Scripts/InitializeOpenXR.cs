using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Management;

public class InitializeOpenXR : MonoBehaviour
{
    bool IsShutdownComplete = false;

    private void Awake()
    {
        Application.wantsToQuit += ApplicationWantsToQuit;
    }
    private void OnDestroy()
    {
        Application.wantsToQuit -= ApplicationWantsToQuit;
    }

    private void OnEnable()
    {
        Debug.Log( "XR: Start OpenXR" );

        if( XRGeneralSettings.Instance.Manager.activeLoaders.Count == 0 )
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog( "No XR Plugin selected", "Please select an XR Plugin. This Sample was tested with OpenXR. For more detailed setup information please refer to Assets/README.md", "Ok" );
            UnityEditor.SettingsService.OpenProjectSettings( "Project/XR Plug-in Management" );
#endif
            Debug.LogError( "No XR Loader is selected in xr Managment" );
        }

        StartCoroutine( Restart( true, null ) );
    }

    bool ApplicationWantsToQuit()
    {
        if( IsShutdownComplete )
        {
            return true;
        }

        StartCoroutine( Restart( false, OnShutdownComplete ) );
        return false;
    }

    void OnShutdownComplete()
    {
        IsShutdownComplete = true;
        Application.Quit();
    }

    IEnumerator Restart( bool shouldRestart, UnityAction onShutdown )
    {
        if( XRGeneralSettings.Instance.Manager.isInitializationComplete )
        {
            Debug.Log( "XR: Stop subsystems." + XRGeneralSettings.Instance.Manager.isInitializationComplete + ", " + XRGeneralSettings.Instance.Manager.activeLoaders.Count );
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            //yield return null;

            Debug.Log( "XR: Deinitizalize Loader." + XRGeneralSettings.Instance.Manager.isInitializationComplete + ", " + XRGeneralSettings.Instance.Manager.activeLoaders.Count );
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            yield return null;
        }

        onShutdown?.Invoke();
        if( shouldRestart )
        {
            Debug.Log( "XR: Initialize Loader" );
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            while( XRGeneralSettings.Instance.Manager.isInitializationComplete == false )
            {
                yield return null;
            }
            Debug.Log( "XR: Start Subsystems" );
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;

            if( XRGeneralSettings.Instance.Manager.activeLoader != null )
            {
                Debug.Log( "XR: Successful restart." );
            }
            else
            {
                Debug.LogError( "XR: Failure to restart OpenXRLoader after shutdown." );
            }
        }
    }

}
