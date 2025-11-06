using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(ClickDetector))]
[RequireComponent(typeof(FlagKeeper))]
[RequireComponent(typeof(ResourceStorage))]
[RequireComponent(typeof(RobotStorage))]

public class Base : MonoBehaviour
{
    [SerializeField] private float _callRate;

    private ResourceDatabase _database;
    private ResourceStorage _storage;
    private FlagKeeper _flagKeeper;
    private ClickDetector _clickDetector;
    private RobotStorage _robotStorage;

    private MeshRenderer _meshRenderer;

    private ColorChanger _colorChanger = new ColorChanger();

    private bool _isFlagPlaced = false;

    public event Action<Robot> SpawnAbled;

    private void Awake()
    {
        _storage = GetComponent<ResourceStorage>();
        _flagKeeper = GetComponent<FlagKeeper>();
        _clickDetector = GetComponent<ClickDetector>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _robotStorage = GetComponent<RobotStorage>();

        _colorChanger.SetDefaultColor(_meshRenderer.material.color);
    }

    private void OnEnable()
    {
        _clickDetector.BaseClicked += TryStartFlagPlacement;
        _storage.EnoughForRobot += TryCreateNewRobot;
    }

    private void OnDisable()
    {
        _clickDetector.BaseClicked -= TryStartFlagPlacement;
    }

    private void Start()
    {
        StartCoroutine(CooldownResourceTask());
    }

    public void Initialize(Robot initialRobot, ResourceDatabase database)
    {
        _robotStorage.AddRobotToList(initialRobot);
        _database = database;
    }

    private IEnumerator CooldownResourceTask()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(_callRate);

        while (enabled)
        {
            yield return cooldown;

            Debug.Log($"{gameObject.name} tasking");

            if (_robotStorage.TryGetFreeRobot(out Robot robot) == false)
            {
                Debug.LogError($"{gameObject.name} cant get free robot");               
                continue;
            }

            if (_storage.IsFull() || TryGetResource(out Resource resource) == false)
            {
                Debug.LogError($"{gameObject.name} storage is full");
                ReturnRobotToStorage(robot);
                continue;
            }

            robot.Initialize(transform);
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

    private void TryStartFlagPlacement(Base detectedBase)
    {
        if (_robotStorage.IsAbleToCreateBase == false && this == detectedBase)
            return;

        _colorChanger.ChangeColor(_meshRenderer, Color.red);

        _flagKeeper.StartPlacement();
        _flagKeeper.Placed += OnFlagPlaced;
    }

    private void OnFlagPlaced()
    {
        if (_isFlagPlaced == false)
            _storage.EnoughForBase += StartCreatingBase;

        _flagKeeper.Placed -= OnFlagPlaced;
        _colorChanger.TryChangeColorToDefault(_meshRenderer);
        _storage.SwitchPriority(StoragePriority.Base);
        _isFlagPlaced = true;
    }

    private void StartCreatingBase()
    {
        _storage.EnoughForBase -= StartCreatingBase;
        _storage.SwitchPriority(StoragePriority.Robot);

        StartCoroutine(CooldownGetFreeRobot());
    }

    private void TryCreateNewRobot()
    {
        if (_robotStorage.IsAbleToCreateRobot)
        {
            _storage.SpendResources();
            _robotStorage.CreateNewRobot(transform);
        }
    }

    private IEnumerator CooldownGetFreeRobot()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(0.25f);

        Robot robot = null;

        while (robot == null)
        {
            yield return null;
            _robotStorage.TryGetFreeRobot(out robot);
        }

        SendRobotToFlag(robot);

        yield break;
    }

    private void SendRobotToFlag(Robot robot)
    {
        if (_flagKeeper.TryGetFlagPosition(out Transform flagPosition))
        {
            robot.WentToFlag(flagPosition);
            robot.FlagAchieved += OnRobotAchievedFlag;
            _robotStorage.TryRemoveRobotFromList(robot);
        }
    }

    private void OnRobotAchievedFlag(Robot robot)
    {
        robot.FlagAchieved -= OnRobotAchievedFlag;
        _flagKeeper.DestroyFlag();
        _isFlagPlaced = false;

        SpawnAbled?.Invoke(robot);
    }
}
