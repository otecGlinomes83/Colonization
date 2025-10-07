using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Robot : MonoBehaviour
{
    [SerializeField] private SpringJoint _springJoint;
    [SerializeField] private Transform _basePosition;
    [SerializeField] private Rope _rope;
    [SerializeField] private ResourceDetector _detector;

    private Resource _resource;
    private Mover _mover;

    private bool _isFree = true;

    public bool IsFree => _isFree;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _rope.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _detector.ResourceDetected += ConnectResource;
        _resource.ReadyForRelease += () => _rope.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _detector.ResourceDetected -= ConnectResource;
        _resource.ReadyForRelease -= () => _rope.gameObject.SetActive(false);
    }

    public void GetResource(Resource resource)
    {
        _isFree = false;
        _resource = resource;

        _detector.SetResource(_resource);
        _mover.StartMoveToTarget(_resource.transform);
    }

    private void ConnectResource()
    {
        _mover.StopMoveTo();
        _springJoint.connectedBody = _resource.Rigidbody;
        _rope.gameObject.SetActive(true);

        _mover.StartMoveToTarget(_basePosition.transform);
    }
}
