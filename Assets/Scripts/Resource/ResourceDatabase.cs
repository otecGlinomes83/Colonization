using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceDatabase : MonoBehaviour
{
    [SerializeField] private ResourceFinder _finder;

    private List<Resource> _foundResources = new List<Resource>();
    private List<Resource> _reservedResources = new List<Resource>();

    public bool TryGetResourceByType(out Resource resource, ResourceType requiredType)
    {
        resource = null;

        if (_finder.TryFindResources(out _foundResources))
        {
            List<Resource> resourcesOneType = _foundResources
                .Where(resource => resource.Type == requiredType && _reservedResources.Contains(resource) == false).ToList();

            if (resourcesOneType.Count > 0)
            {
                resource = resourcesOneType[0];
                TryReserveResource(resource);
                resource.ReadyForRelease += TryUnReserveResource;
                return true;
            }
        }

        return false;
    }

    private void TryReserveResource(Resource resource)
    {
        if (_reservedResources.Contains(resource))
            return;

        _reservedResources.Add(resource);
    }

    private void TryUnReserveResource(Resource resource)
    {
        resource.ReadyForRelease -= TryUnReserveResource;

        if (_reservedResources.Contains(resource) == false)
            return;

        _reservedResources.Remove(resource);
    }
}