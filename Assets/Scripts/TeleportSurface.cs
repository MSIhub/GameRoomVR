using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSurface : MonoBehaviour
{
    public static TeleportSurface Instance;
    public List<Collider> Colliders;
    public LayerMask Mask;

    PhysicsScene m_Scene;
    private void Awake()
    {
        Instance = this;
        m_Scene = gameObject.scene.GetPhysicsScene();
    }

    public Vector3? Raycast( Vector3 position, Vector3 direction, float range )
    {
        if( m_Scene.Raycast( position, direction, out var hitInfo, range, Mask.value ) )
        {
            return Colliders.Contains( hitInfo.collider ) ? new Vector3?( hitInfo.point ) : null;
        }

        return null;
    }

}
