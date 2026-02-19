using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ObstacleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Obstacle obstaclePrefab;

    // We will find this dynamically now to prevent missing references
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeSystem();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeSystem();
    }

    private void InitializeSystem()
    {
        _mainCamera = Camera.main;
        spawnTimer = 0f;

        // Safely find the new WorldScroller to attach obstacles to
        var scroller = FindFirstObjectByType<WorldScroller>();
        if (scroller != null)
        {
            worldContainer = scroller.transform;
            scroller.SetSpeed(levelData.baseGlobalSpeed); // Restart the scrolling speed!
        }

        // Rebuild the Object Pool completely so it doesn't try to use destroyed objects
        if (pool != null) pool.Clear();

        pool = new ObjectPool<Obstacle>(
            createFunc: CreateObstacle,
            actionOnGet: (obj) => obj.gameObject.SetActive(true),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 30
        );

        RecalculateSpawnBounds();
    }

    private void Update()
    {
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
        float distanceToCamera = Mathf.Abs(_mainCamera.transform.position.z - 0f);
        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, distanceToCamera));
        _dynamicSpawnRange = rightEdge.x - spawnEdgePadding;
    }

    private Obstacle CreateObstacle()
    {
        Obstacle obj = Instantiate(obstaclePrefab, worldContainer);
        return obj;
    }

    private void SpawnObstacle()
    {
        Obstacle obj = pool.Get();

        float randomX = Random.Range(-_dynamicSpawnRange, _dynamicSpawnRange);

        // Ensure it spawns as a child of the moving world
        if (worldContainer != null)
        {
            obj.transform.SetParent(worldContainer);
        }

        obj.transform.position = new Vector3(randomX, -levelData.spawnDistanceY, 0f);
        obj.Initialize(ReturnToPool, destructionHeight);
    }

    private void ReturnToPool(Obstacle obj)
    {
        pool.Release(obj);
    }
}