using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Robot : MonoBehaviour
{
    [SerializeField] private Resource _resource;
    [SerializeField] private SpringJoint _springJoint;
    [SerializeField] private Transform _basePosition;
    [SerializeField] private Rope _rope;
    [SerializeField] private ResourceDetector _detector;

    private Coroutine _moveToResourceCoroutine;
    private Mover _mover;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
    }

    private void Start()
    {
        _rope.gameObject.SetActive(false);
        _detector.SetResource(_resource);

        _mover.StartMoveToTarget(_resource.transform);
    }

    private void OnEnable()
    {
        _detector.ResourceDetected += MoveToBase;
    }

    private void OnDisable()
    {
        _detector.ResourceDetected -= MoveToBase;
    }

    private void MoveToBase()
    {
        ConnectResource();
        _mover.StartMoveToTarget(_basePosition.transform);
    }

    private void ConnectResource()
    {
        _springJoint.connectedBody = _resource.GetComponent<Rigidbody>();
        _rope.gameObject.SetActive(true);
    }
}
