using System.Collections.Generic;
using UnityEngine;
using Fusion;

[System.Flags]
public enum InputAction
{
    GRAB = 1 << 0,
    DROP = 1 << 1,
    TELEPORT = 1 << 2,
    LEFTTURN = 1 << 3,
    RIGHTTURN = 1 << 4
}

[System.Flags]
public enum InputState
{
    TELEPORT_MODE = 1 << 0,
}

public struct InputDataController : INetworkStruct
{
    NetworkBool IsPreprocessed;
    
    public InputAction Actions;
    public InputState States;
    
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;

    public bool GetAction( InputAction action )
    {
        Debug.Assert( IsPreprocessed, "Actions are not preprocessed yet. Actions will not be read correctly." );
        return ( Actions & action ) == action;
    }
    public bool GetState( InputState state )
    {
        return ( States & state ) == state;
    }

    public InputAction PreprocessActions( InputAction previousActions )
    {
        var originalActions = Actions;

        Debug.Assert( IsPreprocessed == false , "Trying to preprocess actions twice. Will result in incorrect actions." );
        Actions = previousActions ^ Actions; // xor the previous and current actions to get the state change.
        IsPreprocessed = true;

        return originalActions;
    }
}

public struct InputData : INetworkInput
{
    public Vector3 HeadLocalPosition;
    public Quaternion HeadLocalRotation;

    public InputDataController Left;
    public InputDataController Right;

    public void ResetStates()
    {
        Left.States = 0;
        Right.States = 0;
    }
}
