using System;
using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    [SerializeField] private float _threshold = 1f;

    private Coroutine _moveToCoroutine;

    public event Action TargetAchieved;

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
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        float sqrThreshold = _threshold * _threshold;

        while (enabled)
        {
            targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);

            float sqrDistance = (targetPosition - transform.position).sqrMagnitude;

            if (sqrDistance <= sqrThreshold)
            {
                TargetAchieved?.Invoke();
                _moveToCoroutine = null;

                yield break;
            }

            yield return null;
        }
    }
}
