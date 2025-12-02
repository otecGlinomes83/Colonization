using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    [SerializeField] private int _startCount = 0;

    private List<ResourceParameters> _resources;

    public event Action<ResourceType, int> CountChanged;

    public IReadOnlyList<ResourceParameters> Resources => _resources.AsReadOnly();

    private void Awake()
    {
        _resources = new List<ResourceParameters>
        {
            new ResourceParameters(ResourceType.Metal,_startCount),
            new ResourceParameters(ResourceType.Plastic,_startCount),
            new ResourceParameters(ResourceType.Wires,_startCount)
        };
    }

    public void AddResource(Resource resourceToAdd)
    {
        int index = _resources.FindIndex(resource => resource.Type == resourceToAdd.Type);

        if (index >= 0)
        {
            ResourceParameters parameter = _resources[index];

            parameter.IncreaseCount();
            parameter.DecreaseExpectedCount();

            _resources[index] = parameter;

            CountChanged?.Invoke(resourceToAdd.Type, _resources[index].CurrentCount);
        }

        resourceToAdd.Release();
    }

    public bool TryCancelGettingResourceByType(ResourceType type)
    {
        int index = _resources.FindIndex(r => r.Type == type);

        if (index >= 0)
        {
            ResourceParameters parameter = _resources[index];

            parameter.DecreaseExpectedCount();

            _resources[index] = parameter;

            return true;
        }

        return false;
    }

    public bool TryGetNeededResourceType(out ResourceType type)
    {
        type = 0;

        List<ResourceParameters> neededResources = _resources
            .OrderBy(resource => resource.ExpectedCount + resource.CurrentCount)
            .ToList();

        if (neededResources.Count != 0)
        {
            int index = _resources.FindIndex(resource => resource.Type == neededResources.First().Type);

            if (index >= 0)
            {
                ResourceParameters parameter = _resources[index];

                parameter.IncreaseExpectedCount();
                _resources[index] = parameter;

                type = _resources[index].Type;

                return true;
            }
        }

        return false;
    }

    public void SpendResources(int count)
    {
        for (int i = 0; i < _resources.Count; i++)
        {
            ResourceParameters parameter = _resources[i];

            parameter.DecreaseCount(count);
            _resources[i] = parameter;
            CountChanged?.Invoke(parameter.Type, _resources[i].CurrentCount);
        }
    }
}
