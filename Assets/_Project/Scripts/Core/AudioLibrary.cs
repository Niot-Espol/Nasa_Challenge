// Assets/_Project/Scripts/Core/AudioLibrary.cs
// Todos los sonidos del juego en un solo asset: Assets/_Project/Resources/AudioLibrary.asset.
// Cuando lleguen los audios, solo hay que arrastrarlos aquí; el código ya los llama.
using UnityEngine;

// IMPORTANTE: agregar valores nuevos SIEMPRE al final (se guardan como número en el asset).
public enum Sfx
{
    Click, Select, Error, Launch, Scan, ScanFail, SiteComplete,
    SafeMode, DustStorm, CommsLost, CommsBack, Dialogue, DoorOpen, Success,
    Countdown, Footstep, Typing, LowEnergy, StarDing,
}

public enum Music { None, Title, Hub, Workshop, Mars, Report }

// Sonidos continuos (se encienden y apagan; se detienen solos al cambiar de escena).
public enum Loop { None, RoverMotor, Wind, Static, RocketRoar }

[CreateAssetMenu(menuName = "MarsCrafts/Audio Library", fileName = "AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable] public class SfxEntry { public Sfx id; public AudioClip clip; [Range(0, 1)] public float volume = 1f; }
    [System.Serializable] public class MusicEntry { public Music id; public AudioClip clip; [Range(0, 1)] public float volume = 0.5f; }
    [System.Serializable] public class LoopEntry { public Loop id; public AudioClip clip; [Range(0, 1)] public float volume = 0.5f; public bool loop = true; }

    public SfxEntry[] sfx;
    public MusicEntry[] music;
    public LoopEntry[] loops;

    public SfxEntry Get(Sfx id) { if (sfx != null) foreach (var e in sfx) if (e.id == id) return e; return null; }
    public MusicEntry Get(Music id) { if (music != null) foreach (var e in music) if (e.id == id) return e; return null; }
    public LoopEntry Get(Loop id) { if (loops != null) foreach (var e in loops) if (e.id == id) return e; return null; }
}
