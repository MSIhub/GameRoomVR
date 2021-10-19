using UnityEngine;

[RequireComponent( typeof( LineRenderer ) )]
public class TeleportHandler : MonoBehaviour
{
    public Transform Root;
    public Transform Head;

    public Gradient ValidGradient;
    public Gradient InvalidGradient;
    public float Range = 10f;

    Vector3? LastTeleportPoint;
    LineRenderer m_Line;

    private void Awake()
    {
        m_Line = GetComponent<LineRenderer>();
    }

    public void UpdateInput( InputDataController input )
    {
        if( input.GetState( InputState.TELEPORT_MODE ) )
        {
            LastTeleportPoint = TeleportSurface.Instance.Raycast( transform.position, transform.forward, Range );
        }

        if( input.GetAction( InputAction.TELEPORT ) )
        {
            if( LastTeleportPoint != null && LastTeleportPoint.HasValue )
            {
                DoTeleport( LastTeleportPoint.Value );
            }
        }

        UpdateLine( input );
    }

    void UpdateLine( InputDataController input )
    {
        if( m_Line == null )
        {
            return;
        }

        m_Line.enabled = input.GetState( InputState.TELEPORT_MODE );
        m_Line.SetPosition( 0, transform.position );

        if( LastTeleportPoint.HasValue )
        {
            m_Line.SetPosition( 1, LastTeleportPoint.Value );
            m_Line.colorGradient = ValidGradient;
        }
        else
        {
            m_Line.SetPosition( 1, transform.position + transform.forward * Range );
            m_Line.colorGradient = InvalidGradient;
        }
    }

    void DoTeleport( Vector3 target )
    {
        Vector3 headDelta = Head.position - Root.position;
        headDelta.y = 0f;
        Root.position = target - headDelta;
    }
}
