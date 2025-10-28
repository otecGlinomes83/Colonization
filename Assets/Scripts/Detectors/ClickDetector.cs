using System;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{

    private PlayerInput _input = new PlayerInput();

    public event Action BaseClicked;

    private void OnEnable()
    {
        _input.Player.LeftClick.performed += OnClick;
    }

    private void OnDisable()
    {
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
