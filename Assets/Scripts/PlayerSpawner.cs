using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField] private NetworkObject prefab;
	[SerializeField] private List<GameObject> _startPrefab;
	private Vector3 _startPosition;
	private Quaternion _startRotation;
	
	private Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
	
	[SerializeField] private bool debugLogs;

	#region INetworkRunnerCallbacks

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
	        if (this.debugLogs)
	        {
		        Debug.Log($"OnPlayerJoined {player} mode = {runner.GameMode}");
	        }
	        switch (runner.GameMode)
	        {
		        case GameMode.Single:
		        case GameMode.Server:
		        case GameMode.Host:
			        this.SpawnPlayer(runner, player);
			        break;
	        }
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
	        if (this.debugLogs)
	        {
		        Debug.Log($"OnPlayerLeft {player} mode = {runner.GameMode}");
	        }
	        switch (runner.GameMode)
	        {
		        case GameMode.Single:
		        case GameMode.Server:
		        case GameMode.Host:
			        this.TryDespawnPlayer(runner, player);
			        break;
	        }
        }

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
	        if (this.debugLogs)
	        {
		        Debug.Log($"OnShutdown mode = {runner.GameMode} reason = {shutdownReason}");
		        foreach (var pair in this.spawnedPlayers)
		        {
			        Debug.LogWarning($"Prefab not despawned? {pair.Key}:{pair.Value?.Id}");
		        }
	        }
        }

        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
	        if (this.debugLogs)
	        {
		        Debug.Log($"OnConnectedToServer mode = {runner.GameMode}");
	        }
	        if (runner.GameMode == GameMode.Shared)
	        {
		        this.SpawnPlayer(runner, runner.LocalPlayer);
	        }
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
        {
	        if (this.debugLogs)
	        {
		        Debug.Log($"OnDisconnectedFromServer mode = {runner.GameMode}");
	        }
	        if (runner.GameMode == GameMode.Shared)
	        {
		        this.TryDespawnPlayer(runner, runner.LocalPlayer);
	        }
           
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
        
	private void SpawnPlayer(NetworkRunner runner, PlayerRef player)
	{
		SetStartSpawingPose();

		NetworkObject instance = runner.Spawn(this.prefab, _startPosition, _startRotation, player);
		if (this.debugLogs)
		{
			if (this.spawnedPlayers.TryGetValue(player, out NetworkObject oldValue))
			{
				Debug.LogWarning($"Replacing NO {oldValue?.Id} w/ {instance?.Id} for {player}");
			}
			else
			{
				Debug.Log($"Spawned NO {instance?.Id} for {player}");
			}
		}
		this.spawnedPlayers[player] = instance;
	}

	private void SetStartSpawingPose()
	{
		_startPosition = Vector3.zero;
		_startRotation = Quaternion.identity;
		var scene = SceneManager.GetActiveScene();
		if (scene.buildIndex <= _startPrefab.Count)
		{
			_startPosition = _startPrefab[scene.buildIndex].transform.position;
			_startRotation = _startPrefab[scene.buildIndex].transform.rotation;
		}
	}

	private bool TryDespawnPlayer(NetworkRunner runner, PlayerRef player)
	{
		if (this.spawnedPlayers.TryGetValue(player, out NetworkObject instance))
		{
			if (this.debugLogs)
			{
				Debug.Log($"Despawning NO {instance?.Id} for {player}");
			}
			runner.Despawn(instance);
			return this.spawnedPlayers.Remove(player);
		}
		if (this.debugLogs)
		{
			Debug.LogWarning($"No spawned NO found for player {player}");
		}
		return false;
	}
}
