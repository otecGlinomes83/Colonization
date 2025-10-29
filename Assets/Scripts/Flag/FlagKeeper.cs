using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[RequireComponent(typeof(ClickDetector))]
public class FlagKeeper : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private FlagBlueprint _flagBlueprintPrefab;

    private Coroutine _moveCoroutine;

    private PlayerInput _playerInput;

    private FlagBlueprint _blueprint;
    private Flag _flag;

    public event Action Placed;

    public void StartPlacement()
    {
        if (_blueprint == null)
            _blueprint = Instantiate(_flagBlueprintPrefab);

        _moveCoroutine = StartCoroutine(MoveFlag());

        _playerInput = new PlayerInput();
        _playerInput.Enable();
        _playerInput.Player.LeftClick.performed += OnMouseClicked;
    }

    public bool TryGetFlagPosition(out Transform flagPosition)
    {
        flagPosition = null;

        if(_flag==null)
            return false;

        flagPosition = _flag.transform;

        return true;
    }

    private void OnMouseClicked(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _playerInput.Player.LeftClick.performed -= OnMouseClicked;
        _playerInput.Disable();

        StopCoroutine(_moveCoroutine);

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
                if (hit.collider.gameObject.TryGetComponent<Floor>(out _))
                {
                    _blueprint.transform.position = hit.point;
                }

            yield return null;
        }
    }
}