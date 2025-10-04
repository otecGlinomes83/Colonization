using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Coroutine _moveToCoroutine;

    public void StartMoveToTarget(Transform target)
    {
        if (_moveToCoroutine != null)
            StopMoveTo();

        _moveToCoroutine = StartCoroutine(MoveTo(target));
    }

    public void StopMoveTo()
    {
        if (_moveToCoroutine == null)
            return;

        StopCoroutine(_moveToCoroutine);
        _moveToCoroutine = null;
    }

    private IEnumerator MoveTo(Transform target)
    {
        while (enabled)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
