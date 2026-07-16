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

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        // Проверяем, запущен ли сетевой движок Runner
        if (Runner == null || !Runner.IsRunning)
        {
            _isSinglePlayer = true;
            return;
        }

        // --- ВАЖНОЕ ИЗМЕНЕНИЕ ДЛЯ МУЛЬТИПЛЕЕРА ---
        // Если этот персонаж принадлежит НЕ нам (мы им не управляем)
        if (Object != null && !Object.HasInputAuthority)
        {
            // Отключаем физику и сам скрипт управления на чужом персонаже
            if (_controller != null) _controller.enabled = false;
            this.enabled = false;
        }
    }

    // Вызывается в мультиплеере (и для Хоста, и для Shared Client)
    public override void FixedUpdateNetwork()
    {
        if (_isSinglePlayer) return;

        // --- ВАЖНОЕ ИЗМЕНЕНИЕ ---
        // Двигаем персонажа только в том случае, если у нас есть права на ввод (HasInputAuthority)
        if (Object != null && !Object.HasInputAuthority) return;
        
        MovePlayer(Runner.DeltaTime);
    }

    // Этот метод работает в обычном режиме Unity (Синглплеер)
    private void Update()
    {
        // Если запущен мультиплеер — этот блок молчит, работает FixedUpdateNetwork
        if (!_isSinglePlayer) return;

        MovePlayer(Time.deltaTime);
    }

    // Общий метод управления — одинаково идеален и для Хоста, и для Клиента, и для Сингла
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

        // ---- ПРЫЖОК ----
        if (Input.GetKey(KeyCode.Space) && _controller.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        _controller.Move(_velocity * deltaTime);
    }
}