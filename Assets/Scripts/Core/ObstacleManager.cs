using UnityEngine;
using UnityEngine.Pool;

public class ObstacleManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Transform worldContainer; // The WorldScroller object
    [SerializeField] private Obstacle obstaclePrefab; // Your Obstacle Prefab

    [Header("Settings")]
    [SerializeField] private float xSpawnRange = 4.5f; // Should match PlayerController xClamp
    [SerializeField] private float destructionHeight = 15f; // Y pos above player to despawn

    private ObjectPool<Obstacle> pool;
    private float spawnTimer;

    private void Awake()
    {
        // Initialize Pool
        pool = new ObjectPool<Obstacle>(
            createFunc: CreateObstacle,
            actionOnGet: (obj) => obj.gameObject.SetActive(true),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj.gameObject),
            defaultCapacity: 10,
            maxSize: 30
        );
    }

    private void Start()
    {
        // Apply Level Data Speed
        if (levelData != null && WorldScroller.Instance != null)
        {
            WorldScroller.Instance.SetSpeed(levelData.baseGlobalSpeed);
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= levelData.baseSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
        }
    }

    private Obstacle CreateObstacle()
    {
        Obstacle obj = Instantiate(obstaclePrefab, worldContainer);
        return obj;
    }

    private void SpawnObstacle()
    {
        Obstacle obj = pool.Get();

        // 1. Calculate Spawn Position
        // We spawn relative to the World Container so they move WITH the world.
        // However, we want them to appear at a fixed distance below the PLAYER (who is at World 0,0 usually)
        // Since World moves UP, we must spawn at (PlayerY - Distance) in World Space, 
        // then convert to Container Local Space if needed, OR just spawn in World Space and reparent.

        // Simpler approach for "Falling" game:
        // Spawn at a fixed Y relative to the Camera/Player. 
        // If Player is at Y=0, spawn at Y = -20.

        float randomX = Random.Range(-xSpawnRange, xSpawnRange);
        Vector3 spawnPos = new Vector3(randomX, -levelData.spawnDistanceY, 0f);

        // If obstacles are children of WorldContainer (which moves), setting localPosition puts them relative to the moving world.
        // This is exactly what we want.
        obj.transform.localPosition = new Vector3(randomX, -levelData.spawnDistanceY - worldContainer.transform.position.y, 0f);

        // Wait! If we use localPosition relative to a moving parent, the math gets tricky.
        // EASIER WAY: Set World Position, then ensure it's parented.
        obj.transform.position = new Vector3(randomX, -levelData.spawnDistanceY, 0f);

        // Initialize with cleanup logic
        obj.Initialize(ReturnToPool, destructionHeight);
    }

    private void ReturnToPool(Obstacle obj)
    {
        pool.Release(obj);
    }
}