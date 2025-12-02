using System;
using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Robot : MonoBehaviour
{
    [SerializeField] private Rope _rope;

    private Mover _mover;
    private Transform _endPosition;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _rope.gameObject.SetActive(false);
    }

    public void Initialize(Transform endPosition)
    {
        _endPosition = endPosition;
    }

    public void MoveTo(Transform target, Action<Robot> onComplete) =>
        _mover.StartMoveToTarget(target, () => onComplete?.Invoke(this));

    public void CollectResource(Resource resource, Action<Robot, Resource> onDelivered)
    {
        MoveTo(resource.transform, robot =>
        {
            _rope.gameObject.SetActive(true);
            _rope.SetConnectedBody(resource.Rigidbody);

            MoveTo(_endPosition, robot =>
            {
                _rope.SetConnectedBody(null);
                _rope.gameObject.SetActive(false);

                onDelivered?.Invoke(this, resource);
            });
        });
    }
}
