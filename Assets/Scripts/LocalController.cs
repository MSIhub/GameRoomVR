using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SpatialTracking;
using System;

public class LocalController : MonoBehaviour
{
    public bool showDebugValues;
    public Transform RelativeTo;

    public InputActionAsset ActionMap;

    [Header( "Input Actions:" )]
    [Header( "Events" )]
    public InputActionProperty Grab;
    public InputActionProperty Drop;
    public InputActionProperty Teleport;
    [Header( "States" )]
    public InputActionProperty TeleportMode;

    private void Awake()
    {
        if( RelativeTo == null )
        {
            RelativeTo = transform.parent;
        }
    }
    protected void OnEnable()
    {
        ActionMap.Enable();
    }

    protected void OnDisable()
    {
        ActionMap.Disable();
    }

    public Vector3 GetLocalPosition()
    {
        return RelativeTo.InverseTransformPoint( transform.position );
    }
    public Quaternion GetLocalRotation()
    {
        return Quaternion.Inverse( RelativeTo.rotation ) * transform.rotation;
    }

    public void UpdateInput( ref InputDataController container )
    {
        container.Actions ^= Grab.action.triggered ? InputAction.GRAB : 0; // xor to flip the corresponding bit
        container.Actions ^= Drop.action.triggered ? InputAction.DROP : 0;
        container.Actions ^= Teleport.action.triggered ? InputAction.TELEPORT : 0;
    }

    public void UpdateInputFixed( ref InputDataController container )
    {
        container.LocalPosition = GetLocalPosition();
        container.LocalRotation = GetLocalRotation();

        container.States |= TeleportMode.action.ReadValue<float>() > InputSystem.settings.defaultButtonPressPoint ? InputState.TELEPORT_MODE : 0;

        if( showDebugValues )
        {
            Debug.Log( gameObject.name + "= State: " + Convert.ToString( (uint)container.States, 2 ) + "\t  Events:" + Convert.ToString( (uint)container.Actions, 2 ) );
            if( Grab.action.triggered ) Debug.Log( gameObject.name + "Grab was pressed " + Time.frameCount );
            if( Drop.action.triggered ) Debug.Log( gameObject.name + "Drop was pressed " + Time.frameCount );
            if( TeleportMode.action.triggered ) Debug.Log( gameObject.name + "TeleportMode active" + Time.frameCount );
            if( Teleport.action.triggered ) Debug.Log( gameObject.name + "Teleport performed" + Time.frameCount );
        }

    }
}
