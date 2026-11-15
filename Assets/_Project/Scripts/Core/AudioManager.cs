// Assets/_Project/Scripts/Core/AudioManager.cs
// Reproduce sonidos y música de la AudioLibrary. Se crea solo y sobrevive a los cambios de escena.
// Uso: AudioManager.Play(Sfx.Scan);  AudioManager.PlayMusic(Music.Mars);
// Si un clip no está asignado todavía, no hace nada (no da error).
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    AudioLibrary library;
    AudioSource sfxSource, musicSource;
    Music currentMusic = Music.None;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (instance != null) return;
        var go = new GameObject("AudioManager");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<AudioManager>();
        instance.library = Resources.Load<AudioLibrary>("AudioLibrary");
        instance.sfxSource = go.AddComponent<AudioSource>();
        instance.musicSource = go.AddComponent<AudioSource>();
        instance.musicSource.loop = true;
        SceneManager.sceneLoaded += (s, m) => instance.OnSceneLoaded(s.name);
    }

    public static void Play(Sfx id)
    {
        if (instance == null || instance.library == null) return;
        var e = instance.library.Get(id);
        if (e != null && e.clip != null) instance.sfxSource.PlayOneShot(e.clip, e.volume);
    }

    public static void PlayMusic(Music id)
    {
        if (instance == null || instance.library == null || id == instance.currentMusic) return;
        instance.currentMusic = id;
        var e = instance.library.Get(id);
        if (e == null || e.clip == null) { instance.musicSource.Stop(); return; }
        instance.musicSource.clip = e.clip;
        instance.musicSource.volume = e.volume;
        instance.musicSource.Play();
    }

    // Música por escena (cambiar aquí si se quiere otra asignación).
    void OnSceneLoaded(string scene)
    {
        switch (scene)
        {
            case SceneFlow.Title: PlayMusic(Music.Title); break;
            case SceneFlow.Hub: PlayMusic(Music.Hub); break;
            case SceneFlow.Workshop: PlayMusic(Music.Workshop); break;
            case SceneFlow.Launch: PlayMusic(Music.None); Play(Sfx.Launch); break;
            case SceneFlow.Mars: PlayMusic(Music.Mars); break;
            case SceneFlow.Report: PlayMusic(Music.Report); break;
        }
    }
}
