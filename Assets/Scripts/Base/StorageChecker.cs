using System.Linq;
using UnityEngine;

public class StorageChecker : MonoBehaviour
{
    [SerializeField] private int _resourcesForRobot = 3;
    [SerializeField] private int _resourcesForBase = 5;
    [SerializeField] private int _maxRobotCount = 3;

    public int ResourcesForRobot => _resourcesForRobot;
    public int ResourcesForBase => _resourcesForBase;

    private ResourceStorage _resourceStorage;
    private RobotStorage _robotStorage;

    private int _minRobotsForBase = 1;

    public void Initialize(ResourceStorage resourceStorage, RobotStorage robotStorage)
    {
        _resourceStorage = resourceStorage;
        _robotStorage = robotStorage;
    }

    public bool IsAbleToCreateBase() =>
       _robotStorage.RobotsCount > _minRobotsForBase;

    public bool IsAbleToCreateRobot() =>
        IsEnough(_resourcesForRobot) && _robotStorage.RobotsCount < _maxRobotCount;

    public bool IsEnoughForBase() =>
        IsEnough(_resourcesForBase);

    private bool IsEnough(int count) =>
         _resourceStorage.Resources.All(parameter => parameter.CurrentCount >= count);
}