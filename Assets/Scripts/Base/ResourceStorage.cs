using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    [SerializeField] private int _maxCountPerBase = 5;
    [SerializeField] private int _maxCountPerRobot = 3;
    [SerializeField] private int _startCount = 0;

    private List<ResourceParameters> _resources;

    private StoragePriority _priority;

    public event Action<ResourceType, int> CountChanged;
    public event Action EnoughForRobot;
    public event Action EnoughForBase;

    private void Awake()
    {
        _priority = StoragePriority.Robot;

        _resources = new List<ResourceParameters>
        {
            new ResourceParameters(ResourceType.Metal,_startCount,_maxCountPerRobot),
            new ResourceParameters(ResourceType.Plastic,_startCount,_maxCountPerRobot),
            new ResourceParameters(ResourceType.Wires,_startCount,_maxCountPerRobot)
        };
    }

    public void AddResource(Resource resourceToAdd)
    {
        int index = _resources.FindIndex(resource => resource.Type == resourceToAdd.Type);

        if (index >= 0)
        {
            ResourceParameters tempParameter = _resources[index];

            tempParameter.CurrentCount++;
            tempParameter.ExpectedCount--;
            _resources[index] = tempParameter;

            CountChanged?.Invoke(resourceToAdd.Type, _resources[index].CurrentCount);
        }

        resourceToAdd.Release();

        CheckResources();
    }

    public bool TryCancelGettingResourceByType(ResourceType type)
    {
        int index = _resources.FindIndex(r => r.Type == type);

        if (index >= 0)
        {
            ResourceParameters tempParameter = _resources[index];
            tempParameter.ExpectedCount--;

            _resources[index] = tempParameter;

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
                ResourceParameters tempParameter = _resources[targetIndex];
                tempParameter.ExpectedCount++;
                _resources[targetIndex] = tempParameter;

                type = _resources[targetIndex].Type;

                return true;
            }
        }

        return false;
    }

    public void SwitchPriority(StoragePriority newPriority)
    {
        _priority = newPriority;

        if (_priority == StoragePriority.Base)
        {
            ChangeResourceLimits(_maxCountPerBase);
        }
        else
        {
            ChangeResourceLimits(_maxCountPerRobot);
        }
    }

    public bool IsFull()
    {
        return _resources.Sum(resource => resource.CurrentCount) >= _resources.Sum(resource => resource.MaxCount);
    }

    public void SpendResources()
    {
        int count = 0;

        if (_priority == StoragePriority.Base)
        {
            count = _maxCountPerBase;
            SwitchPriority(StoragePriority.Robot);
        }
        else
        {
            count = _maxCountPerRobot;
        }

        ResourceParameters tempParameters;

        for (int i = 0; i < _resources.Count; i++)
        {
            tempParameters = _resources[i];
            tempParameters.CurrentCount -= count;
            _resources[i] = tempParameters;
            CountChanged?.Invoke(_resources[i].Type, _resources[i].CurrentCount);
        }
    }

    private void CheckResources()
    {
        if (_priority == StoragePriority.Base && IsEnoughForNewBase())
        {
            EnoughForBase?.Invoke();
            return;
        }

        if (_priority == StoragePriority.Robot && IsEnoughForNewRobot())
        {
            EnoughForRobot?.Invoke();
            return;
        }
    }

    private void ChangeResourceLimits(int newLimit)
    {
        ResourceParameters tempParameters;

        for (int i = 0; i < _resources.Count; i++)
        {
            tempParameters = _resources[i];
            tempParameters.MaxCount = newLimit;
            _resources[i] = tempParameters;
        }
    }

    private bool IsEnoughForNewRobot() =>
    _resources.All(resource => resource.CurrentCount >= resource.MaxCount);

    private bool IsEnoughForNewBase() =>
 _resources.All(resource => resource.CurrentCount >= resource.MaxCount);
}
