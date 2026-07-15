using Fusion;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
    [Header("Перетащи сюда объект SpawnPoint из сцены")]
    public Transform SpawnPoint; 

    private NetworkRunner _runner;

    private void Start()
    {
        _runner = Object.FindFirstObjectByType<NetworkRunner>();
    }

    private void Update()
    {
        if (_runner != null && _runner.LocalPlayer != null && _runner.IsPlayer)
        {
            if (Object.FindFirstObjectByType<PlayerMovement>() == null)
            {
                // Если мы указали точку спавна на сцене — берем её координаты.
                // Если забыли указать — спавним по умолчанию на высоте 3 метра.
                Vector3 spawnPos = SpawnPoint != null ? SpawnPoint.position : new Vector3(0, 3f, 0);
                Quaternion spawnRot = SpawnPoint != null ? SpawnPoint.rotation : Quaternion.identity;

                _runner.Spawn(PlayerPrefab, spawnPos, spawnRot);
            }
        }
    }
}