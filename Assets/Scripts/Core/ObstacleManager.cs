using UnityEngine;
using UnityEngine.Pool;

public class ObstacleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Transform worldContainer;
    [SerializeField] private Obstacle obstaclePrefab;

    [Header("Settings")]
    // REPLACED: Fixed xSpawnRange with dynamic calculation
    [Tooltip("Keep obstacles this far from the exact screen edge.")]
    [SerializeField] private float spawnEdgePadding = 0.5f;
    [SerializeField] private float destructionHeight = 15f;

    private ObjectPool<Obstacle> pool;
    private float spawnTimer;
    private Camera _mainCamera;
    private float _dynamicSpawnRange;

    private void Awake()
    {
        _mainCamera = Camera.main;

        // Initialize Pool
        pool = new ObjectPool<Obstacle>(
            createFunc: CreateObstacle,
            actionOnGet: (obj) => obj.gameObject.SetActive(true),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj.gameObject),
            defaultCapacity: 10,
            maxSize: 30
        );

        RecalculateSpawnBounds();
    }

    private void Start()
    {
        if (levelData != null && WorldScroller.Instance != null)
        {
            WorldScroller.Instance.SetSpeed(levelData.baseGlobalSpeed);
        }
    }

    private void Update()
    {
        // Optional: Call RecalculateSpawnBounds() here if you expect runtime resolution changes (WebGL resizing)

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= levelData.baseSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
        }
    }

    private void RecalculateSpawnBounds()
    {
        // Assuming gameplay happens at Z=0 (Standard for 2D/Vertical Runners)
        // If your obstacles are at a different Z depth, change '0f' to that value.
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

        // UPDATED: Use dynamic range
        float randomX = Random.Range(-_dynamicSpawnRange, _dynamicSpawnRange);

        // Note: We use 0f for Z here. If your game uses depth, match the player's Z.
        Vector3 spawnPos = new Vector3(randomX, -levelData.spawnDistanceY, 0f);

        // Set position directly in world space logic (as per your previous file logic)
        obj.transform.position = new Vector3(randomX, -levelData.spawnDistanceY, 0f);

        obj.Initialize(ReturnToPool, destructionHeight);
    }

    private void ReturnToPool(Obstacle obj)
    {
        pool.Release(obj);
    }
}