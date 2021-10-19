using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    InputData m_Data;

    public Transform Head;
    public LocalController LeftController;
    public LocalController RightController;
    public Transform RelativeTo;

    private void Awake()
    {
        if( RelativeTo == null )
        {
            RelativeTo = transform.parent;
        }
    }

    void Start()
    {
        var networkedParent = GetComponentInParent<NetworkObject>();
        if( networkedParent == null || networkedParent.Runner == null )
        {
            return;
        }

        var runner = networkedParent.Runner;
        var events = runner.GetComponent<NetworkEvents>();

        events.OnInput.AddListener( OnInput );
    }

    private void Update()
    {
        LeftController?.UpdateInput( ref m_Data.Left );
        RightController?.UpdateInput( ref m_Data.Right );
    }

    void OnInput( NetworkRunner runner, NetworkInput inputContainer )
    {
        m_Data.HeadLocalPosition = RelativeTo.InverseTransformPoint( Head.position );
        m_Data.HeadLocalRotation = Quaternion.Inverse( RelativeTo.rotation ) * Head.rotation;

        LeftController?.UpdateInputFixed( ref m_Data.Left );
        RightController?.UpdateInputFixed( ref m_Data.Right );

        inputContainer.Set( m_Data );

        m_Data.ResetStates();
    }
}
