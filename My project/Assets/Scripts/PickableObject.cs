using UnityEngine;

public class PickableObject : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Если вдруг на кубе нет Rigidbody — добавим автоматически
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        
        // Настройки физики, чтобы куб нормально падал и его можно было кидать
        rb.mass = 1f;
        rb.linearDamping = 0.5f;
    }

    // Метод для поднятия
    public void PickUp(Transform holdPoint)
    {
        rb.isKinematic = true;          // Отключаем физику, чтобы куб замер в воздухе
        transform.SetParent(holdPoint); // Прикрепляем к точке удержания
        transform.localPosition = Vector3.zero; // Ставим ровно в точку
        transform.localRotation = Quaternion.identity; // Выравниваем поворот
    }

    // Метод для броска
    public void Drop(Vector3 throwForce)
    {
        transform.SetParent(null);      // Открепляем от рук
        rb.isKinematic = false;         // Включаем физику обратно
        rb.linearVelocity = throwForce;       // Придаём скорость (силу броска)
    }
}