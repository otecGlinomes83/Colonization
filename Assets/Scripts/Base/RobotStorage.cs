using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RobotSpawner))]
public class RobotStorage : MonoBehaviour
{
    [SerializeField] private List<Robot> _robots = new List<Robot>();
    [SerializeField] private int _maxRobotCount = 3;

    private RobotSpawner _builder;

    private List<Robot> _freeRobots = new List<Robot>();

    private int _minRobotCountForBase = 1;

    private void Awake()
    {
        _builder = GetComponent<RobotSpawner>();
        _freeRobots = _robots.ToList();
    }

    public void CreateNewRobot(Transform endPosition)
    {
        if (_robots.Count >= _maxRobotCount)
            return;

        AddRobotToList(_builder.CreateRobot(endPosition));
    }

    public bool TryGetFreeRobot(out Robot robot)
    {
        robot = null;

        if (_freeRobots.Count > 0)
        {
            robot = _freeRobots.First();
            _freeRobots.Remove(robot);

            return true;
        }

        return false;
    }

    public void AddFreeRobot(Robot robot)
    {
        _freeRobots.Add(robot);
    }

    public void TryRemoveRobotFromList(Robot robot)
    {
        if (_robots.Contains(robot) == false)
            return;

        if (_freeRobots.Contains(robot))
            _freeRobots.Remove(robot);

        _robots.Remove(robot);
    }

    public void AddRobotToList(Robot robot)
    {
        if (_robots.Contains(robot))
            return;

        _robots.Add(robot);
        AddFreeRobot(robot);
    }

    public bool IsAbleToCreateBase() =>
        _robots.Count > _minRobotCountForBase;

    public bool IsAbleToCreateRobot() =>
        _robots.Count < _maxRobotCount;
}