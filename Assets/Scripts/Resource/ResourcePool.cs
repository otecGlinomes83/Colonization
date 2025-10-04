using System;
using UnityEngine;
using UnityEngine.Pool;

public class ResourcePool : MonoBehaviour
{
    [SerializeField] private Resource _resourcePrefab;

    private ObjectPool<Resource> _resourcePool;

    private void Awake()
    {
        _resourcePool = new ObjectPool<Resource>
            (
            createFunc: () => OnCreate(),
            actionOnGet: (resource) => OnGet(resource)
            );
    }

    public Resource GetResource()
    {
        return _resourcePool.Get();
    }

    private Resource OnCreate()
    {
        return Instantiate(_resourcePrefab);
    }

    private void OnGet(Resource resource)
    {

    }
}
