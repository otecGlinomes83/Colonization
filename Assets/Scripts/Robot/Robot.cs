using System;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField] private Transform _basePosition;
    [SerializeField] private Rope _rope;
    [SerializeField] private ResourceDetector _resourceDetector;
    [SerializeField] private Mover _mover;

    private Resource _targetResource;

    public event Action<Robot> Freed;

    private void Awake()
    {
        _rope.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _resourceDetector.Detected += OnResourceDetected;
    }

    private void OnDisable()
    {
        _resourceDetector.Detected -= ConnectResource;
    }

    public void SetResource(Resource resource)
    {
        resource.SetReserve(true);
        _targetResource = resource;
        _targetResource.ReadyForRelease += OnResourceReadyForRelease;
        _mover.StartMoveToTarget(_targetResource.transform);
    }

    private void OnResourceReadyForRelease(Resource resource)
    {
        _targetResource.ReadyForRelease -= OnResourceReadyForRelease;
        _rope.SetConnectedBody(null);

        resource.SetReserve(false);
        _rope.gameObject.SetActive(false);
        Freed?.Invoke(this);
    }

    private void ConnectResource(Resource resource)
    {
        _mover.StopMoveTo();

        _rope.gameObject.SetActive(true);
        _rope.SetConnectedBody(resource.Rigidbody);

        _mover.StartMoveToTarget(_basePosition.transform);
    }

    private void OnResourceDetected(Resource resource)
    {
        if (resource == _targetResource)
            ConnectResource(resource);
    }
}
