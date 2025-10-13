using System.Linq;
using UnityEngine;

public class ResourceFinder : MonoBehaviour
{
    [SerializeField] private float _radius = 5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (_radius > 0f)
        {
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }

    public bool TryGetNearResourceByType(ResourceType requiredType, out Resource nearestResource)
    {
        nearestResource = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);

        Resource[] resources = hits
            .Select(hit => hit.gameObject.GetComponent<Resource>())
            .Where(resource => resource != null)
            .OrderBy(resource => Vector3.Distance(transform.position, resource.transform.position))
            .Where(resource => resource.Type == requiredType)
            .ToArray();

        if (resources.Count() == 0)
            return false;

        nearestResource = resources.First();
        return true;
    }
}