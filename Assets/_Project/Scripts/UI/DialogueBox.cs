// Assets/_Project/Scripts/UI/DialogueBox.cs
// Caja de diálogo con retrato, nombre y texto que se escribe letra a letra.
// Pausa el juego (timeScale = 0) mientras está abierta. Avanza con E, Espacio, Enter o clic.
// También sirve como popup de "Dato NASA" (ShowFact).
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public static DialogueBox Instance { get; private set; }
    public static bool IsOpen => Instance != null && Instance.open;
    // Frame en que se cerró: los interactuables ignoran la E de ese frame (para no reabrir).
    public static int LastClosedFrame { get; private set; } = -1;

    [SerializeField] GameObject root;            // se activa/desactiva
    [SerializeField] ThemedImage frame;          // DialogueBox o FactPopup según el uso
    [SerializeField] Image portrait;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text bodyText;
    [SerializeField] TMP_Text sourceText;        // solo en datos NASA
    [SerializeField] TMP_Text hintText;          // "E para continuar"
    [SerializeField] float charsPerSecond = 55f;

    DialogueLine[] lines;
    int index;
    bool open, typing;
    int openedFrame;
    Action onDone;
    Coroutine typer;
    float prevTimeScale = 1f;

    void Awake()
    {
        Instance = this;
        if (root != null) root.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this) { Instance = null; if (open) Time.timeScale = prevTimeScale; }
    }

    public void Show(DialogueData data, Action done = null) => Show(data != null ? data.lines : null, done);

    public void Show(DialogueLine[] dialogue, Action done = null)
    {
        if (dialogue == null || dialogue.Length == 0) { done?.Invoke(); return; }
        lines = dialogue; index = 0; onDone = done;
        Open(UIElement.DialogueBox);
        if (sourceText != null) sourceText.gameObject.SetActive(false);
        ShowLine();
    }

    // Popup de dato real con su fuente NASA.
    public void ShowFact(string title, string fact, string source, Action done = null)
    {
        lines = new[] { new DialogueLine { speaker = Speaker.Bimo, text = fact } };
        index = 0; onDone = done;
        Open(UIElement.FactPopup);
        if (nameText != null) nameText.text = title;
        SetPortrait(Speaker.Bimo);
        if (sourceText != null)
        {
            sourceText.gameObject.SetActive(!string.IsNullOrEmpty(source));
            sourceText.text = "Fuente: " + source;
        }
        Type(fact);
    }

    void Open(UIElement style)
    {
        if (!open) { prevTimeScale = Time.timeScale; Time.timeScale = 0f; }
        open = true;
        openedFrame = Time.frameCount;
        if (frame != null) frame.SetElement(style);
        if (root != null) root.SetActive(true);
    }

    void ShowLine()
    {
        AudioManager.Play(Sfx.Dialogue);
        var l = lines[index];
        if (nameText != null) nameText.text = DialogueData.SpeakerName(l.speaker);
        SetPortrait(l.speaker);
        Type(l.text);
    }

    void SetPortrait(Speaker s)
    {
        if (portrait == null) return;
        var sp = DialogueData.Portrait(s);
        portrait.sprite = sp;
        portrait.gameObject.SetActive(sp != null);
    }

    void Type(string text)
    {
        if (typer != null) StopCoroutine(typer);
        typer = StartCoroutine(TypeRoutine(text));
    }

    IEnumerator TypeRoutine(string text)
    {
        typing = true;
        bodyText.text = text;
        bodyText.maxVisibleCharacters = 0;
        bodyText.ForceMeshUpdate();
        int total = bodyText.textInfo.characterCount;
        float shown = 0;
        while (shown < total)
        {
            shown += charsPerSecond * Time.unscaledDeltaTime;
            bodyText.maxVisibleCharacters = Mathf.Min(total, (int)shown);
            yield return null;
        }
        bodyText.maxVisibleCharacters = int.MaxValue;
        typing = false;
        if (hintText != null) hintText.text = index < lines.Length - 1 ? "E / Espacio: continuar" : "E / Espacio: cerrar";
    }

    void Update()
    {
        if (!open || Time.frameCount == openedFrame) return;
        if (!AdvancePressed()) return;

        if (typing)
        {
            // Primer toque: mostrar todo el texto
            if (typer != null) StopCoroutine(typer);
            bodyText.maxVisibleCharacters = int.MaxValue;
            typing = false;
            return;
        }
        index++;
        if (index < lines.Length) ShowLine();
        else Close();
    }

    static bool AdvancePressed()
    {
        var k = Keyboard.current;
        var m = Mouse.current;
        return (k != null && (k.eKey.wasPressedThisFrame || k.spaceKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame))
            || (m != null && m.leftButton.wasPressedThisFrame);
    }

    void Close()
    {
        open = false;
        if (root != null) root.SetActive(false);
        Time.timeScale = prevTimeScale;
        LastClosedFrame = Time.frameCount;
        var cb = onDone; onDone = null;
        cb?.Invoke();
    }

    // Para interactuables: ¿puede el jugador usar E ahora?
    public static bool InputFree => !IsOpen && Time.frameCount != LastClosedFrame;
}
