using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    [SerializeField] private SpringJoint _springJoint;

    private Transform _to;

    private LineRenderer _lineRenderer;

    private bool _isConnected = false;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnDisable()
    {
        _isConnected = false;
    }

    private void Update()
    {
        if (_isConnected)
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _to.position);
        }
    }

    public void SetConnectedBody(Rigidbody target)
    {
        if (target == null)
        {
            _isConnected = false;
            return;
        }

        _springJoint.connectedBody = target;
        _to = target.transform;
        _isConnected = true;
    }
}
