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
        Vector3 targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        while (enabled)
        {
            targetPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);

          //  transform.rotation = Quaternion.LookRotation(targetPosition);

            yield return null;
        }
    }
}
