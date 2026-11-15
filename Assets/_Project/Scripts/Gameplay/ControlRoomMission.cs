// Assets/_Project/Scripts/Gameplay/ControlRoomMission.cs
// Misión "Rescate en el centro de control" (escena 06_Control): tres salas unidas, cámara por pantallas (RoomCamera).
//   · Sala de telemetría (izquierda): la pantalla de la pared da el diagnóstico y QUÉ ANTENA DE LA DSN VE MARTE.
//   · Mars Yard (derecha): réplica del rover en un arenero; pruebas maniobras y ves cómo responde.
//   · Centro de control (centro): consola DSN; eliges la antena correcta y envías la maniobra. Luego, retraso de la señal.
// Inspirada en casos reales: Spirit (2009) atascado en arena, con pruebas de JPL en una réplica en su "Mars Yard",
// y la Red de Espacio Profundo (DSN), con tres complejos (Goldstone, Madrid, Canberra) separados ~120° para que
// siempre alguno vea la nave.
using System.Collections;
using TMPro;
using UnityEngine;

public class ControlRoomMission : MonoBehaviour
{
    public static ControlRoomMission Instance { get; private set; }

    [SerializeField] Transform replicaRover;   // réplica en el arenero (Mars Yard)
    [SerializeField] float signalDelay = 8f;   // retraso de la señal en el juego (en la realidad, ~3 a ~22 minutos)

    enum Step { Briefing, Explore, Waiting, Done }
    Step step = Step.Briefing;

    static readonly string[] Antennas = { "Goldstone (EE. UU.)", "Madrid (España)", "Canberra (Australia)" };
    static readonly string[] Maneuvers = { "Avanzar a toda potencia", "Retroceder despacio girando las ruedas", "Girar en el sitio" };
    const int Correct = 1;

    int visibleAntenna;             // la que ve Marte en esta partida (se lee en la sala de telemetría)
    bool readTelemetry;
    int chosen = -1;                // última maniobra probada
    readonly bool[] tested = new bool[3];
    int wrongAntenna, sends;
    bool sentUntested;

    TMP_Text objective, notes, roomLabel;
    GameObject choicePanel, antennaPanel;
    Vector3 replicaHome;

    void Awake() { Instance = this; RoomCamera.RoomChanged += OnRoom; }
    void OnDestroy() { if (Instance == this) Instance = null; RoomCamera.RoomChanged -= OnRoom; }

    void Start()
    {
        visibleAntenna = Random.Range(0, Antennas.Length);
        if (replicaRover != null) replicaHome = replicaRover.position;
        BuildHud();
        if (pendingRoom != null) OnRoom(pendingRoom);
        StartCoroutine(Intro());
    }

    string pendingRoom;
    void OnRoom(string name) { pendingRoom = name; if (roomLabel != null) roomLabel.text = name; }

    IEnumerator Intro()
    {
        yield return null;
        Say(new[] {
            L(Speaker.Vega, Expression.Worried, "¡Comandante! Alerta de Marte: nuestro rover entró en modo seguro y dejó de moverse.", null),
            L(Speaker.Vega, Expression.Neutral, "No podemos ir a arreglarlo: todo se hace desde la Tierra. Tenemos tres salas para ello.", null),
            L(Speaker.Bimo, Expression.Neutral, "A la izquierda, la sala de telemetría: su pantalla dice qué le pasa al rover. A la derecha, el Mars Yard, con una réplica para probar maniobras.", null),
            L(Speaker.Vega, Expression.Neutral, "Y aquí, la consola de la Red de Espacio Profundo para enviar el comando. Ve investigando, comandante.", null) },
            () => { step = Step.Explore; Refresh(); });
    }

    // ---------- Consolas ----------
    public string PromptFor(ConsoleStation.Kind k)
    {
        switch (k)
        {
            case ConsoleStation.Kind.Telemetry: return "Pantalla de telemetría";
            case ConsoleStation.Kind.TestBench: return "Réplica del rover · probar maniobra";
            default: return "Red de Espacio Profundo (DSN) · enviar comando";
        }
    }

    public void Use(ConsoleStation.Kind k)
    {
        if (step != Step.Explore || choicePanel.activeSelf || antennaPanel.activeSelf || busy) return;
        switch (k)
        {
            case ConsoleStation.Kind.Telemetry: Telemetry(); break;
            case ConsoleStation.Kind.TestBench: Open(choicePanel); break;
            default: OpenDsn(); break;
        }
    }

