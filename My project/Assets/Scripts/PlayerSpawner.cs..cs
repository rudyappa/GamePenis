using Fusion;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
    private NetworkRunner _runner;

    private void Start()
    {
        _runner = FindObjectOfType<NetworkRunner>();
    }

    private void Update()
    {
        // Если подключился новый игрок (локальный)
        if (_runner != null && _runner.LocalPlayer != null && _runner.IsPlayer)
        {
            // Проверяем, есть ли уже игрок в сцене
            if (FindObjectOfType<PlayerMovement>() == null)
            {
                // Спавним игрока в точке (0, 1, 0)
                _runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }
}