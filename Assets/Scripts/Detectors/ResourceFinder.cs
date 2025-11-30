using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceFinder : MonoBehaviour
{
    [SerializeField] private float _radius = 5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (_radius < 0f)
            return;

        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public bool TryFindResources(out List<Resource> resources)
    {
        resources = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);

        List<Resource> foundResources = hits
            .Select(hit => hit.gameObject.GetComponent<Resource>())
            .Where(resource => resource != null)
            .ToList();

        if (foundResources.Count() == 0)
            return false;

        resources = foundResources;

        return true;
    }
}