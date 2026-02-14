using UnityEngine;
using UnityEngine.Pool; // Unity 2021+ built-in pooling

public class SonarManager : MonoBehaviour
{
    [Header("Pooling Config")]
    [SerializeField] private SonarPulse sonarPrefab;
    [SerializeField] private int defaultPoolSize = 10;
    [SerializeField] private int maxPoolSize = 20;

    private ObjectPool<SonarPulse> pool;

    private void Awake()
    {
        pool = new ObjectPool<SonarPulse>(
            createFunc: CreatePulse,
            actionOnGet: (p) => p.gameObject.SetActive(true),
            actionOnRelease: (p) => p.gameObject.SetActive(false),
            defaultCapacity: defaultPoolSize
        );
    }

    private SonarPulse CreatePulse()
    {
        SonarPulse p = Instantiate(sonarPrefab, transform);
        p.Initialize(ReturnPulseToPool); // Teach it how to return
        return p;
    }

    private void ReturnPulseToPool(SonarPulse p)
    {
        pool.Release(p);
    }

    public void TriggerSonar(Vector3 position)
    {
        SonarPulse pulse = pool.Get();
        pulse.StartPulse(position);
    }
}