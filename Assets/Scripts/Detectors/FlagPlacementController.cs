using UnityEngine;
using UnityEngine.InputSystem;

public class FlagPlacementController : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;

    private FlagKeeper _selectedFlagKeeper;

    private bool _isPlacing = false;

    private void OnEnable()
    {
        _inputHandler.LeftButtonClicked += OnClick;
    }

    private void OnDisable()
    {
        _inputHandler.LeftButtonClicked -= OnClick;
    }

    private void OnClick()
    {
        if (_isPlacing)
        {
            TryPlaceFlag();
        }
        else
        {
            TrySelectBase();
        }
    }

    private void TrySelectBase()
    {
        if (TryGetComponentUnderMouse(out FlagKeeper flagKeeper))
        {
            _selectedFlagKeeper = flagKeeper;
            _selectedFlagKeeper.TryStartPlacement();
            _isPlacing = true;
        }
    }

    private void TryPlaceFlag()
    {
        if (TryGetComponentUnderMouse<Floor>(out _))
        {
            _selectedFlagKeeper.StopPlacement();
            ResetPlacement();
        }
    }

    private void ResetPlacement()
    {
        _selectedFlagKeeper = null;
        _isPlacing = false;
    }

    private bool TryGetComponentUnderMouse<T>(out T component) where T : Component
    {
        component = null;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out component))
                return true;
        }

        return false;
    }
}
