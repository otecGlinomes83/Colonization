using UnityEngine;

public class CameraMouseControl : MonoBehaviour
{
    [SerializeField] private Vector3 _startPosition = new Vector3(0, 20f, 0);

    [SerializeField] private float _maxZ;
    [SerializeField] private float _minZ;
    [SerializeField] private float _maxX;
    [SerializeField] private float _minX;

    [SerializeField] private float _dragSpeed = 0.01f;

    private PlayerInput _playerInput;

    private bool _isDragging = false;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void Start()
    {
        transform.position = _startPosition;
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

            Vector3 newPosition = transform.position + new Vector3(-mouseDelta.x, 0f, -mouseDelta.y) * _dragSpeed;

            newPosition = new Vector3
                (
                Mathf.Clamp(newPosition.x, _minX, _maxX),
                transform.position.y,
                Mathf.Clamp(newPosition.z, _minZ, _maxZ)
                );

            transform.position = newPosition;
        }
    }
}