    void Telemetry()
    {
        AudioManager.Play(Sfx.Click);
        bool first = !readTelemetry;
        readTelemetry = true;
        Say(new[] {
            L(Speaker.Bimo, Expression.Worried, "Telemetría: las seis ruedas giran, pero el rover no avanza. Batería al 40 %. Está ATASCADO EN ARENA BLANDA, como le pasó al rover Spirit en 2009.", null),
            L(Speaker.Bimo, Expression.Neutral, $"Y mira la esquina de la pantalla: ahora mismo la antena que ve Marte es la de <b>{Antennas[visibleAntenna]}</b>. ¡Lo anoto!", null),
            L(Speaker.Bimo, Expression.Neutral, first ? "La red DSN tiene tres complejos repartidos por el planeta, separados unos 120°, para que siempre alguno vea la nave mientras la Tierra gira." : "Recuerda: prueba la maniobra en el Mars Yard antes de enviarla.", null) },
            Refresh);
    }

    void Open(GameObject panel) { panel.SetActive(true); Time.timeScale = 0f; }
    void ClosePanels() { choicePanel.SetActive(false); antennaPanel.SetActive(false); Time.timeScale = 1f; }

    bool busy;
    void Choose(int i)
    {
        ClosePanels();
        tested[i] = true; chosen = i;
        StartCoroutine(TestOnReplica(i));
    }

