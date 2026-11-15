// Assets/_Project/Scripts/Gameplay/Bobbing.cs
// Movimiento suave de arriba a abajo (BIMO flotando, indicadores "E").
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    [SerializeField] float amplitude = 0.08f;
    [SerializeField] float speed = 3f;
    Vector3 start;

    void Awake() => start = transform.localPosition;

    void Update()
    {
        transform.localPosition = start + Vector3.up * Mathf.Sin(Time.unscaledTime * speed) * amplitude;
    }
}
