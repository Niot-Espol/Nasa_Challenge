// Assets/_Project/Scripts/Core/YSortCamera.cs
// Ordena los sprites por su altura en pantalla: lo que está más abajo (más cerca) se dibuja delante.
// Va en la cámara principal de las escenas con mundo (Hub, Marte). Los sprites usan su pivote
// (en los pies o la base) como punto de comparación: Sprite Sort Point = Pivot.
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class YSortCamera : MonoBehaviour
{
    void OnEnable()
    {
        var cam = GetComponent<Camera>();
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = new Vector3(0f, 1f, 0f);
    }

    void OnDisable()
    {
        var cam = GetComponent<Camera>();
        if (cam != null) cam.transparencySortMode = TransparencySortMode.Default;
    }
}
