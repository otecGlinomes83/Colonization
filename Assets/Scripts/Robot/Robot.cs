using System;
using UnityEngine;

[RequireComponent(typeof(BaseBuilder))]
[RequireComponent(typeof(Mover))]
public class Robot : MonoBehaviour
{
    [SerializeField] private Rope _rope;

    private Mover _mover;
    private Resource _targetResource;
    private BaseBuilder _builder;
    private Transform _endPosition;

    public event Action<Robot, Resource> ResourceDelivered;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _builder = GetComponent<BaseBuilder>();
        _rope.gameObject.SetActive(false);
    }

    public void Initialize(Transform endPosition)
    {
        _endPosition = endPosition;
    }

    public void WentToFlag(Transform flagPosition)
    {
        Debug.Log("Going to flag!");
        _endPosition = flagPosition;

        _mover.StartMoveToTarget(flagPosition);
        _mover.TargetAchieved += OnFlagAchieved;
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
        Debug.Log("Creating base");
        _builder.CreateNewBase(this);
    }

    private void ClearSubscribes()
    {
        _mover.TargetAchieved -= OnResourceAchieved;
        _mover.TargetAchieved -= OnEndPositionAchieved;
    }
}
