// Assets/_Project/Scripts/Editor/MissionRegression.cs
// Prueba de regresión del modelo de misión: simula varios diseños con un guion fijo
// (moverse, tormenta, apagón, escaneos) y vuelca TODO el resultado (incidentes, reporte, textos) a texto.
// Sirve para comprobar que un cambio de código (p. ej. la abstracción de misiones) no altera Marte:
// se guarda una salida antes del cambio y se compara con la de después (deben ser idénticas).
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class MissionRegression
{
    [MenuItem("Mars Crafts/Pruebas/Regresión de misión (Marte)")]
    static void RunMenu()
    {
        string path = "Logs/mission_regression.txt";
        System.IO.File.WriteAllText(path, Run());
        Debug.Log("[Regresión] Escrito en " + path);
    }

    static readonly string[][] Designs =
    {
        new[] { "launcher_light", "power_solar", "battery_small", "instrument_camera", "instrument_spectrometer", "instrument_drill", "comms_direct", "wheels_standard" },
        new[] { "launcher_medium", "power_rtg", "battery_small", "instrument_camera", "instrument_spectrometer", "comms_direct", "wheels_standard" },
        new[] { "launcher_medium", "power_solar", "battery_large", "instrument_camera", "instrument_spectrometer", "comms_relay", "wheels_allterrain" },
        new[] { "launcher_heavy", "power_rtg", "battery_large", "instrument_camera", "instrument_spectrometer", "instrument_drill", "comms_relay", "wheels_allterrain" },
        new[] { "launcher_light", "power_solar", "battery_small", "instrument_camera", "comms_relay", "wheels_standard" },
        new[] { "launcher_light", "power_rtg", "battery_small", "comms_direct", "wheels_standard" },
    };

    public static string Run()
    {
        var all = new Dictionary<string, ComponentData>();
        foreach (var g in AssetDatabase.FindAssets("t:ComponentData"))
        {
            var c = AssetDatabase.LoadAssetAtPath<ComponentData>(AssetDatabase.GUIDToAssetPath(g));
            all[c.id] = c;
        }

        var sb = new StringBuilder();
        // Textos estáticos de Marte
        foreach (ScienceSite s in new[] { ScienceSite.Delta, ScienceSite.MineralOutcrop, ScienceSite.SpecialRock })
            sb.AppendLine($"SITE {s}: {ReportBuilder.SiteName(s)} / {MissionState.RequiredInstrument(s)} / {MissionState.InstrumentName(MissionState.RequiredInstrument(s))}");

        for (int i = 0; i < Designs.Length; i++)
        {
            var d = RoverDesignBuilder.Build(Designs[i].Select(id => all[id]));
            sb.AppendLine($"\n=== Diseño {i}: {string.Join(", ", Designs[i])}");
            Simulate(d, sb);
        }
        return sb.ToString();
    }

    // Guion fijo, idéntico al comportamiento de MissionController en 04_Mars
    static void Simulate(RoverDesign d, StringBuilder sb)
    {
        var st = new MissionState(d, 150f, 3f);
        float dt = 0.1f, stormAt = 30f, stormDur = 25f, blackoutAt = 75f, minGap = 25f, latest = 25f;
        float bStart = -1f, lastWait = 0f;
        // Orden de visita y segundo en que el rover "llega" a cada punto
        var plan = new List<KeyValuePair<float, ScienceSite>> {
            new KeyValuePair<float, ScienceSite>(20f, ScienceSite.Delta),
            new KeyValuePair<float, ScienceSite>(62f, ScienceSite.MineralOutcrop),
            new KeyValuePair<float, ScienceSite>(118f, ScienceSite.SpecialRock) };
        int next = 0;
        if (!d.AllTerrainWheels) st.RecordIncident(IncidentType.RockyZoneBlocked);

        var messages = new List<string>();
        for (int step = 0; step < 2000 && !st.IsOver; step++)
        {
            float t = st.Time;
            st.SetDustStorm(t >= stormAt && t < stormAt + stormDur);
            if (st.InSafeMode || (st.DustStormActive && d.UsesSolar)) lastWait = t;
            if (bStart < 0f && t >= blackoutAt && t - lastWait >= minGap && t <= 150f - latest) bStart = t;
            st.SetCommsDown(bStart >= 0f && t < bStart + d.BlackoutSeconds);

            if (next < plan.Count && t >= plan[next].Key)
            {
                var site = plan[next].Value;
                var res = st.TryScan(site);
                if (messages.Count == 0 || messages[messages.Count - 1] != res + "|" + st.ScanMessage(res, site))
                    messages.Add(res + "|" + st.ScanMessage(res, site));
                if (res == ScanStatus.Ok || res == ScanStatus.MissingInstrument || res == ScanStatus.AlreadyDone) next++;
            }
            bool moving = next < plan.Count && !st.InSafeMode;
            st.Tick(dt, moving);
        }
        if (!st.IsOver) st.End(false);

        var r = st.Result;
        sb.AppendLine($"estrellas={r.Stars} tiempo={r.TimeUsedSeconds:0.0} energía={r.EnergyConsumed:0.0} datos={r.DataTransmitted}");
        foreach (var inc in r.Incidents) sb.AppendLine($"  inc {inc.Type} t={inc.Time:0.0} dur={inc.Duration:0.0} site={inc.Site}");
        foreach (var m in messages) sb.AppendLine("  msg " + m);
        var rep = ReportBuilder.Build(d, r);
        sb.AppendLine($"  grade: {rep.Grade} | badges: {string.Join(",", rep.Badges)}");
        foreach (var l in rep.Lines) sb.AppendLine($"  line {l.Tone}: {l.Text}");
        foreach (var l in rep.Comparison) sb.AppendLine($"  cmp {l.Tone}: {l.Text}");
    }
}
