using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(ClickDetector))]
[RequireComponent(typeof(FlagPlacer))]
[RequireComponent(typeof(ResourceStorage))]
[RequireComponent(typeof(RobotStorage))]
public class Base : MonoBehaviour
{
    [SerializeField] private float _callRate;

    private ResourceDatabase _database;
    private ResourceStorage _storage;
    private FlagPlacer _flagPlacer;
    private ClickDetector _clickDetector;
    private RobotStorage _robotStorage;

    private MeshRenderer _meshRenderer;
    WaitForSecondsRealtime _cooldown;

    private ColorChanger _colorChanger = new ColorChanger();
    private bool _isFlagPlaced = false;
    private bool _isAbleToTask = true;

    public event Action<Robot> SpawnAbled;

    private void Awake()
    {
        _storage = GetComponent<ResourceStorage>();
        _flagPlacer = GetComponent<FlagPlacer>();
        _clickDetector = GetComponent<ClickDetector>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _robotStorage = GetComponent<RobotStorage>();

        _cooldown = new WaitForSecondsRealtime(_callRate);

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
        _storage.EnoughForRobot -= TryCreateNewRobot;
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
        while (enabled)
        {
            yield return _cooldown;

            if (_isAbleToTask == false)
                continue;

            if (_robotStorage.TryGetFreeRobot(out Robot robot) == false)
                continue;

            if (_storage.IsFull() || TryGetResource(out Resource resource) == false)
            {
                _robotStorage.AddFreeRobot(robot);
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

        _robotStorage.AddFreeRobot(robot);
        _storage.AddResource(resource);
    }

    private void TryStartFlagPlacement(Base detectedBase)
    {
        if (_robotStorage.IsAbleToCreateBase() == false)
            return;

        if (this != detectedBase)
            return;

        _colorChanger.ChangeColor(_meshRenderer, Color.red);

        _flagPlacer.StartPlacement();
        _flagPlacer.Placed += OnFlagPlaced;
    }

    private void OnFlagPlaced()
    {
        _flagPlacer.Placed -= OnFlagPlaced;
        _colorChanger.TryChangeColorToDefault(_meshRenderer);

        if (_robotStorage.IsAbleToCreateBase() == false)
        {
            _flagPlacer.DestroyFlag();
            return;
        }

        if (_isFlagPlaced == false)
        {
            _storage.EnoughForBase += StartCreatingBase;
            _storage.SwitchPriority(StoragePriority.Base);
            _isFlagPlaced = true;
        }
    }

    private void StartCreatingBase()
    {
        _storage.EnoughForBase -= StartCreatingBase;
        _isAbleToTask = false;

        StartCoroutine(CooldownGetFreeRobot());
    }

    private void TryCreateNewRobot()
    {
        if (_robotStorage.IsAbleToCreateRobot())
        {
            _storage.SpendResources();
            _robotStorage.CreateNewRobot(transform);
        }
    }

    private IEnumerator CooldownGetFreeRobot()
    {
        Robot robot = null;

        while (robot == null)
        {
            yield return _cooldown;
            _robotStorage.TryGetFreeRobot(out robot);
        }

        SendRobotToFlag(robot);

        yield break;
    }

    private void SendRobotToFlag(Robot robot)
    {
        if (_flagPlacer.GetFlagPosition(out Transform flagPosition))
        {
            robot.WentToFlag(flagPosition);
            robot.FlagAchieved += OnRobotAchievedFlag;
        }
    }

    private void OnRobotAchievedFlag(Robot robot)
    {
        robot.FlagAchieved -= OnRobotAchievedFlag;

        _robotStorage.TryRemoveRobotFromList(robot);
        _flagPlacer.DestroyFlag();
        _isFlagPlaced = false;

        _storage.SpendResources();
        _storage.SwitchPriority(StoragePriority.Robot);
        _isAbleToTask = true;

        SpawnAbled?.Invoke(robot);
    }
}
