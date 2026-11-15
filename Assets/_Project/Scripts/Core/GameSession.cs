// Assets/_Project/Scripts/Core/GameSession.cs
// Estado compartido entre escenas. Las clases estáticas sobreviven a SceneManager.LoadScene.
// IMPORTANTE: no desactivar el Domain Reload en Enter Play Mode Options (no se reiniciaría).
public static class GameSession
{
    public static RoverDesign Design;    // lo escribe el Taller, lo lee Marte
    public static MissionResult Result;  // lo escribe la exploración, lo lee el Reporte
    public static MissionDefinition Mission;   // misión elegida en el título (Marte, Luna...)

    // Para escenas que leen la sesión: si vienen vacías (Play directo en esa escena),
    // crean datos de prueba en vez de fallar.
    public static RoverDesign EnsureDesign() => Design ??= RoverDesign.CreateDebugDefault();
    public static MissionResult EnsureResult() => Result ??= MissionResult.CreateDebugDefault();
    public static MissionDefinition EnsureMission() => Mission ??= MissionCatalog.Mars;

    // Nueva partida desde el título. "Rediseñar" NO llama a esto: conserva Design.
    // La misión se conserva (la elige el título justo antes de empezar).
    public static void Reset()
    {
        Design = null;
        Result = null;
    }

    // Nueva partida de una misión concreta.
    public static void StartMission(MissionDefinition mission)
    {
        Reset();
        Mission = mission;
    }
}
