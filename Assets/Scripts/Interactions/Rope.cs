using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    private Transform _from;
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
            _lineRenderer.SetPosition(0, _from.position);
            _lineRenderer.SetPosition(1, _to.position);
        }
    }

    public void Initialize(Transform from, Transform to)
    {
        _from = from;
        _to = to;
        _isConnected = true;    
    }
}
