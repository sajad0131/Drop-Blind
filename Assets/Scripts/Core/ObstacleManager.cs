using UnityEngine;
using UnityEngine.Pool;

public class ObstacleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Obstacle obstaclePrefab;

    // Found dynamically to stay valid across restart flows.
    private Transform worldContainer;

    [Header("Settings")]
    [SerializeField] private float spawnEdgePadding = 0.5f;
    [SerializeField] private float destructionHeight = 15f;

    private ObjectPool<Obstacle> pool;
    private float spawnTimer;
    private Camera _mainCamera;
    private float _dynamicSpawnRange;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (pool != null) return;

        RefreshSceneReferences();
        spawnTimer = 0f;

        pool = new ObjectPool<Obstacle>(
            createFunc: CreateObstacle,
            actionOnGet: (obj) =>
            {
                obj.PrepareForReuse();
                obj.gameObject.SetActive(true);
            },
            actionOnRelease: (obj) =>
            {
                obj.PrepareForPoolRelease();
                obj.gameObject.SetActive(false);
            },
            actionOnDestroy: (obj) => Destroy(obj.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 30
        );

        RecalculateSpawnBounds();
    }

    private void RefreshSceneReferences()
    {
        _mainCamera = Camera.main;

        var scroller = FindFirstObjectByType<WorldScroller>();
        if (scroller != null)
        {
            worldContainer = scroller.transform;
            if (levelData != null)
            {
                scroller.SetSpeed(levelData.baseGlobalSpeed);
            }
        }
    }

    private void Update()
    {
        if (pool == null)
        {
            EnsureInitialized();
            if (pool == null) return;
        }

        if (levelData == null || obstaclePrefab == null) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= levelData.baseSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
        }
    }

    private void RecalculateSpawnBounds()
    {
        if (_mainCamera == null)
        {
            _dynamicSpawnRange = 4f;
            return;
        }

        float distanceToCamera = Mathf.Abs(_mainCamera.transform.position.z - 0f);
        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, distanceToCamera));
        _dynamicSpawnRange = Mathf.Max(0.5f, rightEdge.x - spawnEdgePadding);
    }

    private Obstacle CreateObstacle()
    {
        return Instantiate(obstaclePrefab, worldContainer);
    }

    private void SpawnObstacle()
    {
        if (worldContainer == null)
        {
            RefreshSceneReferences();
        }

        Obstacle obj = pool.Get();
        if (obj == null) return;

        float randomX = Random.Range(-_dynamicSpawnRange, _dynamicSpawnRange);

        if (worldContainer != null)
        {
            obj.transform.SetParent(worldContainer);
        }

        obj.transform.position = new Vector3(randomX, -levelData.spawnDistanceY, 0f);
        obj.Initialize(ReturnToPool, destructionHeight);
    }

    private void ReturnToPool(Obstacle obj)
    {
        if (pool == null || obj == null) return;
        pool.Release(obj);
    }
}
