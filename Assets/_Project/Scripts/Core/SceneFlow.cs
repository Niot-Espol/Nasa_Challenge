using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlow : MonoBehaviour
{
    // Pasa a la siguiente escena según el orden de Build Settings.
    // En el Reporte (última escena), vuelve al título al terminar.
    public void Next()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next >= SceneManager.sceneCountInBuildSettings) next = 0;
        SceneManager.LoadScene(next);
    }
}
