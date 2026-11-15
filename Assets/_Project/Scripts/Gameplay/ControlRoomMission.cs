// Assets/_Project/Scripts/Gameplay/ControlRoomMission.cs
// Misión "Rescate desde el centro de control" (escena 06_Control).
// Inspirada en casos reales de la NASA: Spirit (2004) se recuperó desde la Tierra tras un fallo de memoria,
// y cuando quedó atascado en arena (2009) JPL ensayó maniobras con una réplica en su "Mars Yard".
// Pasos: diagnóstico (telemetría) → probar maniobras en la réplica → enviar por la red DSN en una ventana
// de comunicación → esperar el retraso de la señal → resultado. Estrellas: probar antes, maniobra correcta, sin derrochar ventanas.
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlRoomMission : MonoBehaviour
{
    public static ControlRoomMission Instance { get; private set; }

    enum Step { Briefing, Diagnose, Test, Send, Waiting, Done }

    [Header("Tiempos (segundos de juego)")]
    [SerializeField] float windowOpen = 14f;      // la antena ve Marte durante este tiempo...
    [SerializeField] float windowClosed = 10f;    // ...y luego deja de verlo este otro
    [SerializeField] float signalDelay = 8f;      // retraso de la señal (en la realidad, entre ~3 y ~22 minutos)

    Step step = Step.Briefing;
    int chosen = -1;            // maniobra elegida en la réplica
    bool[] tested = new bool[3];
    int sends;                  // comandos enviados
    bool sentUntested;
    float windowClock;
    TMP_Text objective, windowText;
    GameObject choicePanel, endPanel;

    static readonly string[] Maneuvers = { "Avanzar a toda potencia", "Retroceder despacio girando las ruedas", "Girar en el sitio" };
    static readonly string[] TestResult =
    {
        "En la réplica, las ruedas giran en falso y el rover se hunde MÁS en la arena. ¡Mala idea!",
        "En la réplica, el rover retrocede poco a poco y sale de la arena. ¡Esta maniobra funciona!",
        "En la réplica, al girar el rover cava un hoyo y queda inclinado. Arriesgado."
    };
    const int Correct = 1;

    void Awake() => Instance = this;
    void OnDestroy() { if (Instance == this) Instance = null; }

    void Start()
    {
        BuildHud();
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        yield return null;
        Say(new[] {
            L(Speaker.Vega, Expression.Worried, "¡Comandante! Recibimos una alerta de Marte: nuestro rover entró en modo seguro y dejó de moverse.", null),
            L(Speaker.Vega, Expression.Neutral, "Como en las misiones reales, no podemos ir a arreglarlo: todo se hace desde aquí, desde el centro de control.", null),
            L(Speaker.Bimo, Expression.Neutral, "Primero lo primero: revisa la consola de telemetría, la de los monitores. Ahí sabremos qué le pasa.", null) },
            () => SetStep(Step.Diagnose));
    }

    void Update()
    {
        windowClock += Time.deltaTime;
        if (windowText != null && step >= Step.Diagnose && step < Step.Done)
            windowText.text = WindowOpen
                ? $"<color=#72C88B>ANTENA DSN · MARTE A LA VISTA · {WindowLeft:0} s</color>"
                : $"<color=#FF8A6B>ANTENA DSN · SIN VISIBILIDAD · próxima ventana en {WindowLeft:0} s</color>";
    }

    bool WindowOpen => (windowClock % (windowOpen + windowClosed)) < windowOpen;
    float WindowLeft
    {
        get
        {
            float t = windowClock % (windowOpen + windowClosed);
            return WindowOpen ? windowOpen - t : windowOpen + windowClosed - t;
        }
    }

    // ---------- Consolas ----------
    public string PromptFor(ConsoleStation.Kind k)
    {
        switch (k)
        {
            case ConsoleStation.Kind.Telemetry: return "Consola de telemetría";
            case ConsoleStation.Kind.TestBench: return "Réplica del rover · probar maniobras";
            default: return "Red de Espacio Profundo (DSN) · enviar comando";
        }
    }

    public void Use(ConsoleStation.Kind k)
    {
        if (step == Step.Briefing || step == Step.Waiting || step == Step.Done || choicePanel.activeSelf) return;
        switch (k)
        {
            case ConsoleStation.Kind.Telemetry: Telemetry(); break;
            case ConsoleStation.Kind.TestBench: OpenChoices(); break;
            default: Send(); break;
        }
    }

    void Telemetry()
    {
        AudioManager.Play(Sfx.Click);
        Say(new[] {
            L(Speaker.Bimo, Expression.Worried, "Telemetría recibida: las seis ruedas giran, pero el rover no avanza. Batería al 40 %.", null),
            L(Speaker.Bimo, Expression.Neutral, "Diagnóstico: está ATASCADO EN ARENA BLANDA. Le pasó algo parecido al rover Spirit en 2009.", null),
            L(Speaker.Vega, Expression.Neutral, "No enviemos nada a ciegas. Ve a la réplica del rover, en la mesa de herramientas, y prueba maniobras aquí en la Tierra.", null) },
            () => { if (step < Step.Test) SetStep(Step.Test); });
    }

    void OpenChoices()
    {
        if (step < Step.Test)
        {
            Say(new[] { L(Speaker.Bimo, Expression.Neutral, "Antes de probar nada, necesitamos saber qué le pasa. Revisa la consola de telemetría.", null) }, null);
            return;
        }
        choicePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Choose(int i)
    {
        choicePanel.SetActive(false);
        Time.timeScale = 1f;
        tested[i] = true;
        chosen = i;
        AudioManager.Play(i == Correct ? Sfx.SiteComplete : Sfx.Error);
        Say(new[] {
            L(Speaker.Bimo, i == Correct ? Expression.Happy : Expression.Worried, TestResult[i], null),
            L(Speaker.Bimo, Expression.Neutral, i == Correct
                ? "Maniobra lista. Ahora ve a la consola de la Red de Espacio Profundo (la pantalla grande) y envíala cuando la antena vea Marte."
                : "Prueba otra maniobra, o envíala igualmente desde la pantalla grande... aunque yo no lo haría.", null) },
            () => { if (step < Step.Send) SetStep(Step.Send); });
    }

    void Send()
    {
        if (step < Step.Send || chosen < 0)
        {
            Say(new[] { L(Speaker.Bimo, Expression.Neutral, "Todavía no tenemos ninguna maniobra preparada. Pruébala primero en la réplica del rover.", null) }, null);
            return;
        }
        if (!WindowOpen)
        {
            AudioManager.Play(Sfx.Error);
            Say(new[] { L(Speaker.Bimo, Expression.Worried, $"Ahora mismo ninguna antena de la DSN ve Marte: la Tierra gira y hay que esperar. Próxima ventana en {WindowLeft:0} s.", null) }, null);
            return;
        }
        sends++;
        if (!tested[chosen]) sentUntested = true;
        AudioManager.Play(Sfx.CommsBack);
        SetStep(Step.Waiting);
        StartCoroutine(WaitSignal());
    }

    IEnumerator WaitSignal()
    {
        for (float t = signalDelay; t > 0; t -= Time.deltaTime)
        {
            objective.text = $"Comando en camino a Marte... la respuesta llegará en {t:0} s\n<size=75%>Las señales viajan a la velocidad de la luz, pero Marte está a millones de km: tardan entre 3 y 22 minutos.</size>";
            yield return null;
        }
        if (chosen == Correct)
        {
            AudioManager.Play(Sfx.Success);
            Say(new[] {
                L(Speaker.Bimo, Expression.Happy, "¡Llegó la respuesta! El rover retrocedió y salió de la arena. ¡Está libre!", null),
                L(Speaker.Vega, Expression.Happy, "Excelente trabajo, comandante. Así se rescata un rover: diagnosticar, probar en la Tierra y enviar con paciencia.", null) },
                Finish);
        }
        else
        {
            AudioManager.Play(Sfx.SafeMode);
            Say(new[] {
                L(Speaker.Bimo, Expression.Worried, "Respuesta recibida... el rover se hundió más en la arena.", null),
                L(Speaker.Vega, Expression.Worried, "No pasa nada, aún podemos intentarlo. Vuelve a la réplica y busca una maniobra que funcione.", null) },
                () => SetStep(Step.Test));
        }
    }

    void Finish()
    {
        SetStep(Step.Done);
        int stars = 1;                                  // rover rescatado
        if (!sentUntested && sends == 1) stars++;        // probaste antes y acertaste a la primera
        if (tested[Correct] && TestedCount() <= 2) stars++;   // no necesitaste probarlo todo
        ShowEnd(stars);
    }

    int TestedCount() { int n = 0; foreach (var b in tested) if (b) n++; return n; }

    // ---------- Pasos y objetivo ----------
    void SetStep(Step s)
    {
        step = s;
        if (objective == null) return;
        switch (s)
        {
            case Step.Diagnose: objective.text = "Objetivo: revisa la <b>consola de telemetría</b> (monitores, a la derecha)."; break;
            case Step.Test: objective.text = "Objetivo: prueba maniobras en la <b>réplica del rover</b> (mesa de herramientas, a la izquierda)."; break;
            case Step.Send: objective.text = "Objetivo: envía la maniobra desde la <b>pantalla grande (DSN)</b> cuando la antena vea Marte."; break;
            case Step.Done: objective.text = "¡Rover rescatado!"; break;
        }
    }

    // ---------- Interfaz ----------
    void BuildHud()
    {
        var canvas = GameObject.Find("Canvas").transform;
        var top = UIKit.Panel(canvas, "ObjectivePanel", UIElement.PanelDark);
        UIKit.Place(top.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(1180, 150));
        var v = UIKit.VList(top.gameObject, 6, 18, TextAnchor.UpperCenter);
        windowText = UIKit.Text(top.transform, "Window", "", 24, TextAlignmentOptions.Center); UIKit.Size(windowText, -1, 32);
        objective = UIKit.Text(top.transform, "Objective", "Centro de control · alerta de Marte", 26, TextAlignmentOptions.Center); UIKit.Size(objective, -1, 78);

        // Elección de maniobra
        var dim = UIKit.Panel(canvas, "ChoiceDim", UIElement.Overlay); UIKit.Fill(dim.rectTransform);
        choicePanel = dim.gameObject;
        var p = UIKit.Panel(dim.transform, "Choices", UIElement.Panel);
        UIKit.Place(p.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(820, 470));
        UIKit.VList(p.gameObject, 16, 36, TextAnchor.UpperCenter);
        var t = UIKit.Text(p.transform, "Title", "Réplica del rover: ¿qué maniobra pruebas?", 32, TextAlignmentOptions.Center, TextTone.Accent);
        t.GetComponent<ThemedText>().title = true; t.GetComponent<ThemedText>().Apply(); UIKit.Size(t, -1, 50);
        for (int i = 0; i < Maneuvers.Length; i++)
        {
            int k = i;
            UIKit.Size(UIKit.Button(p.transform, "Maneuver" + i, Maneuvers[i], 28, UIElement.Button, () => Choose(k)), -1, 82);
        }
        choicePanel.SetActive(false);

        endPanel = null;
    }

    void ShowEnd(int stars)
    {
        var canvas = GameObject.Find("Canvas").transform;
        var dim = UIKit.Panel(canvas, "EndDim", UIElement.Overlay); UIKit.Fill(dim.rectTransform);
        endPanel = dim.gameObject;
        var p = UIKit.Panel(dim.transform, "End", UIElement.Panel);
        UIKit.Place(p.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1100, 640));
        UIKit.VList(p.gameObject, 14, 40, TextAnchor.UpperCenter);
        var title = UIKit.Text(p.transform, "Title", "RESCATE COMPLETADO", 46, TextAlignmentOptions.Center, TextTone.Accent);
        title.GetComponent<ThemedText>().title = true; title.GetComponent<ThemedText>().Apply(); UIKit.Size(title, -1, 64);

        var row = UIKit.NewRect("Stars", p.transform); UIKit.HList(row.gameObject, 16, 0, TextAnchor.MiddleCenter); UIKit.Size(row, -1, 90);
        var th = UITheme.Current;
        for (int i = 0; i < 3; i++) { var img = UIKit.Icon(row, "Star", i < stars ? th.starFull : th.starEmpty, new Vector2(84, 84)); img.enabled = true; }

        string lines =
            "<color=#72C88B>+</color> Rescataste el rover desde la Tierra.\n" +
            (!sentUntested && sends == 1 ? "<color=#72C88B>+</color> Probaste la maniobra en la réplica antes de enviarla y acertaste a la primera.\n"
                                         : "<color=#FF6B6B>x</color> Enviaste comandos sin probar o fallaste algún envío: en Marte cada error cuesta días.\n") +
            (tested[Correct] && TestedCount() <= 2 ? "<color=#72C88B>+</color> Encontraste la maniobra correcta sin agotar todas las pruebas.\n"
                                                   : "<color=#C8D2E6>-</color> Tuviste que probar todas las maniobras.\n") +
            "\n<size=85%><color=#C8D2E6>Dato NASA: en 2009 el rover Spirit quedó atascado en arena blanda. Los ingenieros de JPL ensayaron maniobras con una réplica en un arenero, pero esta vez el rover no pudo salir y siguió trabajando como estación fija hasta 2010.</color></size>";
        var tx = UIKit.Text(p.transform, "Lines", lines, 25, TextAlignmentOptions.TopLeft); UIKit.Size(tx, -1, 300);

        var b = UIKit.Button(p.transform, "Back", "Volver al título", 28, UIElement.ButtonPrimary, () => { GameSession.Reset(); SceneFlow.Go(SceneFlow.Title); });
        UIKit.Size(b, 360, 70);
    }

    // ---------- Diálogos ----------
    static DialogueLine L(Speaker s, Expression e, string t, string h) => new DialogueLine { speaker = s, expression = e, text = t, highlight = h };

    void Say(DialogueLine[] lines, System.Action done)
    {
        if (DialogueBox.Instance == null) { done?.Invoke(); return; }
        DialogueBox.Instance.Show(lines, done);
    }
}
