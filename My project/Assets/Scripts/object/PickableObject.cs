using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public Transform holdPoint;
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.mass = 1f;
        rb.linearDamping = 0.5f;

        // Игнорируем столкновение с игроком, чтобы предмет не "выталкивало" при отпускании
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Collider playerCollider = player.GetComponent<CharacterController>();
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(col, playerCollider, true);
            }
        }
    }

    public void PickUp(Transform holdPoint)
{
    rb.isKinematic = true;
    transform.SetParent(holdPoint);
    
    // Берём настройки позы из ItemData
    ItemData data = GetComponent<ItemData>();
    if (data != null)
    {
        transform.localPosition = data.holdPosition;
        transform.localRotation = Quaternion.Euler(data.holdRotation);
    }
    else
    {
        // Если нет ItemData — ставим в центр (как раньше)
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    col.enabled = false;
}

public void PlaceDown()
{
    transform.SetParent(null);
    rb.isKinematic = false;
    rb.linearVelocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    // ВКЛЮЧАЕМ КОЛЛАЙДЕР
    col.enabled = true;
}
}