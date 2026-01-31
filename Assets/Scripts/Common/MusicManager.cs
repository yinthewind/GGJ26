using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Creates the MusicManager singleton if it doesn't exist.
    /// </summary>
    public static MusicManager Create()
    {
        if (Instance != null)
            return Instance;

        GameObject obj = new GameObject("MusicManager");
        return obj.AddComponent<MusicManager>();
    }

    /// <summary>
    /// Loads and plays music from Resources folder.
    /// </summary>
    /// <param name="resourcePath">Path relative to Resources folder (without extension)</param>
    public void PlayMusic(string resourcePath)
    {
        AudioClip clip = Resources.Load<AudioClip>(resourcePath);
        if (clip == null)
        {
            Debug.LogWarning($"MusicManager: Could not load audio clip at Resources/{resourcePath}");
            return;
        }

        PlayMusic(clip);
    }

    /// <summary>
    /// Plays the given audio clip as looping music.
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (_audioSource.clip == clip && _audioSource.isPlaying)
            return;

        _audioSource.clip = clip;
        _audioSource.Play();
    }

    /// <summary>
    /// Stops the current music.
    /// </summary>
    public void StopMusic()
    {
        _audioSource.Stop();
    }

    /// <summary>
    /// Pauses the current music.
    /// </summary>
    public void PauseMusic()
    {
        _audioSource.Pause();
    }

    /// <summary>
    /// Resumes paused music.
    /// </summary>
    public void ResumeMusic()
    {
        _audioSource.UnPause();
    }

    /// <summary>
    /// Sets the music volume (0-1).
    /// </summary>
    public void SetVolume(float volume)
    {
        _audioSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Gets the current music volume.
    /// </summary>
    public float GetVolume()
    {
        return _audioSource.volume;
    }

    /// <summary>
    /// Returns true if music is currently playing.
    /// </summary>
    public bool IsPlaying => _audioSource.isPlaying;
}
