using UnityEngine;
using Fusion;

public class PickableObject : NetworkBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private NetworkTransform _netTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        _netTransform = GetComponent<NetworkTransform>();
    }

    public void PickUp(Transform targetHoldPoint)
    {
        // 1. Запрашиваем права, если их нет
        if (!Object.HasStateAuthority)
        {
            Object.RequestStateAuthority();
        }

        // 2. ОТКЛЮЧАЕМ сетевой трансформ, чтобы он не дергал кубик назад и не создавал 1 FPS лаги!
        if (_netTransform != null)
        {
            _netTransform.enabled = false;
        }

        // 3. Выключаем физику, чтобы кубик стал послушным
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (col != null) col.isTrigger = true;

        // 4. Привязываем кубик к руке обычным методом Unity
        transform.SetParent(targetHoldPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void PlaceDown()
    {
        if (!Object.HasStateAuthority)
        {
            Object.RequestStateAuthority();
        }

        // 1. Отвязываем от руки
        transform.SetParent(null);

        // 2. Возвращаем физику
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (col != null) col.isTrigger = false;

        // 3. ВКЛЮЧАЕМ сетевой трансформ обратно, чтобы он снова синхронизировал кубик на земле
        if (_netTransform != null)
        {
            _netTransform.enabled = true;
        }
    }
}