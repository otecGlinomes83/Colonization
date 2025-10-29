using System.Collections;
using UnityEngine;

public class RobotBuilder : MonoBehaviour
{
    [SerializeField] private Robot _robotPrefab;

    public Robot CreateRobot(Transform endPosition)
    {
        Robot robot = Instantiate(_robotPrefab);
        robot.Initialize(endPosition);

        return robot;
    }
}