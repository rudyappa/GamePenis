using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float Speed = 5f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15f;
    public float TurnSmoothTime = 0.08f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _turnSmoothVelocity;
    private NetworkInput _input;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        // Двигаемся только на своей машине
        if (!Object.HasStateAuthority) return;

        // ---- ПОЛУЧАЕМ ВВОД ИЗ СЕТИ (автоматически) ----
        if (GetInput(out NetworkInput input))
        {
            _input = input;
        }

        // ---- ГРАВИТАЦИЯ ----
        _velocity.y += Gravity * Runner.DeltaTime;

        // ---- ПРЫЖОК ----
        if (_input.Jump && _controller.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        // ---- ДВИЖЕНИЕ ----
        Vector3 direction = new Vector3(_input.Horizontal, 0, _input.Vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Плавный поворот
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                ref _turnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // Движение вперёд
            Vector3 move = transform.forward * Speed;
            move.y = _velocity.y;
            _controller.Move(move * Runner.DeltaTime);
        }
        else
        {
            // Только гравитация
            _controller.Move(_velocity * Runner.DeltaTime);
        }

        // ---- КАМЕРА ----
        if (Camera.main != null && Object.HasStateAuthority)
        {
            Vector3 camPos = transform.position + new Vector3(0, 1.8f, -4f);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, camPos, Time.deltaTime * 10f);
            Camera.main.transform.LookAt(transform.position + Vector3.up * 1.2f);
        }
    }
}