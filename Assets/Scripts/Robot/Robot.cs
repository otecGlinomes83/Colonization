using System;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField] private Transform _basePosition;
    [SerializeField] private Rope _rope;
    [SerializeField] private Mover _mover;

    private Resource _targetResource;

    public event Action<Robot, Resource> ResourceDelivered;

    private void Awake()
    {
        _rope.gameObject.SetActive(false);
    }

    public void SetResource(Resource resource)
    {
        _targetResource = resource;

        _mover.StartMoveToTarget(_targetResource.transform);
        _mover.TargetAchieved += OnResourceAchieved;
    }

    private void ConnectResource()
    {
        _mover.StopMoveTo();

        _rope.gameObject.SetActive(true);
        _rope.SetConnectedBody(_targetResource.Rigidbody);

        _mover.StartMoveToTarget(_basePosition.transform);

        _mover.TargetAchieved += OnBaseAchieved;
    }

    private void OnResourceAchieved()
    {
        _mover.TargetAchieved -= OnResourceAchieved;
        ConnectResource();
    }

    private void OnBaseAchieved()
    {
        ClearSubscribes();

        Resource DeliveredResource = _targetResource;
        _targetResource = null;

        ResourceDelivered?.Invoke(this, DeliveredResource);

        _rope.SetConnectedBody(null);
        _rope.gameObject.SetActive(false);
    }

    private void ClearSubscribes()
    {
        _mover.TargetAchieved -= OnResourceAchieved;
        _mover.TargetAchieved -= OnBaseAchieved;
    }
}
