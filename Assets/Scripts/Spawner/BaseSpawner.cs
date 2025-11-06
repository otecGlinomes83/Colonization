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

        createdBase.Initialize(initialRobot, _resourceDatabase);
        createdBase.transform.position = initialRobot.transform.position;
        createdBase.SpawnAbled += CreateBase;

        TryAddBase(createdBase);
    }

    private void TryAddBase(Base @base)
    {
        if (_createdBases.Contains(@base) == false)
            return;

        _createdBases.Add(@base);
    }

    private void UnsubscribeAll()
    {
        foreach (Base @base in _createdBases)
        {
            @base.SpawnAbled -= CreateBase;
        }
    }
}
