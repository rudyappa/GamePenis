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

    private void Start()
    {
        _runner = UnityEngine.Object.FindFirstObjectByType<NetworkRunner>();
        if (_runner != null)
        {
            _runner.AddCallbacks(this);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Спавним персонажа ТОЛЬКО для себя (локального игрока), когда МЫ заходим
        if (player == runner.LocalPlayer)
        {
            // Отключаем меню-камеру
            GameObject menuCam = GameObject.Find("MenuCamera");
            if (menuCam != null)
            {
                menuCam.SetActive(false);
            }

            // Настраиваем точку спавна
            Vector3 spawnPos = SpawnPoint != null ? SpawnPoint.position : new Vector3(0, 3f, 0);
            Quaternion spawnRot = SpawnPoint != null ? SpawnPoint.rotation : Quaternion.identity;

            // Определяем префаб:
            GameObject prefabToSpawn;

            // Считаем, сколько игроков сейчас в сессии
            int activePlayerCount = runner.ActivePlayers != null ? System.Linq.Enumerable.Count(runner.ActivePlayers) : 1;

            // Если комната пустая или мы в ней самые первые — спавним Player 1 (белый)
            // Иначе — спавним Player 2 (синий)
            if (activePlayerCount <= 1)
            {
                prefabToSpawn = Player1Prefab;
                Debug.Log($"[SPAWNER] Ты зашел первым! Спавним Player 1 для {player}");
            }
            else
            {
                prefabToSpawn = Player2Prefab != null ? Player2Prefab : Player1Prefab;
                Debug.Log($"[SPAWNER] Ты зашел вторым (или позже)! Спавним Player 2 для {player}");
                
                // Слегка смещаем позицию спавна второго игрока вбок, чтобы они не застревали друг в друге
                spawnPos += new Vector3(2f, 0f, 0f);
            }

            // Спавним ровно ОДИН сетевой объект игрока
            runner.Spawn(prefabToSpawn, spawnPos, spawnRot, player);
        }
    }

    // ======================================================================
    // ВСЕ ОБЯЗАТЕЛЬНЫЕ СЕТЕВЫХ ЗАГЛУШКИ ДЛЯ ИСКЛЮЧЕНИЯ ОШИБОК КОМПИЛЯЦИИ
    // ======================================================================

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    
    // Использование полного пути Fusion.NetworkInput решает проблему конфликта типов
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
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}