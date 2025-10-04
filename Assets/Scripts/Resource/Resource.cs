using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private int _minCount;
    [SerializeField] private int _maxCount;

    private int _count;

    private void Awake()
    {
        _count = Random.Range(_minCount, _maxCount);
    }
}
