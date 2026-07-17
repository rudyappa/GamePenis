using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkItem : NetworkBehaviour
{
    [Header("Название предмета для инвентаря")]
    public string ItemName = "Cube";

    // Метод для сетевого удаления предмета
    public void PickupItem()
    {
        // Если у нас нет прав на этот объект в Shared Mode — запрашиваем их
        if (!Object.HasStateAuthority)
        {
            Object.RequestStateAuthority();
        }

        // Удаляем предмет из сетевого мира для ВСЕХ игроков одновременно
        Runner.Despawn(Object);
    }
}