using System.Collections.Generic;
using UnityEngine;
using Fusion;

[OrderAfter( typeof( GrabbableObject ) )]
public class VelocityBuffer : SimulationBehaviour
{
    Queue<Vector3> m_VelocityBuffer = new Queue<Vector3>();
    const int VELOCITY_BUFFER_SIZE = 10;
    Rigidbody m_Body;
    Vector3 m_PreviousPosition;

    private void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if( m_VelocityBuffer.Count > VELOCITY_BUFFER_SIZE )
        {
            m_VelocityBuffer.Dequeue();
        }

        m_VelocityBuffer.Enqueue( ( transform.position - m_PreviousPosition ) );
        m_PreviousPosition = transform.position;
    }
    // todo: angular velocity: https://answers.unity.com/questions/49082/rotation-quaternion-to-angular-velocity.html
    public Vector3 GetAverageVelocity()
    {
        if( m_VelocityBuffer.Count == 0 )
        {
            return Vector3.zero;
        }

        Vector3 sum = new Vector3();
        var enumerator = m_VelocityBuffer.GetEnumerator();
        while( enumerator.MoveNext() )
        {
            sum += enumerator.Current;
        }

        return (sum / m_VelocityBuffer.Count ) / Runner.DeltaTime;
    }
}
