using UnityEngine;
using UnityEngine.SceneManagement;

// Navegación entre escenas. Los métodos de instancia sirven para los OnClick de los botones.
public class SceneFlow : MonoBehaviour
{
    // Nombres de las escenas (deben coincidir con Build Settings).
    public const string Title = "00_Title";
    public const string Hub = "01_Hub";
    public const string Workshop = "02_Workshop";
    public const string Launch = "03_Launch";
    public const string Mars = "04_Mars";
    public const string Report = "05_Report";

    // Pasa a la siguiente escena según el orden de Build Settings.
    // En el Reporte, vuelve al título al terminar.
    public void Next()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next >= SceneManager.sceneCountInBuildSettings) next = 0;
        Go(next);
    }

    public void LoadScene(string sceneName) => Go(sceneName);

    // Nueva partida de Marte: limpia la sesión y va al centro espacial.
    public void NewGame() => StartMission("mars");

    // Nueva partida de la misión indicada (id de MissionCatalog: "mars", "moon"...).
    public void StartMission(string missionId)
    {
        GameSession.StartMission(MissionCatalog.Get(missionId));
        Go(Hub);
    }

    // Escena de exploración de la misión activa.
    public static string MissionScene => GameSession.EnsureMission().SceneName;

    // Rediseñar: vuelve al taller conservando GameSession.Design.
    public void Redesign()
    {
        GameSession.Result = null;
        Go(Workshop);
    }

    // Terminar: vuelve al título y limpia la sesión.
    public void BackToTitle()
    {
        GameSession.Reset();
        Go(Title);
    }

    public static void Go(string sceneName)
    {
        if (SceneFader.Instance != null) SceneFader.Instance.FadeTo(sceneName);
        else SceneManager.LoadScene(sceneName);
    }

    public static void Go(int buildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        Go(System.IO.Path.GetFileNameWithoutExtension(path));
    }
}
