// Assets/_Project/Scripts/Core/AudioManager.cs
// Reproduce sonidos, loops y música de la AudioLibrary. Se crea solo y sobrevive a los cambios de escena.
// Uso: AudioManager.Play(Sfx.Scan);  AudioManager.Play(Sfx.Footstep, pitch: 1.1f);
//      AudioManager.SetLoop(Loop.Wind, true);  AudioManager.PlayMusic(Music.Mars);
// Si un clip no está asignado, no hace nada (no da error).
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    AudioLibrary library;
    AudioSource musicSource, typingSource;
    readonly AudioSource[] pool = new AudioSource[6];   // para sonidos con tono variable
    int next;
    readonly Dictionary<Loop, AudioSource> loops = new Dictionary<Loop, AudioSource>();
    Music currentMusic = Music.None;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (instance != null) return;
        var go = new GameObject("AudioManager");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<AudioManager>();
        instance.library = Resources.Load<AudioLibrary>("AudioLibrary");
        for (int i = 0; i < instance.pool.Length; i++) instance.pool[i] = go.AddComponent<AudioSource>();
        instance.musicSource = go.AddComponent<AudioSource>();
        instance.musicSource.loop = true;
        instance.typingSource = go.AddComponent<AudioSource>();
        SceneManager.sceneLoaded += (s, m) => instance.OnSceneLoaded(s.name);
    }

    public static void Play(Sfx id, float pitch = 1f, float volumeScale = 1f)
    {
        if (instance == null || instance.library == null) return;
        var e = instance.library.Get(id);
        if (e == null || e.clip == null) return;
        var src = instance.pool[instance.next];
        instance.next = (instance.next + 1) % instance.pool.Length;
        src.pitch = pitch;
        src.PlayOneShot(e.clip, e.volume * volumeScale);
    }

    // Tecleo de diálogos: reinicia un solo canal para que no se amontonen.
    public static void PlayTyping(float pitch)
    {
        if (instance == null || instance.library == null) return;
        var e = instance.library.Get(Sfx.Typing);
        if (e == null || e.clip == null) return;
        var src = instance.typingSource;
        src.Stop();
        src.clip = e.clip;
        src.volume = e.volume;
        src.pitch = pitch * Random.Range(0.95f, 1.05f);
        src.Play();
    }

    public static void SetLoop(Loop id, bool on, float volumeScale = 1f)
    {
        if (instance == null || instance.library == null || id == Loop.None) return;
        AudioSource src;
        if (!instance.loops.TryGetValue(id, out src))
        {
            if (!on) return;
            var e = instance.library.Get(id);
            if (e == null || e.clip == null) return;
            src = instance.gameObject.AddComponent<AudioSource>();
            src.clip = e.clip; src.loop = e.loop; src.volume = e.volume * volumeScale;
            instance.loops[id] = src;
        }
        if (on && !src.isPlaying) src.Play();
        else if (!on && src.isPlaying) src.Stop();
    }

    // Mantiene un loop sonando y ajusta su volumen (0..1 sobre el de la librería) y su tono.
    public static void SetLoopLevel(Loop id, float volume01, float pitch)
    {
        if (instance == null || instance.library == null || id == Loop.None) return;
        var e = instance.library.Get(id);
        if (e == null || e.clip == null) return;
        AudioSource src;
        if (!instance.loops.TryGetValue(id, out src))
        {
            src = instance.gameObject.AddComponent<AudioSource>();
            src.clip = e.clip; src.loop = true;
            instance.loops[id] = src;
        }
        src.volume = e.volume * volume01;
        src.pitch = pitch;
        if (!src.isPlaying && volume01 > 0f) src.Play();
        else if (src.isPlaying && volume01 <= 0f) src.Stop();
    }

    public static void StopAllLoops()
    {
        if (instance == null) return;
        foreach (var s in instance.loops.Values) if (s != null) s.Stop();
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

    // Música por escena (cambiar aquí si se quiere otra asignación). Los loops se cortan al cambiar.
    void OnSceneLoaded(string scene)
    {
        StopAllLoops();
        switch (scene)
        {
            case SceneFlow.Title: PlayMusic(Music.Title); break;
            case SceneFlow.Hub: PlayMusic(Music.Hub); break;
            case SceneFlow.Workshop: PlayMusic(Music.Workshop); break;
            case SceneFlow.Launch: PlayMusic(Music.None); break;
            case SceneFlow.Mars: PlayMusic(Music.Mars); break;
            case "04_Moon": PlayMusic(Music.Mars); break;   // de momento, la misma música de exploración
            case "06_Control": PlayMusic(Music.Hub); break;
            case SceneFlow.Report: PlayMusic(Music.Report); break;
        }
    }
}
