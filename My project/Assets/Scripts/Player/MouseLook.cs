using Fusion;
using UnityEngine;

// Изменяем наследование с MonoBehaviour на NetworkBehaviour, чтобы иметь доступ к сети Fusion
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

    // Метод Spawned запускается Photon-ом, когда объект появляется в сети
    public override void Spawned()
    {
        // Если у нас НЕТ прав на управление этим персонажем — выключаем его камеру и этот скрипт!
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
        // Если это не наш персонаж, код дальше выполняться не будет (скрипт уже выключен в Spawned)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}