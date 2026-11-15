// Assets/_Project/Scripts/Editor/SfxGenerator.cs
// Genera efectos de sonido simples por código (WAV mono 16 bits) para los que no se consiguieron:
// error, alarma de energía baja, modo seguro, viento (loop), estática (loop), señal recuperada.
// Menú: Tools > Mars Crafts > Generar sonidos. Se pueden reemplazar luego por mejores en AudioLibrary.
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SfxGenerator
{
    const int Rate = 44100;
    public const string Folder = "Assets/_Project/Audio/SFX/Generated/";

    [MenuItem("Tools/Mars Crafts/Generar sonidos")]
    public static void GenerateAll()
    {
        Directory.CreateDirectory(Folder);
        Save("error_buzz", ErrorBuzz());
        Save("scan_fail", ScanFail());
        Save("low_energy", LowEnergy());
        Save("safe_mode", SafeMode());
        Save("comms_back", CommsBack());
        Save("wind_loop", WindLoop());
        Save("static_loop", StaticLoop());
        Save("dialogue_blip", DialogueBlip());
        AssetDatabase.Refresh();
    }

    // ---------- Sonidos ----------
    static float[] ErrorBuzz()
    {
        var s = new float[(int)(Rate * 0.32f)];
        for (int i = 0; i < s.Length; i++)
        {
            float t = (float)i / Rate;
            float sq = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * 110f * t)) * 0.5f + Mathf.Sign(Mathf.Sin(2 * Mathf.PI * 116f * t)) * 0.5f;
            s[i] = sq * 0.35f * Env(t, 0.32f, 0.01f, 0.08f);
        }
        return s;
    }

    static float[] ScanFail()
    {
        var s = new float[(int)(Rate * 0.3f)];
        for (int i = 0; i < s.Length; i++)
        {
            float t = (float)i / Rate;
            float f = t < 0.14f ? 420f : 300f;
            float local = t < 0.14f ? t : t - 0.16f;
            float on = (t < 0.14f || t > 0.16f) ? 1 : 0;
            s[i] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * f * t)) * 0.25f * on * Env(local, 0.14f, 0.005f, 0.04f);
        }
        return s;
    }

    static float[] LowEnergy()
    {
        var s = new float[(int)(Rate * 0.36f)];
        for (int i = 0; i < s.Length; i++)
        {
            float t = (float)i / Rate;
            bool first = t < 0.12f, second = t > 0.2f && t < 0.32f;
            float local = first ? t : t - 0.2f;
            s[i] = (first || second) ? Mathf.Sin(2 * Mathf.PI * 880f * t) * 0.3f * Env(local, 0.12f, 0.005f, 0.03f) : 0;
        }
        return s;
    }

    static float[] SafeMode()
    {
        float dur = 1.1f;
        var s = new float[(int)(Rate * dur)];
        float phase = 0;
        for (int i = 0; i < s.Length; i++)
        {
            float t = (float)i / Rate;
            float f = Mathf.Lerp(620f, 60f, Mathf.Pow(t / dur, 0.7f));
            phase += f / Rate;
            float saw = (phase % 1f) * 2f - 1f;
            s[i] = saw * 0.3f * Env(t, dur, 0.01f, 0.35f);
        }
        return s;
    }

    static float[] CommsBack()
    {
        var s = new float[(int)(Rate * 0.34f)];
        for (int i = 0; i < s.Length; i++)
        {
            float t = (float)i / Rate;
            bool a = t < 0.12f, b = t > 0.16f && t < 0.3f;
            float f = a ? 660f : 990f;
            float local = a ? t : t - 0.16f;
            s[i] = (a || b) ? Mathf.Sin(2 * Mathf.PI * f * t) * 0.28f * Env(local, a ? 0.12f : 0.14f, 0.005f, 0.04f) : 0;
        }
        return s;
    }

    // Blip de diálogo: onda cuadrada suavizada (paso bajo) de 45 ms con caída rápida.
    // El tono por personaje lo da AudioManager.PlayTyping (BIMO más agudo, Vega más grave).
    static float[] DialogueBlip()
    {
        float dur = 0.045f;
        var s = new float[(int)(Rate * dur)];
        float lp = 0;
        for (int i = 0; i < s.Length; i++)
        {
            float t = (float)i / Rate;
            float f = 760f - 120f * (t / dur);                  // baja apenas el tono: suena a "bip" natural
            float sq = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * f * t));
            lp += (sq - lp) * 0.25f;                            // quita aspereza
            float env = Mathf.Min(1f, t / 0.003f) * Mathf.Exp(-t * 55f);
            s[i] = lp * 0.4f * env;
        }
        return s;
    }

    // Viento: ruido filtrado con ráfagas lentas. Loop sin corte (cruce de extremos).
    static float[] WindLoop()
    {
        float dur = 4f;
        var s = Noise(dur, 7);
        float lp = 0, lp2 = 0;
        for (int i = 0; i < s.Length; i++)
        {
            float t = (float)i / Rate;
            lp += (s[i] - lp) * 0.02f;          // paso bajo
            lp2 += (lp - lp2) * 0.05f;
            float gust = 0.55f + 0.45f * Mathf.Sin(2 * Mathf.PI * t / dur) * Mathf.Sin(2 * Mathf.PI * t * 2f / dur + 1f);
            s[i] = lp2 * 6f * gust;
        }
        return MakeLoop(Normalize(s, 0.5f), 0.4f);
    }

    // Estática de radio: ruido de banda media con chasquidos.
    static float[] StaticLoop()
    {
        float dur = 2f;
        var s = Noise(dur, 11);
        var rnd = new System.Random(5);
        float lp = 0, hp = 0, prev = 0;
        for (int i = 0; i < s.Length; i++)
        {
            lp += (s[i] - lp) * 0.35f;
            hp = 0.9f * (hp + lp - prev); prev = lp;
            float crackle = rnd.NextDouble() < 0.0008 ? (float)(rnd.NextDouble() * 2 - 1) * 1.5f : 0;
            s[i] = hp * 0.8f + crackle;
        }
        return MakeLoop(Normalize(s, 0.35f), 0.2f);
    }

    // ---------- Utilidades ----------
    static float Env(float t, float dur, float attack, float release)
    {
        if (t < 0 || t > dur) return 0;
        if (t < attack) return t / attack;
        if (t > dur - release) return Mathf.Max(0, (dur - t) / release);
        return 1;
    }

    static float[] Noise(float dur, int seed)
    {
        var rnd = new System.Random(seed);
        var s = new float[(int)(Rate * dur)];
        for (int i = 0; i < s.Length; i++) s[i] = (float)(rnd.NextDouble() * 2 - 1);
        return s;
    }

    static float[] Normalize(float[] s, float peak)
    {
        float max = 0.0001f;
        foreach (var v in s) max = Mathf.Max(max, Mathf.Abs(v));
        for (int i = 0; i < s.Length; i++) s[i] = s[i] / max * peak;
        return s;
    }

    // Mezcla el final con el principio para que el loop no tenga "click".
    static float[] MakeLoop(float[] s, float fadeSeconds)
    {
        int n = (int)(Rate * fadeSeconds);
        var outS = new float[s.Length - n];
        Array.Copy(s, outS, outS.Length);
        for (int i = 0; i < n; i++)
        {
            float a = (float)i / n;
            outS[i] = s[i] * a + s[outS.Length + i] * (1 - a);
        }
        return outS;
    }

    static void Save(string name, float[] samples)
    {
        using (var fs = new FileStream(Folder + name + ".wav", FileMode.Create))
        using (var w = new BinaryWriter(fs))
        {
            int bytes = samples.Length * 2;
            w.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); w.Write(36 + bytes);
            w.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt ")); w.Write(16); w.Write((short)1); w.Write((short)1);
            w.Write(Rate); w.Write(Rate * 2); w.Write((short)2); w.Write((short)16);
            w.Write(System.Text.Encoding.ASCII.GetBytes("data")); w.Write(bytes);
            foreach (var v in samples) w.Write((short)(Mathf.Clamp(v, -1f, 1f) * 32767));
        }
    }
}
