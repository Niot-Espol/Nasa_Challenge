// Assets/_Project/Scripts/Gameplay/SpriteFlipbook.cs
// Animación simple por frames para sprites sueltos (indicador "E", efecto de escaneo...).
// Usa tiempo sin escala: sigue animando aunque el juego esté pausado por un diálogo.
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipbook : MonoBehaviour
{
    public Sprite[] frames;
    public float framesPerSecond = 4f;

    SpriteRenderer sr;
    float t;

    void Awake() => sr = GetComponent<SpriteRenderer>();
    void OnEnable() => t = 0f;

    void Update()
    {
        if (frames == null || frames.Length == 0) return;
        t += Time.unscaledDeltaTime * framesPerSecond;
        sr.sprite = frames[(int)t % frames.Length];
    }
}
