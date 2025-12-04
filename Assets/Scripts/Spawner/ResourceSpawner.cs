using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(BoxCollider))]
public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField] private float _spawnRate;
    [SerializeField] private int _maxSpawns;
    [SerializeField] private int _baseDetectingArea = 5;

    private BoxCollider _spawnZone;

    private ObjectPool<Resource> _resourcePool;

    private void Awake()
    {
        _spawnZone = GetComponent<BoxCollider>();
        _spawnZone.isTrigger = true;

        _resourcePool = new ObjectPool<Resource>
            (
            createFunc: () => Instantiate(_resourcePrefab),
            actionOnGet: (resource) => OnGet(resource),
            actionOnRelease: (resource) => OnRelease(resource)
            );
    }

    private void Start()
    {
        StartCoroutine(CooldownSpawn());
    }

    private IEnumerator CooldownSpawn()
    {
        WaitForSecondsRealtime cooldown = new WaitForSecondsRealtime(_spawnRate);

        while (enabled)
        {
            yield return cooldown;
            TrySpawnResource();
        }
    }

    private void TrySpawnResource()
    {
        if (_resourcePool.CountActive < _maxSpawns)
            _resourcePool.Get();
    }

    private void OnGet(Resource resource)
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();

        bool isBaseHere = IsBaseHere(spawnPosition);

        while (isBaseHere)
        {
            spawnPosition = GetRandomSpawnPosition();
            isBaseHere = IsBaseHere(spawnPosition);
        }

        resource.gameObject.SetActive(true);

        resource.Rigidbody.linearVelocity = Vector3.zero;
        resource.ReadyForRelease += _resourcePool.Release;
        resource.transform.position = spawnPosition;
    }

    private void OnRelease(Resource resource)
    {
        resource.ReadyForRelease -= _resourcePool.Release;
        resource.gameObject.SetActive(false);
    }

    private bool IsBaseHere(Vector3 spawnPosition)
    {
        Collider[] hits = Physics.OverlapSphere(spawnPosition, _baseDetectingArea);

        return hits.Any(hit => hit.gameObject.GetComponent<Base>() != null);
    }

    private Vector3 GetRandomSpawnPosition() =>
         new Vector3(Random.Range(_spawnZone.bounds.min.x, _spawnZone.bounds.max.x), 1f,
             Random.Range(_spawnZone.bounds.min.z, _spawnZone.bounds.max.z));
}