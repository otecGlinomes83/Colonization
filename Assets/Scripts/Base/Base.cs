using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ResourceStorage))]
[RequireComponent(typeof(RobotStorage))]
[RequireComponent(typeof(FlagKeeper))]
[RequireComponent(typeof(StorageChecker))]
[RequireComponent(typeof(MeshRenderer))]
public class Base : MonoBehaviour
{
    [SerializeField] private float _taskRate;
    [SerializeField] private float _waitTime;

    private ResourceDatabase _database;
    private ResourceStorage _resourceStorage;
    private RobotStorage _robotStorage;
    private FlagKeeper _flagKeeper;
    private StorageChecker _storageChecker;

    private WaitForSecondsRealtime _waitCooldown;
    private WaitForSecondsRealtime _cooldown;

    private MeshRenderer _meshRenderer;

    private bool _isAbleToTask = true;
    private bool _isFlagPlaced = false;

    public event Action<Robot> SpawnAbled;

    private void Awake()
    {
        _resourceStorage = GetComponent<ResourceStorage>();
        _robotStorage = GetComponent<RobotStorage>();
        _flagKeeper = GetComponent<FlagKeeper>();
        _storageChecker = GetComponent<StorageChecker>();
        _meshRenderer = GetComponent<MeshRenderer>();

        _cooldown = new WaitForSecondsRealtime(_waitTime);
    }

    private void OnDisable()
    {
        _flagKeeper.FlagPlaced -= OnFlagPlaced;
        _flagKeeper.FlagDestroyed -= OnFlagDestroyed;
    }

    private void Start()
    {
        StartCoroutine(CooldownTask());
    }

    public void Initialize(Robot initialRobot, ResourceDatabase database)
    {
        _robotStorage.AddRobotToList(initialRobot);
        _database = database;

        _storageChecker.Initialize(_resourceStorage, _robotStorage);
        _flagKeeper.Initialize(_storageChecker, _meshRenderer);

        _flagKeeper.FlagPlaced += OnFlagPlaced;
        _flagKeeper.FlagDestroyed += OnFlagDestroyed;
    }

    private IEnumerator CooldownTask()
    {
        while (enabled)
        {
            yield return _cooldown;

            if (_isAbleToTask == false)
                continue;

            if (_robotStorage.TryGetFreeRobot(out Robot robot) == false)
                continue;

            if (TryGetResource(out Resource resource) == false)
            {
                _robotStorage.AddFreeRobot(robot);
                continue;
            }

            robot.Initialize(transform);
            robot.CollectResource(resource, OnResourceDelivered);
        }
    }

    private bool TryGetResource(out Resource resource)
    {
        resource = null;

        if (_resourceStorage.TryGetNeededResourceType(out ResourceType type))
        {
            if (_database.TryGetResourceByType(out Resource foundResource, type))
            {
                resource = foundResource;

                return true;
            }
            else
            {
                _resourceStorage.TryCancelGettingResourceByType(type);
            }
        }

        return false;
    }

    private void OnResourceDelivered(Robot robot, Resource resource)
    {
        _robotStorage.AddFreeRobot(robot);
        _resourceStorage.AddResource(resource);

        if (_isFlagPlaced)
            return;

        if (_storageChecker.IsAbleToCreateRobot())
        {
            _robotStorage.CreateNewRobot(transform);
            _resourceStorage.SpendResources(_storageChecker.ResourcesForRobot);
        }
    }

    private void OnFlagPlaced()
    {
        if (_isFlagPlaced)
            return;

        _isFlagPlaced = true;

        StartCoroutine(StartCreatingBase());
    }

    private void OnFlagDestroyed()
    {
        _isFlagPlaced = false;
        _isAbleToTask = true;
    }

    private IEnumerator StartCreatingBase()
    {
        while (_storageChecker.IsEnoughForBase() == false)
        {
            yield return _waitCooldown;
        }

        StartCoroutine(CooldownGetFreeRobot());

        yield break;
    }

    private IEnumerator CooldownGetFreeRobot()
    {
        Robot robot = null;

        _isAbleToTask = false;

        while (robot == null)
        {
            yield return _waitCooldown;
            _robotStorage.TryGetFreeRobot(out robot);
        }

        SendRobotToFlag(robot);

        yield break;
    }

    private void SendRobotToFlag(Robot freeRobot)
    {
        if (_flagKeeper.TryGetFlagPosition(out Transform flagPosition))
            freeRobot.MoveTo(flagPosition, OnRobotFlagAchieved);

        _resourceStorage.SpendResources(_storageChecker.ResourcesForBase);
    }

    private void OnRobotFlagAchieved(Robot robot)
    {
        _robotStorage.TryRemoveRobotFromList(robot);
        _flagKeeper.DestroyFlag();
        SpawnAbled?.Invoke(robot);
    }
}
