using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    private float xRotation = 0f;

    void Start()
    {
        // Блокируем курсор мыши
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    void Update()
    {
        // Крутим ТОЛЬКО камеру вверх и вниз (по оси X)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}