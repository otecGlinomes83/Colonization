using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Base : MonoBehaviour
{
    private Storage _storage = new Storage();
    private List<Robot> _robots = new List<Robot>();
    private List<Robot> _freeRobots = new List<Robot>();

    private IEnumerator CooldownResourceTask()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(5f);

        while (enabled)
        {
            if (_storage.IsFull == false)
            {
                yield return cooldown;

            }
        }
    }

    private void FindResource()
    {
        ResourceType requiredType = _storage.GetNecessaryResourceType();


    }

    private Robot GetFreeRobot()
    {
        if (_freeRobots.Count > 0)
        {
            return _freeRobots.First();
        }

        return null;
    }
}
