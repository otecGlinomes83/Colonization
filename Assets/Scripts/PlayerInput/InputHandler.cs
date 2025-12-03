using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    private PlayerInput _playerInput;

    public event Action LeftButtonClicked;
    public event Action MoveButtonPressed;
    public event Action MoveButtonReleased;

    public Vector2 MouseDelta { get; private set; }
    public Vector2 MousePosition { get; private set; }

    private void Awake()
    {
        Instance = this;

        _playerInput = new PlayerInput();

        _playerInput.Player.LeftClick.performed += ctx => LeftButtonClicked?.Invoke();
        _playerInput.Player.MoveButton.performed += ctx => MoveButtonPressed?.Invoke();
        _playerInput.Player.MoveButton.canceled += ctx => MoveButtonReleased?.Invoke();

    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.LeftClick.performed -= ctx => LeftButtonClicked?.Invoke();
        _playerInput.Player.MoveButton.performed -= ctx => MoveButtonPressed?.Invoke();
        _playerInput.Player.MoveButton.canceled -= ctx => MoveButtonReleased?.Invoke();

        _playerInput.Disable();
    }

    private void Update()
    {
        MouseDelta = _playerInput.Player.MouseDelta.ReadValue<Vector2>();
        MousePosition = Mouse.current.position.ReadValue();
    }
}