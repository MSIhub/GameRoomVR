using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
	[SerializeField] private Player _playerPrefab;
	[SerializeField] private List<GameObject> _startPrefab;
	private Vector3 _startPosition;
	private Quaternion _startRotation;
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		_startPosition = Vector3.zero;
		_startRotation = Quaternion.identity;
		var scene = SceneManager.GetActiveScene();
		if (scene.buildIndex <= _startPrefab.Count)
		{ 
			_startPosition = _startPrefab[scene.buildIndex].transform.position; 
			_startRotation = _startPrefab[scene.buildIndex].transform.rotation;
		}
		runner.Spawn( _playerPrefab, _startPosition, _startRotation, player );
	}

	
	public void OnConnectedToServer(NetworkRunner runner)
	{
		_startPosition = Vector3.zero;
		_startRotation = Quaternion.identity;
		var scene = SceneManager.GetActiveScene();
		if (scene.buildIndex <= _startPrefab.Count)
		{ 
			_startPosition = _startPrefab[scene.buildIndex].transform.position; 
			_startRotation = _startPrefab[scene.buildIndex].transform.rotation;
		}
		if(runner.Topology==SimulationConfig.Topologies.Shared)
			runner.Spawn( _playerPrefab, null, null, runner.LocalPlayer );
	}
	

}
