using System;
using System.Linq;
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

public class ResourceFinder : MonoBehaviour
{
    [SerializeField] private float _radius = 5f;

    public Resource GetNearResourceByType(ResourceType requiredType)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);

        Resource[] resources = hits
            .Select(hit => hit.gameObject.GetComponent<Resource>())
            .Where(resource => resource != null)
            .ToArray();

        resources = resources
            .OrderBy(resource => Vector3.Distance(transform.position, resource.transform.position))
            .ToArray();
    }
}