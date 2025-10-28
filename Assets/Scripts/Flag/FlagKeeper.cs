using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ClickDetector))]
public class FlagKeeper : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private FlagBlueprint _flagBlueprintPrefab;

    private PlayerInput _playerInput;

    private void OnEnable()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();
        _playerInput.Player.LeftClick.performed += StartPlacement;
    }

    private void OnDisable()
    {
        _playerInput.Player.LeftClick.performed -= StartPlacement;
        _playerInput.Disable();
    }

    private void StartPlacement(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        FlagBlueprint flag = Instantiate(_flagBlueprintPrefab);

        StartCoroutine(MoveFlag(flag));
    }

    private IEnumerator MoveFlag(FlagBlueprint flag)
    {
        yield return null;

        while (enabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
                if (hit.collider.gameObject.TryGetComponent<Floor>(out _))
                {
                    flag.transform.position = hit.point;
                }

            yield return null;
        }
    }
}