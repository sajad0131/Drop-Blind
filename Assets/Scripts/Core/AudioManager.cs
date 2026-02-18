using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private GameAudioData audioData;
    [SerializeField] private int sfxPoolSize = 10;

    [Header("Internal")]
    private AudioSource musicSource;
    private List<AudioSource> sfxPool;
    private bool isWebAudioInitialized = false;

    [Header("Audio Control")]
    [SerializeField] private float warningCooldown = 2.0f; // Seconds between warning sounds
    private float lastWarningTime = -10f; // Initialize to allow immediate play

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudio()
    {
        // 1. Setup Music Source
        GameObject musicObj = new GameObject("MusicSource");
        musicObj.transform.SetParent(transform);
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // 2. Setup SFX Pool (Critical for Mobile Performance)
        sfxPool = new List<AudioSource>();
        GameObject poolParent = new GameObject("SFX_Pool");
        poolParent.transform.SetParent(transform);

        for (int i = 0; i < sfxPoolSize; i++)
        {
            CreatePooledSource(poolParent.transform);
        }

        // 3. Generate a fallback ping if missing (Procedural Audio)
        if (audioData != null && audioData.sonarPing == null)
        {
            Debug.LogWarning("DropBlind :: No Sonar Ping clip assigned. Generating procedural beep.");
            audioData.sonarPing = GenerateProceduralPing();
        }
    }

    private AudioSource CreatePooledSource(Transform parent)
    {
        GameObject obj = new GameObject("SFX_Source");
        obj.transform.SetParent(parent);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        sfxPool.Add(source);
        return source;
    }

    private void Start()
    {
        if (audioData != null && audioData.backgroundAmbience != null)
        {
            PlayMusic(audioData.backgroundAmbience, audioData.musicVolume);
        }
    }

    // --- Public API ---

    public void PlaySonarPing()
    {
        if (audioData == null) return;

        // Randomize pitch slightly for organic feel
        float pitch = 1.0f + Random.Range(-audioData.pingPitchVariance, audioData.pingPitchVariance);
        PlaySFX(audioData.sonarPing, 1.0f, pitch);
    }

    public void PlaySFX(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null) return;

        // WebGL Resume Context Hack (First interaction fix)
        if (Application.platform == RuntimePlatform.WebGLPlayer && !isWebAudioInitialized)
        {
            isWebAudioInitialized = true; // Assume interaction happened
        }

        AudioSource source = GetAvailableSource();
        source.pitch = pitch;
        source.volume = volume;
        source.PlayOneShot(clip);
    }
    public void PlayWarning()
    {
        if (audioData == null || audioData.highNoiseWarning == null) return;

        // COOLDOWN CHECK: Prevent spamming the warning sound
        if (Time.time - lastWarningTime < warningCooldown)
        {
            return;
        }

        lastWarningTime = Time.time;

        // Play with slightly higher pitch to induce stress? 
        // Let's keep it normal (1.0f) for now so the sound designer has control.
        PlaySFX(audioData.highNoiseWarning, 1.2f, 1.0f);
    }
    public void PlayMusic(AudioClip clip, float volume = 0.5f)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // --- Helpers ---

    private AudioSource GetAvailableSource()
    {
        // Find free source
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying) return source;
        }

        // If all busy, expand pool (performance hit, but better than missing sound)
        return CreatePooledSource(transform.GetChild(1));
    }

    // Procedural Audio Generation (Fallback)
    private AudioClip GenerateProceduralPing()
    {
        int sampleRate = 44100;
        float frequency = 880f; // High A
        float duration = 0.3f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            // Sine wave with exponential decay (ping sound)
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * Mathf.Exp(-5f * t);
        }

        AudioClip clip = AudioClip.Create("ProceduralPing", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}