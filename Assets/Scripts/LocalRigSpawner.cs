using UnityEngine;
using Fusion;

public class LocalRigSpawner : SimulationBehaviour, ISpawned
{

    public GUISkin GUISkin;

    public bool AutoSpawnXR = true;
    public GameObject XRRig;
    [Space]
    public bool AutoSpawnPC = false;
    public GameObject PCRig;

    static bool SavedSpawnSelectionXR = false;
    static bool SavedSpawnSelectionPC = false;


    bool m_ShowPlayerSpawnChoice = false;

    public void Spawned()
    {
        if( Object.HasInputAuthority )
        {
            m_ShowPlayerSpawnChoice = AutoSpawnPC == false && AutoSpawnXR == false;

            if( AutoSpawnXR || Application.platform == RuntimePlatform.Android || SavedSpawnSelectionXR )
            {
                SpawnPlayer( XRRig );
            }
            else if( AutoSpawnPC || SavedSpawnSelectionPC )
            {
                SpawnPlayer( PCRig );
            }
        }
    }

    
    private void OnGUI()
    {
        if( m_ShowPlayerSpawnChoice == false )
        {
            return;
        }

        GUI.skin = FusionScalableIMGUI.GetScaledSkin( GUISkin, out var height, out var width, out var padding, out var margin, out var leftBoxMargin );
        GUILayout.BeginArea( new Rect( leftBoxMargin, margin, width, Screen.height ) );
        {
            GUILayout.BeginVertical( GUI.skin.window );
            {

                if( GUILayout.Button( "VR Rig" ) )
                {
                    SavedSpawnSelectionXR = true;
                    SpawnPlayer( XRRig );

                }
                if( GUILayout.Button( "PC Rig (Debug)" ) )
                {
                    SavedSpawnSelectionPC = true;
                    SpawnPlayer( PCRig );
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndArea();
    }
    

    void SpawnPlayer( GameObject prefab )
    {
        ObserverCamera.DisableObserverCamera();

        m_ShowPlayerSpawnChoice = false;
        var rig = Instantiate( prefab, transform );
        rig.transform.localPosition = Vector3.zero;
        rig.transform.localRotation = Quaternion.identity;

        GetComponent<Player>().LocalInput = GetComponentInChildren<PlayerInputHandler>();

        this.enabled = false;
    }
}
