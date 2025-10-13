using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    private int _startMetalCount = 0;
    private int _startPlasticCount = 0;
    private int _startWiresCount = 0;

    private int _maxMetalCount = 3;
    private int _maxPlasticCount = 3;
    private int _maxWiresCount = 3;

    private int _currentResourceCount = 0;
    private int _maxTotalResourceCount;

    private List<ResourceParameters> _resources;
    private List<ResourceParameters> _expectedResources;

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

    public void AddResource(Resource resource)
    {
        int index = _resources.FindIndex(x => x.Type == resource.Type);

        if (index >= 0)
        {
            ResourceParameters parameter = _resources[index];

            parameter.CurrentCount++;
            _currentResourceCount++;

            _resources[index] = parameter;

            CountChanged?.Invoke(resource.Type, _resources[index].CurrentCount);
            resource.Release();
        }
    }

    public bool TryGetNeededResourceType(out ResourceType type)
    {
        type = default;

        List<ResourceParameters> neededResources = _resources
            .Where(resource => resource.CurrentCount < resource.MaxCount)
            .OrderBy(resource => resource.CurrentCount)
            .ToList();

        if (neededResources.Count == 0)
            return false;

        type = neededResources.First().Type;
        Debug.Log($"Current needed type is {type}");

        return true;
    }
}

public struct ExpectedResourceParameters
{
    private ResourceType _type;
    private int _count;

    public ResourceType Type => _type;
    public int Count => _count;

    public ExpectedResourceParameters(ResourceType type, int count)
    {
        _type = type;
        _count = count;
    }
}
