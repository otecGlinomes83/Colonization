using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ResourceDetector : MonoBehaviour
{
    public event Action ResourceDetected;

    private BoxCollider _boxCollider;
    private Resource _resource;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
    }

    public void SetResource(Resource resource)
    {
        _resource = resource;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_resource == null)
            return;

        if (other.gameObject.TryGetComponent(out Resource resource))
            if (resource == _resource)
                ResourceDetected?.Invoke();
    }
}
