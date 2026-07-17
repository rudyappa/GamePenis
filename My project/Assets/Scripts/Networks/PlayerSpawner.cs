using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Префаб для первого игрока (Хоста)")]
    public GameObject Player1Prefab;

    [Header("Префаб для второго игрока (Клиента)")]
    public GameObject Player2Prefab;

    [Header("Перетащи сюда объект SpawnPoint из сцены")]
    public Transform SpawnPoint;

    private NetworkRunner _runner;
    
    // Статический флаг — он один на все копии скрипта в этом окне игры
    private static bool _localPlayerSpawnedInThisWindow = false;

    private void Start()
    {
        _localPlayerSpawnedInThisWindow = false; // Сбрасываем при старте сцены
        _runner = UnityEngine.Object.FindFirstObjectByType<NetworkRunner>();
        if (_runner != null)
        {
            _runner.AddCallbacks(this);
        }
    }

    private void OnDestroy()
    {
        if (_runner != null) _runner.RemoveCallbacks(this);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            // ЖЕЛЕЗОБЕТОННАЯ ЗАЩИТА: если в этом окне уже кто-то спавнился — полный игнор
            if (_localPlayerSpawnedInThisWindow) return;
            _localPlayerSpawnedInThisWindow = true;

            // Отключаем меню-камеру
            GameObject menuCam = GameObject.Find("MenuCamera");
            if (menuCam != null) menuCam.SetActive(false);

            Vector3 spawnPos = SpawnPoint != null ? SpawnPoint.position : new Vector3(0, 3f, 0);
            Quaternion spawnRot = SpawnPoint != null ? SpawnPoint.rotation : Quaternion.identity;

            int activePlayerCount = runner.ActivePlayers != null ? System.Linq.Enumerable.Count(runner.ActivePlayers) : 1;
            GameObject prefabToSpawn = (activePlayerCount <= 1) ? Player1Prefab : (Player2Prefab != null ? Player2Prefab : Player1Prefab);

            if (activePlayerCount > 1) spawnPos += new Vector3(2f, 0f, 0f);

            Debug.Log($"[SPAWNER] Спавним единственного персонажа для {player}. Всего игроков: {activePlayerCount}");
            runner.Spawn(prefabToSpawn, spawnPos, spawnRot, player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, Fusion.NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, Fusion.NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}