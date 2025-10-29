using System;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField] private Transform _endPosition;
    [SerializeField] private Rope _rope;
    [SerializeField] private Mover _mover;

    private Resource _targetResource;

    public event Action<Robot, Resource> ResourceDelivered;
    public event Action<Robot> FlagAchieved;

    private void Awake()
    {
        _rope.gameObject.SetActive(false);
    }

    public void Initialize(Transform endPosition)
    {
        _endPosition = endPosition;
    }

    public void WentToNewBase(Transform flagPosition)
    {
        _endPosition = flagPosition;
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

        _mover.StartMoveToTarget(_endPosition.transform);

        _mover.TargetAchieved += OnEndPositionAchieved;
    }

    private void OnResourceAchieved()
    {
        _mover.TargetAchieved -= OnResourceAchieved;
        ConnectResource();
    }

    private void OnEndPositionAchieved()
    {
        ClearSubscribes();

        Resource DeliveredResource = _targetResource;
        _targetResource = null;

        ResourceDelivered?.Invoke(this, DeliveredResource);

        _rope.SetConnectedBody(null);
        _rope.gameObject.SetActive(false);
    }

    private void OnFlagAchieved()
    {
        FlagAchieved?.Invoke(this);
    }

    private void ClearSubscribes()
    {
        _mover.TargetAchieved -= OnResourceAchieved;
        _mover.TargetAchieved -= OnEndPositionAchieved;
    }
}
