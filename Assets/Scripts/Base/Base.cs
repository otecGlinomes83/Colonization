using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pusher))]
public class Base : MonoBehaviour
{
    [SerializeField] private ResourceDetector _resourceDetector;
    [SerializeField] private ResourceFinder _resourceFinder;
    [SerializeField] private RobotStorage _robotStorage;
    [SerializeField] private ResourceStorage _storage;
    [SerializeField] private float _callRate;

    private Pusher _pusher;

    private List<Resource> _expectedResources = new List<Resource>();

    private void Awake()
    {
        _pusher = GetComponent<Pusher>();
    }

    private void OnEnable()
    {
        _resourceDetector.Detected += OnResourceDetected;
    }

    private void OnDisable()
    {
        _resourceDetector.Detected -= OnResourceDetected;
    }

    private void Start()
    {
        StartCoroutine(CooldownResourceTask());
    }

    private IEnumerator CooldownResourceTask()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(_callRate);

        while (enabled)
        {
            yield return cooldown;

            if (_robotStorage.TryGetFreeRobot(out Robot robot))
            {
                if (_storage.IsFull == false)
                {
                    if (TryGetResource(out Resource resource))
                    {
                        robot.SetResource(resource);
                        _resourceDetector.Detected += OnResourceDetected;
                    }
                    else
                    {
                        _robotStorage.AddFreeRobot(robot);
                        Debug.Log("Resource not found or all robots is busy");
                    }
                }
                else
                {
                    _robotStorage.AddFreeRobot(robot);
                    Debug.Log("Storage is Full!");
                }
            }
            else
            {
                Debug.Log("All robots is busy");
            }
        }
    }

    private bool TryGetResource(out Resource resource)
    {
        resource = null;

        if (_storage.TryGetNeededResourceType(out ResourceType type))
        {
            if (_resourceFinder.TryGetNearResourceByType(type, out Resource nearestResource))
            {
                resource = nearestResource;
                _expectedResources.Add(nearestResource);

                return true;
            }
        }

        return false;
    }

    private void OnResourceDetected(Resource resource)
    {
        if (_expectedResources.Contains(resource))
        {
            _expectedResources.Remove(resource);
            _storage.AddResource(resource);
        }
        else
        {
            _pusher.Push(resource.Rigidbody);
        }
    }
}
