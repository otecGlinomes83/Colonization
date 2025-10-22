using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotStorage : MonoBehaviour
{
    [SerializeField] private List<Robot> _robots = new List<Robot>();

    private List<Robot> _freeRobots = new List<Robot>();

    public int FreeAmount => _freeRobots.Count;

    private void Awake()
    {
        _freeRobots = _robots.ToList();
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
}