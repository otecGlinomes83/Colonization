using System.Collections;
using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    [SerializeField] private Robot _robotPrefab;

    public Robot CreateRobot(Transform endPosition)
    {
        Robot robot = Instantiate(_robotPrefab);
        robot.Initialize(endPosition);

        return robot;
    }
}