using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Pool for SFX")]
    [SerializeField] private int poolSize = 10;
    private List<AudioSource> sfxPool = new();
    private int poolIndex = 0;

    [Header("Music Source")]
    [Tooltip("AudioSource encargado de la música de fondo")]
    [SerializeField] private AudioSource musicSource;

    private const string SFX_PARAM = "SFX";
    private const string MUSIC_PARAM = "Music";

    private bool isSFXMuted;
    private bool isMusicMuted;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioMixer == null)
            Debug.LogError("AudioMixer no asignado.");

        // Leer prefs
        isSFXMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;

        SetVolume(SFX_PARAM, isSFXMuted);
        SetVolume(MUSIC_PARAM, isMusicMuted);

        InitSFXPool();

        // Verificar MusicSource
        if (musicSource == null)
            Debug.LogError("MusicSource no asignado en AudioManager.", this);
        else
            musicSource.loop = true;
    }

    #region SFX Pool

    private void InitSFXPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"SFX_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            sfxPool.Add(src);
        }
    }

    public void PlaySFX(AudioClip clip, float vol = 0.5f)
    {
        if (isSFXMuted || clip == null) return;
        var src = sfxPool[poolIndex];
        src.clip = clip;
        src.volume = vol;
        src.Play();
        poolIndex = (poolIndex + 1) % sfxPool.Count;
    }

    public void ToggleSFX()
    {
        isSFXMuted = !isSFXMuted;
        PlayerPrefs.SetInt("SFXMuted", isSFXMuted ? 1 : 0);
        SetVolume(SFX_PARAM, isSFXMuted);
    }

    #endregion

    #region Music Control

    public void PlayMusicFromStart()
    {
        if (musicSource == null || isMusicMuted) return;
        musicSource.Stop();
        musicSource.time = 0f;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource == null || !musicSource.isPlaying) return;
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource == null || isMusicMuted) return;
        musicSource.UnPause();
    }

    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        SetVolume(MUSIC_PARAM, isMusicMuted);

        if (isMusicMuted)
            musicSource.Pause();
        else
            musicSource.UnPause();
    }

    #endregion

    private void SetVolume(string param, bool mute)
    {
        if (audioMixer != null)
            audioMixer.SetFloat(param, mute ? -80f : 0f);
    }
}
