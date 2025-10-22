using System.Collections;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private RobotStorage _robotStorage;
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private ResourceDatabase _database;
    [SerializeField] private float _callRate;

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

            if(_robotStorage.TryGetFreeRobot(out Robot robot)==false)
                continue;

            if(_storage.IsFull||TryGetResource(out Resource resource) == false) 
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
                Debug.LogError($"Cancel Getting resource {type}!");
                _storage.TryCancelGettingResourceByType(type);
            }
        }

        return false;
    }

    private void OnResourceDelivered(Resource resource)
    {
        _storage.AddResource(resource);
    }

    private void ReturnRobotToStorage(Robot robot)
    {
        robot.ResourceDelivered -= OnResourceDelivered;
        _robotStorage.AddFreeRobot(robot);
    }
}
