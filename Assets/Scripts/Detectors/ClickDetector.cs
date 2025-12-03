using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDetector : MonoBehaviour
{
    private void OnEnable()
    {
        InputHandler.Instance.LeftButtonClicked += OnClick;
    }

    private void OnDisable()
    {
        InputHandler.Instance.LeftButtonClicked -= OnClick;
    }

    private void OnClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out FlagKeeper flagKeeper))
                flagKeeper.TryStartPlacement();
        }
    }
}
