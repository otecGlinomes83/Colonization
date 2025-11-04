using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(ClickDetector))]
[RequireComponent(typeof(FlagKeeper))]
[RequireComponent(typeof(ResourceStorage))]
[RequireComponent(typeof(RobotStorage))]

public class Base : MonoBehaviour
{
    [SerializeField] private ResourceDatabase _database;
    [SerializeField] private float _callRate;

    private Coroutine _resourceTaskCoroutine;

    private ResourceStorage _storage;
    private FlagKeeper _flagKeeper;
    private ClickDetector _clickDetector;
    private RobotStorage _robotStorage;

    private MeshRenderer _meshRenderer;

    private ColorChanger _colorChanger = new ColorChanger();

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
        _resourceTaskCoroutine = StartCoroutine(CooldownResourceTask());
    }

    public void Initialize(Robot initialRobot)
    {
        _robotStorage.AddRobotToList(initialRobot);
    }

    private IEnumerator CooldownResourceTask()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(_callRate);

        while (enabled)
        {
            yield return cooldown;

            if (_robotStorage.TryGetFreeRobot(out Robot robot) == false)
            {
                continue;
            }

            if (_storage.IsFull() || TryGetResource(out Resource resource) == false)
            {
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

    private void TryStartFlagPlacement()
    {
        if (_robotStorage.IsAbleToCreateBase == false)
            return;

        _colorChanger.ChangeColor(_meshRenderer, Color.red);

        _flagKeeper.StartPlacement();
        _flagKeeper.Placed += OnFlagPlaced;
    }

    private void OnFlagPlaced()
    {
        _colorChanger.TryChangeColorToDefault(_meshRenderer);

        _flagKeeper.Placed -= OnFlagPlaced;
        _storage.SwitchPriority(StoragePriority.Base);
        Debug.LogWarning("Base has new base as priority");
        _storage.EnoughForBase += CreateBase;
    }

    private void CreateBase()
    {
        Debug.LogWarning("Creating new Base");

        _storage.EnoughForBase -= CreateBase;
        _storage.SwitchPriority(StoragePriority.Robot);

        StopCoroutine(_resourceTaskCoroutine);
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

        yield return null;

        while (robot == null)
        {
            Debug.Log("Trying get free robot");
            _robotStorage.TryGetFreeRobot(out robot);
        }

        SendRobotToFlag(robot);
    }

    private void SendRobotToFlag(Robot robot)
    {
        if (_flagKeeper.TryGetFlagPosition(out Transform flagPosition))
        {
            robot.WentToFlag(flagPosition);
            _robotStorage.TryRemoveRobotFromList(robot);
            Debug.Log("robot has been send to flag");
        }
    }
}
