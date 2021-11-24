using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField] private Player _playerPrefab;
	[SerializeField] private List<GameObject> _startPrefab;
	private Vector3 _startPosition;
	private Quaternion _startRotation;
	
	[SerializeField]
	private bool debugLogs;
	
	#region INetworkRunnerCallbacks

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
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

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
	      //  runner.Despawn(player.Get<NetworkObject>());
	      Debug.Log("Player Left" + player.PlayerId);
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
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

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
        {
           
        }

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
        
        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
        {
        }

        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
        {
        }

        #endregion

	
	
	
	/*public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
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

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		runner.Despawn(player.Get<NetworkObject>());
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
	*/
	

}
