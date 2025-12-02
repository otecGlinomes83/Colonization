using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RobotSpawner))]
public class RobotStorage : MonoBehaviour
{
    [SerializeField] private List<Robot> _robots = new List<Robot>();

    private RobotSpawner _robotSpawner;

    private List<Robot> _freeRobots = new List<Robot>();

    public int RobotsCount => _robots.Count;

    private void Awake()
    {
        _robotSpawner = GetComponent<RobotSpawner>();
        _freeRobots = _robots.ToList();
    }

    public void CreateNewRobot(Transform endPosition)
    {
        AddRobotToList(_robotSpawner.CreateRobot(endPosition));
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
}