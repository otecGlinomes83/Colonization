using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceFinder : MonoBehaviour
{
    [SerializeField] private float _radius = 5f;
    [SerializeField] private int _maxFoundResources = 25;

    private List<Resource> _foundResources;

    private Collider[] _collidersBuffer;

    private void Awake()
    {
        _collidersBuffer = new Collider[_maxFoundResources];
        _foundResources = new List<Resource>(_maxFoundResources);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (_radius < 0f)
            return;

        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public bool TryFindResources(out List<Resource> resources)
    {
        _foundResources.Clear();

        int hitsCount = Physics.OverlapSphereNonAlloc(transform.position, _radius, _collidersBuffer);

        if (hitsCount <= 0)
        {
            resources = null;

            return false;
        }

        for (int i = 0; i < hitsCount; i++)
        {
            if (_collidersBuffer[i].TryGetComponent(out Resource resource))
                _foundResources.Add(resource);
        }

        if (_foundResources.Count == 0)
        {
            resources = null;

            return false;
        }

        resources = _foundResources;

        return true;
    }
}