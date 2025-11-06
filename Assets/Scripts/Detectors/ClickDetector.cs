using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDetector : MonoBehaviour
{
    private PlayerInput _input;

    public event Action<Base> BaseClicked;

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

    private void OnClick(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out Base detectedBase))
                BaseClicked?.Invoke(detectedBase);
        }
    }
}
