using UnityEngine;
using Fusion;

public class PrefabSpawner : NetworkBehaviour
{
    public void Spawn( NetworkObject prefab )
    {
        if( Object.HasStateAuthority )// todo: predicted spawn does not work correctly yet, so we only spawn on state authority.
        {
            Runner.Spawn( prefab, transform.position, transform.rotation );
        }
    }

}
