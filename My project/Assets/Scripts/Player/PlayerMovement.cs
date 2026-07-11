using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;      // скорость ходьбы
    public float gravity = -9.81f;    // сила гравитации
    public float jumpHeight = 1.2f;   // высота прыжка (если понадобится)

    private CharacterController controller;
    private Vector3 velocity;         // текущая вертикальная скорость (падение/прыжок)
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // если стоим на земле и скорость падения отрицательная — обнуляем её
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // небольшое отрицательное значение, чтобы "прижимало" к земле
        }

        // считываем ввод с клавиатуры (WASD или стрелки)
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        // направление движения относительно того, куда смотрит игрок
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // прыжок по пробелу (можно убрать, если хоррору прыжки не нужны)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // применяем гравитацию
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}