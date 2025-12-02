using System.Collections.Generic;
using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private ResourceDatabase _resourceDatabase;
    [SerializeField] private Robot _initialRobot;
    [SerializeField] private Base _basePrefab;

    private List<Base> _createdBases = new List<Base>();

    private void Awake()
    {
        CreateBase(_initialRobot);
    }

    private void OnDisable()
    {
        UnsubscribeAll();
    }

    private void CreateBase(Robot initialRobot)
    {
        Base createdBase = Instantiate(_basePrefab);

        createdBase.transform.position = initialRobot.transform.position;
        createdBase.Initialize(initialRobot, _resourceDatabase);
        createdBase.SpawnAbled += CreateBase;

        TryAddBase(createdBase);
    }

    private void TryAddBase(Base baseToAdd)
    {
        if (_createdBases.Contains(baseToAdd) == false)
            return;

        _createdBases.Add(baseToAdd);
    }

    private void UnsubscribeAll()
    {
        foreach (Base baseToUnsubscribe in _createdBases)
        {
            baseToUnsubscribe.SpawnAbled -= CreateBase;
        }
    }
}
