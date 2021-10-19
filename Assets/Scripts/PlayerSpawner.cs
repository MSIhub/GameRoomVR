using UnityEngine;
using Fusion;

public class PlayerSpawner : MonoBehaviour
{
	[SerializeField] private Player _playerPrefab;

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		runner.Spawn( _playerPrefab, null, null, player );
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
		if(runner.Topology==SimulationConfig.Topologies.Shared)
			runner.Spawn( _playerPrefab, null, null, runner.LocalPlayer );
	}
}
