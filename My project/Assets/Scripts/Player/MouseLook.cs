using Fusion;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    public float mouseSensitivity = 200f;
    private float xRotation = 0f;

    void Start()
    {
        // Блокируем курсор мыши
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    public override void Spawned()
    {
        // Проверяем: если это сетевой объект и у нас НЕТ прав на него — выключаем камеру
        if (Object != null && !Object.HasInputAuthority)
        {
            Camera cam = GetComponent<Camera>();
            if (cam != null) cam.enabled = false;

            AudioListener listener = GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;

            this.enabled = false;
        }
    }

    void Update()
    {
        // ЗАЩИТА: Если сеть запущена, но у нас нет прав на этот объект (это чужой игрок), игнорируем ввод
        if (Object != null && !Object.HasInputAuthority) return;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}