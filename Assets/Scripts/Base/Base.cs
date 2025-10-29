using System.Collections;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private RobotStorage _robotStorage;
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private ResourceDatabase _database;
    [SerializeField] private ClickDetector _clickDetector;
    [SerializeField] private FlagKeeper _flagKeeper;

    [SerializeField] private float _callRate;

    private Coroutine _resourceTaskCoroutine;

    private void OnEnable()
    {
        _clickDetector.BaseClicked += OnBaseClicked;
    }

    private void OnDisable()
    {
        _clickDetector.BaseClicked -= OnBaseClicked;
    }

    private void Start()
    {
        _resourceTaskCoroutine = StartCoroutine(CooldownResourceTask());
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
        _flagKeeper.StartPlacement();
        _flagKeeper.Placed += OnFlagPlaced;
    }

    private void OnFlagPlaced()
    {
        _flagKeeper.Placed -= OnFlagPlaced;
        _storage.SwitchPriority(StoragePriority.Base);
        _storage.EnoughForBase += OnEnoughForBase;
    }

    private void OnEnoughForBase()
    {
        _storage.EnoughForBase -= OnEnoughForBase;

        _storage.SwitchPriority(StoragePriority.Robot);

        StopCoroutine(_resourceTaskCoroutine);

        StartCoroutine(CooldownGetFreeRobot());
    }

    private IEnumerator CooldownGetFreeRobot()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(0.25f);

        Robot robot = null;

        yield return null;

        while (robot == null)
        {
            _robotStorage.TryGetFreeRobot(out robot);
        }

        if (_flagKeeper.TryGetFlagPosition(out Transform flagPosition))
            robot.WentToNewBase(flagPosition);
    }
}
