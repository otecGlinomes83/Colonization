using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    [SerializeField] private int _maxMetalCount = 3;
    [SerializeField] private int _maxPlasticCount = 3;
    [SerializeField] private int _maxWiresCount = 3;

    [SerializeField] private int _startMetalCount = 0;
    [SerializeField] private int _startPlasticCount = 0;
    [SerializeField] private int _startWiresCount = 0;

    private int _currentResourceCount = 0;
    private int _maxTotalResourceCount;

    private List<ResourceParameters> _resources;

    public event Action<ResourceType, int> CountChanged;

    public bool IsFull => _currentResourceCount >= _maxTotalResourceCount;

    private void Awake()
    {
        _resources = new List<ResourceParameters>
        {
            new ResourceParameters(ResourceType.Metal,_startMetalCount,_maxMetalCount),
            new ResourceParameters(ResourceType.Plastic,_startPlasticCount,_maxPlasticCount),
            new ResourceParameters(ResourceType.Wires,_startWiresCount,_maxWiresCount)
        };

        _maxTotalResourceCount = _resources.Sum(resource => resource.MaxCount);
    }

    public void AddResource(Resource resourceToAdd)
    {
        int index = _resources.FindIndex(resource => resource.Type == resourceToAdd.Type);

        if (index >= 0)
        {
            ResourceParameters tempParameter = _resources[index];

            tempParameter.CurrentCount++;
            tempParameter.ExpectedCount--;
            _currentResourceCount++;

            _resources[index] = tempParameter;

            CountChanged?.Invoke(resourceToAdd.Type, _resources[index].CurrentCount);
        }

        resourceToAdd.Release();
    }

    public bool TryCancelGettingResourceByType(ResourceType type)
    {
        int index = _resources.FindIndex(r => r.Type == type);

        if (index >= 0)
        {
            ResourceParameters tempParameters = _resources[index];
            tempParameters.ExpectedCount--;
            _resources[index] = tempParameters;

            return true;
        }

        return false;
    }

    public bool TryGetNeededResourceType(out ResourceType type)
    {
        type = 0;

        List<ResourceParameters> neededResources = _resources
            .Where(resource => resource.ExpectedCount + resource.CurrentCount < resource.MaxCount)
            .OrderBy(resource => resource.ExpectedCount)
            .ThenBy(resource => resource.CurrentCount)
            .ToList();

        if (neededResources.Count != 0)
        {
            int targetIndex = _resources.FindIndex(resource => resource.Type == neededResources.First().Type);

            if (targetIndex >= 0)
            {
                ResourceParameters tempParameters = _resources[targetIndex];
                tempParameters.ExpectedCount++;
                _resources[targetIndex] = tempParameters;

                type = _resources[targetIndex].Type;

                return true;
            }
        }

        return false;
    }
}
