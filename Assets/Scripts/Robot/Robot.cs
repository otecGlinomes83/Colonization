using System;
using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Robot : MonoBehaviour
{
    [SerializeField] private SpringJoint _springJoint;
    [SerializeField] private Transform _basePosition;
    [SerializeField] private Rope _rope;
    [SerializeField] private ResourceDetector _resourceDetector;

    private Resource _targetResource;
    private Mover _mover;

    public event Action<Robot> Freed;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
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
        Debug.Log($"Robot has {resource.Type} as target");
        resource.SetReserve(true);
        _targetResource = resource;
        _targetResource.ReadyForRelease += OnResourceReadyForRelease;
        _mover.StartMoveToTarget(_targetResource.transform);
    }

    private void OnResourceReadyForRelease(Resource resource)
    {
        _targetResource.ReadyForRelease -= OnResourceReadyForRelease;
        _springJoint.connectedBody = null;

        resource.SetReserve(false);
        _rope.gameObject.SetActive(false);
        Freed?.Invoke(this);
    }

    private void ConnectResource(Resource resource)
    {
        _mover.StopMoveTo();
        _springJoint.connectedBody = resource.Rigidbody;
        _rope.gameObject.SetActive(true);
        _rope.Initialize(transform, resource.transform);
        _mover.StartMoveToTarget(_basePosition.transform);
    }

    private void OnResourceDetected(Resource resource)
    {
        if (resource == _targetResource)
            ConnectResource(resource);
    }
}
