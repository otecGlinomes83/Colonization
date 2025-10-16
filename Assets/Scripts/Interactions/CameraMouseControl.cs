using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMouseControl : MonoBehaviour
{
    [SerializeField] private Vector3 _startPosition = new Vector3(0, 30f, 0);

    [SerializeField] private BoxCollider _cameraBounds;

    [SerializeField] private float _dragSpeed = 0.01f;

    private PlayerInput _playerInput;

    private bool _isDragging = false;

    private void OnValidate()
    {
        transform.position = _startPosition;
    }

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();

        _playerInput.CameraMove.MoveButton.performed += OnDragPerformed;
        _playerInput.CameraMove.MoveButton.canceled += OnDragCanceled;
    }

    private void OnDisable()
    {
        _playerInput.Disable();

        _playerInput.CameraMove.MoveButton.performed -= OnDragPerformed;
        _playerInput.CameraMove.MoveButton.canceled -= OnDragCanceled;
    }

    private void Update()
    {
        if (_isDragging)
        {
            if (Cursor.lockState != CursorLockMode.Confined)
                Cursor.lockState = CursorLockMode.Confined;

            Vector2 mouseDelta = _playerInput.CameraMove.MouseDelta.ReadValue<Vector2>();

            Vector3 newPosition = transform.position + new Vector3(-mouseDelta.x, 0f, -mouseDelta.y) * _dragSpeed;

            newPosition = new Vector3(Mathf.Clamp(newPosition.x, _cameraBounds.bounds.min.x, _cameraBounds.bounds.max.x), transform.position.y,
                Mathf.Clamp(newPosition.z, _cameraBounds.bounds.min.z, _cameraBounds.bounds.max.z));

            transform.position = newPosition;
        }

        if (Cursor.lockState != CursorLockMode.None)
            Cursor.lockState = CursorLockMode.None;
    }

    private void OnDragPerformed(InputAction.CallbackContext ctx) =>
         _isDragging = true;

    private void OnDragCanceled(InputAction.CallbackContext ctx) =>
        _isDragging = false;
}