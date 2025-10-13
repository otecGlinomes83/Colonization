using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ResourceDetector : MonoBehaviour
{
    public event Action<Resource> Detected;

    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Resource resource))
            Detected?.Invoke(resource);
    }
}
