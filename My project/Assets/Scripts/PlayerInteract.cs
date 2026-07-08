using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 3f;
    public Camera playerCamera;
    public Transform holdPoint; // Сюда перетащишь пустой объект перед камерой

    private GameObject heldObject;        // Что сейчас в руках
    private PickableObject heldPickable;  // Скрипт предмета в руках

    void Update()
    {
        // --- Логика для предмета в руках ---
        if (heldObject != null)
        {
            // Нажали E — бросаем
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropObject();
                return; // Чтобы не проверять луч ниже
            }
            // Нажали Q — уничтожаем предмет в руках
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Destroy(heldObject);
                heldObject = null;
                heldPickable = null;
                return;
            }
        }

        // --- Луч из центра экрана ---
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Отладочная линия (видна в окне Scene)
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            GameObject target = hit.collider.gameObject;
            // Можно закомментировать, если надоест спам в консоли
            // Debug.Log("Смотрю на: " + target.name);

            // --- Взаимодействие с предметом (если смотрим на него и нажали E) ---
            if (heldObject == null && Input.GetKeyDown(KeyCode.E))
            {
                PickableObject pickable = target.GetComponent<PickableObject>();
                if (pickable != null)
                {
                    PickUpObject(target, pickable);
                }
            }

            // --- Разрушение предмета (если нажали Q) ---
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Чтобы случайно не разрушить пол или стены, проверяем наличие Rigidbody
                if (target.GetComponent<Rigidbody>() != null)
                {
                    Destroy(target);
                }
            }
        }
    }

    void PickUpObject(GameObject obj, PickableObject pickable)
    {
        heldObject = obj;
        heldPickable = pickable;
        pickable.PickUp(holdPoint);
    }

    void DropObject()
    {
        if (heldPickable != null)
        {
            // Кидаем вперёд (куда смотрит камера) со скоростью 5, и чуть вверх (+2 по Y)
            Vector3 throwDir = playerCamera.transform.forward * 5f + Vector3.up * 2f;
            heldPickable.Drop(throwDir);
        }
        heldObject = null;
        heldPickable = null;
    }
}