using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ClickDetector))]
public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private FlagBlueprint _flagBlueprintPrefab;

    private FlagBlueprint _blueprint;
    private Flag _flag;

    private PlayerInput _playerInput;
    private Coroutine _moveCoroutine;

    private float _delay = 0.25f;

    public event Action Placed;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();
    }

    public void StartPlacement()
    {
        StartCoroutine(WaitForPlacementDelay());
    }

    public void DestroyFlag()
    {
        if (_flag == null)
            return;

        Destroy(_flag.gameObject);
    }

    public bool GetFlagPosition(out Transform flagPosition)
    {
        flagPosition = null;

        if (_flag == null)
            return false;

        flagPosition = _flag.transform;

        return true;
    }

    private IEnumerator WaitForPlacementDelay()
    {
        yield return new WaitForSecondsRealtime(_delay);

        if (_blueprint == null)
            _blueprint = Instantiate(_flagBlueprintPrefab);

        _playerInput.Enable();

        _moveCoroutine = StartCoroutine(MoveFlag());

        _playerInput.Player.LeftClick.performed += OnMouseClicked;
    }

    private void OnMouseClicked(InputAction.CallbackContext context)
    {
        _playerInput.Player.LeftClick.performed -= OnMouseClicked;

        StopCoroutine(_moveCoroutine);
        _playerInput.Disable();

        if (_flag == null)
            _flag = Instantiate(_flagPrefab);

        _flag.transform.position = _blueprint.transform.position;

        Destroy(_blueprint.gameObject);

        Placed?.Invoke();
    }

    private IEnumerator MoveFlag()
    {
        yield return null;

        while (enabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
                if (hit.collider.gameObject.TryGetComponent<Floor>(out _))
                    _blueprint.transform.position = hit.point;

            yield return null;
        }
    }
}