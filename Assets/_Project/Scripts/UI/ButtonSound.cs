// Assets/_Project/Scripts/UI/ButtonSound.cs
// Sonido de clic para cualquier botón (UIKit.Button lo agrega solo).
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    public Sfx sound = Sfx.Click;
    void Awake() => GetComponent<Button>().onClick.AddListener(() => AudioManager.Play(sound));
}
