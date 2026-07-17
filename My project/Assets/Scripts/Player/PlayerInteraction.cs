using Fusion;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    private NetworkItem _currentOverlappingItem;

    private void Update()
    {
        // Кнопку подбора обрабатываем только для нашего ЛОКАЛЬНОГО игрока
        if (Object == null || !Object.HasInputAuthority) return;

        // Если стоим рядом с предметом и нажали клавишу E
        if (_currentOverlappingItem != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[ИНТЕРАКТ] Подобрали предмет: {_currentOverlappingItem.ItemName}");
            
            // TODO: Здесь ты можешь добавить предмет в свой инвентарь!
            // Inventory.Add(_currentOverlappingItem.ItemName);

            // Удаляем предмет из сетевого мира
            _currentOverlappingItem.PickupItem();
            _currentOverlappingItem = null;
        }
    }

    // Обнаруживаем предмет в зоне досягаемости
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkItem>(out var item))
        {
            _currentOverlappingItem = item;
            Debug.Log("Нажмите [E] чтобы подобрать предмет");
        }
    }

    // Уходим от предмета
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<NetworkItem>(out var item))
        {
            if (_currentOverlappingItem == item)
            {
                _currentOverlappingItem = null;
            }
        }
    }
}