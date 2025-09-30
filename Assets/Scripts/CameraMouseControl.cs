using UnityEngine;

public class CameraMouseControl : MonoBehaviour
{
    [SerializeField] private float _dragSpeed=0.01f;

    private PlayerInput _playerInput;

    private bool _isDragging = false;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.CameraMove.MoveButton.performed += ctx => _isDragging = true;
        _playerInput.CameraMove.MoveButton.canceled += ctx => _isDragging = false;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.CameraMove.MoveButton.performed -= ctx => _isDragging = true;
        _playerInput.CameraMove.MoveButton.canceled -= ctx => _isDragging = false;
    }

    private void Update()
    {
        if (_isDragging)
        {
            Vector2 mouseDelta = _playerInput.CameraMove.MouseDelta.ReadValue<Vector2>();

            Vector3 move = new Vector3(-mouseDelta.x, 0f, -mouseDelta.y) * _dragSpeed;

            transform.position += move;
        }
    }
}
