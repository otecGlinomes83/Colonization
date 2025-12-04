using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private FlagBlueprint _blueprintPrefab;

    private FlagBlueprint _blueprint;
    private Flag _flag;
    private Coroutine _moveCoroutine;

    public event Action Placed;

    public void StartPlacement()
    {
        if (_moveCoroutine != null)
            return;

        if (_blueprint == null)
            _blueprint = Instantiate(_blueprintPrefab);

        _moveCoroutine = StartCoroutine(MoveFlag());
    }

    public void DestroyFlag()
    {
        if (_flag == null)
            return;

        TryStopPlacement();
        Destroy(_flag.gameObject);
    }

    public bool TryGetFlagPosition(out Transform flagPosition)
    {
        flagPosition = null;

        if (_flag == null)
            return false;

        flagPosition = _flag.transform;

        return true;
    }

    public void TryStopPlacement()
    {
        if (_moveCoroutine == null)
            return;

        StopCoroutine(_moveCoroutine);
        _moveCoroutine = null;

        if (_flag == null)
            _flag = Instantiate(_flagPrefab);

        _flag.transform.position = _blueprint.transform.position;

        Destroy(_blueprint.gameObject);

        Placed?.Invoke();
    }

    private IEnumerator MoveFlag()
    {
        yield return null;

        while (_blueprint != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
                if (hit.collider.gameObject.TryGetComponent<Floor>(out _))
                    _blueprint.transform.position = hit.point;

            yield return null;
        }
    }
}