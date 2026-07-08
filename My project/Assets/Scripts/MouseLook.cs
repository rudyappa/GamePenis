using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public Transform playerBody; // сюда перетащим объект Player

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // прячем и блокируем курсор в центре экрана
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // вращение вверх-вниз (камера)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // не даём "переломить шею" — ограничение вверх/вниз
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // вращение влево-вправо (весь игрок, не только камера)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}