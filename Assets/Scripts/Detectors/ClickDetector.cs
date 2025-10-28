using System;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{

    private PlayerInput _input;

    public event Action BaseClicked;

    private void Awake()
    {
        _input = new PlayerInput();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.LeftClick.performed += OnClick;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Player.LeftClick.performed -= OnClick;
    }

    private void OnClick(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent<Base>(out _))
                BaseClicked?.Invoke();
        }
    }
}
