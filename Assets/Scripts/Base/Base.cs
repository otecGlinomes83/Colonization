using System;
using System.Collections;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private RobotStorage _robotStorage;
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private ResourceDatabase _database;
    [SerializeField] private ClickDetector _clickDetector;

    [SerializeField] private float _callRate;

    private void OnEnable()
    {
        _clickDetector.BaseClicked += OnBaseClicked;
        _storage.EnoughForBase += OnEnoughForBase;
    }

    private void OnDisable()
    {
        _clickDetector.BaseClicked -= OnBaseClicked;
    }

    private void Start()
    {
        StartCoroutine(CooldownResourceTask());
    }

    private IEnumerator CooldownResourceTask()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(_callRate);

        while (enabled)
        {
            yield return cooldown;

            if (_robotStorage.TryGetFreeRobot(out Robot robot) == false)
                continue;

            if (_storage.IsFull || TryGetResource(out Resource resource) == false)
            {
                ReturnRobotToStorage(robot);
                continue;
            }

            robot.SetResource(resource);
            robot.ResourceDelivered += OnResourceDelivered;
        }
    }

    private bool TryGetResource(out Resource resource)
    {
        resource = null;

        if (_storage.TryGetNeededResourceType(out ResourceType type))
        {
            if (_database.TryGetResourceByType(out Resource nearestResource, type))
            {
                resource = nearestResource;

                return true;
            }
            else
            {
                _storage.TryCancelGettingResourceByType(type);
            }
        }

        return false;
    }

    private void OnResourceDelivered(Robot robot, Resource resource)
    {
        robot.ResourceDelivered -= OnResourceDelivered;

        ReturnRobotToStorage(robot);
        _storage.AddResource(resource);
    }

    private void ReturnRobotToStorage(Robot robot)
    {
        _robotStorage.AddFreeRobot(robot);
    }

    private void OnBaseClicked()
    {
        _storage.SwitchPriority(StoragePriority.Base);
    }

    private void OnEnoughForBase()
    {
        _flag
    }
}
