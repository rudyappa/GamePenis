using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float Speed = 5f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15f;
    public float MouseSensitivity = 200f; 

    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isSinglePlayer = false;

    // Переменная для надежного запоминания нажатия прыжка
    private bool _jumpBuffered = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        if (Runner == null || !Runner.IsRunning)
        {
            _isSinglePlayer = true;
            return;
        }

        if (Object != null && !Object.HasInputAuthority)
        {
            if (_controller != null) _controller.enabled = false;
            this.enabled = false;
        }
    }

    // Ввод мышки и прыжка ловим в обычном кадре (Update) — так он никогда не пропадёт
    private void Update()
    {
        // Считываем прыжок для сетевого игрока
        if (!_isSinglePlayer)
        {
            if (Object != null && Object.HasInputAuthority)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _jumpBuffered = true;
                }
            }
            return;
        }

        // Одиночная игра
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpBuffered = true;
        }
        MovePlayer(Time.deltaTime);
    }

    // Применяем движение и физику в сетевом тике
    public override void FixedUpdateNetwork()
    {
        if (_isSinglePlayer) return;

        if (Object != null && !Object.HasInputAuthority) return;
        
        MovePlayer(Runner.DeltaTime);

        // В конце сетевого тика обязательно сбрасываем прыжок
        _jumpBuffered = false; 
    }

    private void MovePlayer(float deltaTime)
    {
        if (_controller == null || !_controller.enabled) return;

        // ---- ВРАЩЕНИЕ ТЕЛА МЫШКОЙ ----
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // ---- ГРАВИТАЦИЯ ----
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; 
        }
        _velocity.y += Gravity * deltaTime;

        // ---- ВВОД WASD ----
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move * Speed * deltaTime);

        // ---- ПРЫЖОК (теперь работает идеально!) ----
        if (_jumpBuffered && _controller.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            _jumpBuffered = false; // Сразу зануляем, чтобы не прыгнуть дважды
        }

        _controller.Move(_velocity * deltaTime);
    }
}