    // La réplica responde en el arenero: retrocede (correcto) o se hunde/inclina (mal)
    IEnumerator TestOnReplica(int i)
    {
        busy = true;
        if (replicaRover != null)
        {
            var sr = replicaRover.GetComponentInChildren<SpriteRenderer>();
            Vector3 home = replicaHome; float t = 0f;
            AudioManager.SetLoop(Loop.RoverMotor, true);
            while (t < 1.6f)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / 1.6f);
                if (i == Correct) replicaRover.position = home + new Vector3(-1.8f * Mathf.SmoothStep(0, 1, k), 0f, 0f);
                else
                {
                    replicaRover.position = home + new Vector3(Random.Range(-0.05f, 0.05f), -0.35f * k, 0f);
                    if (sr != null) sr.color = Color.Lerp(Color.white, new Color(0.65f, 0.55f, 0.45f), k);
                    replicaRover.localRotation = Quaternion.Euler(0, 0, i == 2 ? 10f * k : 0f);
                }
                yield return null;
            }
            AudioManager.SetLoop(Loop.RoverMotor, false);
            AudioManager.Play(i == Correct ? Sfx.SiteComplete : Sfx.Error);
            yield return new WaitForSeconds(0.4f);
        }
        busy = false;
        Say(new[] {
            L(Speaker.Bimo, i == Correct ? Expression.Happy : Expression.Worried,
                i == Correct ? "¡La réplica salió de la arena retrocediendo despacio! Esta maniobra funciona."
              : i == 0 ? "Las ruedas giraron en falso y la réplica se hundió MÁS. Mala idea."
              : "Al girar, la réplica cavó un hoyo y quedó inclinada. Demasiado arriesgado.", null),
            L(Speaker.Bimo, Expression.Neutral, i == Correct
                ? "Vuelve al centro de control y envíala por la red DSN. ¡Elige bien la antena!"
                : "Prueba otra maniobra antes de enviar nada a Marte.", null) },
            () => { ResetReplica(); Refresh(); });
    }

    void ResetReplica()
    {
        if (replicaRover == null) return;
        replicaRover.position = replicaHome; replicaRover.localRotation = Quaternion.identity;
        var sr = replicaRover.GetComponentInChildren<SpriteRenderer>(); if (sr != null) sr.color = Color.white;
    }

    void OpenDsn()
    {
        if (chosen < 0)
        {
            Say(new[] { L(Speaker.Bimo, Expression.Neutral, "Aún no tenemos maniobra. Primero averigua qué le pasa (telemetría, a la izquierda) y pruébala en el Mars Yard (a la derecha).", null) }, null);
            return;
        }
        Open(antennaPanel);
    }

    void PickAntenna(int a)
    {
        ClosePanels();
        if (a != visibleAntenna)
        {
            wrongAntenna++;
            AudioManager.Play(Sfx.Error);
            Say(new[] { L(Speaker.Bimo, Expression.Worried, readTelemetry
                ? $"{Antennas[a]} no ve Marte ahora mismo: la Tierra la tapa. ¿No era otra la que anotamos?"
                : $"{Antennas[a]} no ve Marte ahora mismo. La pantalla de telemetría dice cuál sí lo ve.", null) }, Refresh);
            return;
        }
        sends++;
        if (!tested[chosen]) sentUntested = true;
        AudioManager.Play(Sfx.CommsBack);
        step = Step.Waiting;
        StartCoroutine(WaitSignal());
    }

    IEnumerator WaitSignal()
    {
        for (float t = signalDelay; t > 0; t -= Time.deltaTime)
        {
            objective.text = $"Comando en camino a Marte desde {Antennas[visibleAntenna]}... respuesta en {t:0} s\n<size=78%>La señal viaja a la velocidad de la luz, pero a Marte tarda entre 3 y 22 minutos.</size>";
            yield return null;
        }
        if (chosen == Correct)
        {
            AudioManager.Play(Sfx.Success);
            Say(new[] {
                L(Speaker.Bimo, Expression.Happy, "¡Llegó la respuesta! El rover retrocedió y salió de la arena. ¡Está libre!", null),
                L(Speaker.Vega, Expression.Happy, "Excelente trabajo, comandante: diagnosticar, probar en la Tierra y enviar por la antena correcta. Así se rescata un rover.", null) },
                Finish);
        }
        else
        {
            AudioManager.Play(Sfx.SafeMode);
            Say(new[] {
                L(Speaker.Bimo, Expression.Worried, "Respuesta recibida... el rover se hundió más en la arena.", null),
                L(Speaker.Vega, Expression.Worried, "Aún podemos intentarlo. Vuelve al Mars Yard y busca una maniobra que funcione.", null) },
                () => { step = Step.Explore; Refresh(); });
        }
    }

    void Finish()
    {
        step = Step.Done;
        int stars = 1;
        bool firstTry = !sentUntested && sends == 1;
        bool rightAntenna = wrongAntenna == 0;
        if (firstTry) stars++;
        if (rightAntenna) stars++;
        objective.text = "¡Rover rescatado!";
        ShowEnd(stars, firstTry, rightAntenna);
    }

    // ---------- Objetivo y notas ----------
    void Refresh()
    {
        if (objective == null || step == Step.Waiting || step == Step.Done) return;
        if (!readTelemetry) objective.text = "Objetivo: averigua qué le pasa al rover en la <b>pantalla de la sala de telemetría</b> (a la izquierda).";
        else if (!tested[Correct]) objective.text = "Objetivo: prueba maniobras con la <b>réplica del Mars Yard</b> (a la derecha).";
        else objective.text = "Objetivo: envía la maniobra desde la <b>consola DSN</b> (centro) por la antena que ve Marte.";

        string n = "<b>Notas</b>\n";
        n += readTelemetry ? $"· Diagnóstico: atascado en arena\n· Antena con Marte a la vista: <color=#72C88B>{Antennas[visibleAntenna]}</color>\n" : "· Diagnóstico: ?\n· Antena: ?\n";
        n += chosen >= 0 ? $"· Última prueba: {Maneuvers[chosen]} {(chosen == Correct ? "<color=#72C88B>(funciona)</color>" : "<color=#FF6B6B>(falla)</color>")}" : "· Maniobra: sin probar";
        notes.text = n;
    }

    // ---------- Interfaz ----------
    void BuildHud()
    {
        var canvas = GameObject.Find("Canvas").transform;
        var top = UIKit.Panel(canvas, "ObjectivePanel", UIElement.PanelDark);
        UIKit.Place(top.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -16), new Vector2(1100, 120));
        UIKit.VList(top.gameObject, 4, 16, TextAnchor.UpperCenter);
        roomLabel = UIKit.Text(top.transform, "Room", "", 24, TextAlignmentOptions.Center, TextTone.Accent);
        roomLabel.GetComponent<ThemedText>().title = true; roomLabel.GetComponent<ThemedText>().Apply(); UIKit.Size(roomLabel, -1, 30);
        objective = UIKit.Text(top.transform, "Objective", "Centro de control · alerta de Marte", 24, TextAlignmentOptions.Center); UIKit.Size(objective, -1, 60);

        var np = UIKit.Panel(canvas, "NotesPanel", UIElement.PanelDark);
        UIKit.Place(np.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-16, -150), new Vector2(430, 170));
        UIKit.VList(np.gameObject, 0, 16, TextAnchor.UpperLeft);
        notes = UIKit.Text(np.transform, "Notes", "", 20, TextAlignmentOptions.TopLeft); UIKit.Size(notes, -1, 140);

        choicePanel = MakeChoicePanel(canvas, "ManeuverPanel", "Réplica del rover: ¿qué maniobra pruebas?", Maneuvers, i => Choose(i));
        antennaPanel = MakeChoicePanel(canvas, "AntennaPanel", "Red de Espacio Profundo: ¿por qué antena envías?", Antennas, i => PickAntenna(i));
        Refresh();
    }

    GameObject MakeChoicePanel(Transform canvas, string name, string title, string[] options, System.Action<int> onPick)
    {
        var dim = UIKit.Panel(canvas, name, UIElement.Overlay); UIKit.Fill(dim.rectTransform);
        var p = UIKit.Panel(dim.transform, "Panel", UIElement.Panel);
        UIKit.Place(p.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(860, 150 + options.Length * 98 + 70));
        UIKit.VList(p.gameObject, 16, 36, TextAnchor.UpperCenter);
        var t = UIKit.Text(p.transform, "Title", title, 30, TextAlignmentOptions.Center, TextTone.Accent);
        t.GetComponent<ThemedText>().title = true; t.GetComponent<ThemedText>().Apply(); UIKit.Size(t, -1, 48);
        for (int i = 0; i < options.Length; i++)
        {
            int k = i;
            UIKit.Size(UIKit.Button(p.transform, "Option" + i, options[i], 28, UIElement.Button, () => onPick(k)), -1, 82);
        }
        UIKit.Size(UIKit.Button(p.transform, "Cancel", "Cancelar", 24, UIElement.Button, ClosePanels), 260, 56);
        dim.gameObject.SetActive(false);
        return dim.gameObject;
    }

    void ShowEnd(int stars, bool firstTry, bool rightAntenna)
    {
        var canvas = GameObject.Find("Canvas").transform;
        var dim = UIKit.Panel(canvas, "EndDim", UIElement.Overlay); UIKit.Fill(dim.rectTransform);
        var p = UIKit.Panel(dim.transform, "End", UIElement.Panel);
        UIKit.Place(p.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1120, 660));
        UIKit.VList(p.gameObject, 14, 40, TextAnchor.UpperCenter);
        var title = UIKit.Text(p.transform, "Title", "RESCATE COMPLETADO", 46, TextAlignmentOptions.Center, TextTone.Accent);
        title.GetComponent<ThemedText>().title = true; title.GetComponent<ThemedText>().Apply(); UIKit.Size(title, -1, 64);
        var row = UIKit.NewRect("Stars", p.transform); UIKit.HList(row.gameObject, 16, 0, TextAnchor.MiddleCenter); UIKit.Size(row, -1, 90);
        var th = UITheme.Current;
        for (int i = 0; i < 3; i++) { var img = UIKit.Icon(row, "Star", i < stars ? th.starFull : th.starEmpty, new Vector2(84, 84)); img.enabled = true; }
        string lines =
            "<color=#72C88B>+</color> Rescataste el rover desde la Tierra.\n" +
            (firstTry ? "<color=#72C88B>+</color> Probaste la maniobra en la réplica y acertaste en el primer envío.\n"
                      : "<color=#FF6B6B>x</color> Enviaste algo sin probar o fallaste un envío: en Marte cada error cuesta días.\n") +
            (rightAntenna ? "<color=#72C88B>+</color> Usaste la antena que veía Marte gracias a la telemetría.\n"
                          : "<color=#FF6B6B>x</color> Intentaste enviar por una antena que no veía Marte.\n") +
            "\n<size=85%><color=#C8D2E6>Dato NASA: la Red de Espacio Profundo tiene complejos en Goldstone, Madrid y Canberra para hablar con las naves a toda hora. En 2009 el rover Spirit quedó atascado en arena blanda; JPL ensayó maniobras con una réplica, pero Spirit no salió y siguió trabajando como estación fija hasta 2010.</color></size>";
        var tx = UIKit.Text(p.transform, "Lines", lines, 24, TextAlignmentOptions.TopLeft); UIKit.Size(tx, -1, 320);
        var b = UIKit.Button(p.transform, "Back", "Volver al título", 28, UIElement.ButtonPrimary, () => { GameSession.Reset(); SceneFlow.Go(SceneFlow.Title); });
        UIKit.Size(b, 360, 70);
    }

    static DialogueLine L(Speaker s, Expression e, string t, string h) => new DialogueLine { speaker = s, expression = e, text = t, highlight = h };

    void Say(DialogueLine[] lines, System.Action done)
    {
        if (DialogueBox.Instance == null) { done?.Invoke(); return; }
        DialogueBox.Instance.Show(lines, done);
    }
}
