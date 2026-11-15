// Assets/_Project/Scripts/Gameplay/NpcDialogue.cs
// Personaje con el que se habla con E. Puede tener un diálogo para la primera vez
// y otro para las siguientes (p. ej. la Dra. Vega: briefing y luego un recordatorio).
using UnityEngine;
using UnityEngine.Events;

public class NpcDialogue : Interactable
{
    [SerializeField] DialogueData firstTime;
    [SerializeField] DialogueData repeat;              // opcional
    [SerializeField] UnityEvent onFirstDialogueDone;   // p. ej. desbloquear la puerta del taller

    // Diálogos propios de otras misiones (si la misión activa tiene uno, sustituye a firstTime/repeat)
    [System.Serializable]
    public class MissionDialogue
    {
        public string missionId = "moon";
        public DialogueData firstTime;
        public DialogueData repeat;    // opcional (si falta, se usa el repeat general)
    }
    [SerializeField] MissionDialogue[] perMission;

    bool talked;

    protected override void Interact()
    {
        if (DialogueBox.Instance == null) return;
        var first0 = firstTime; var repeat0 = repeat;
        var mission = GameSession.EnsureMission().Id;
        if (perMission != null)
            foreach (var m in perMission)
                if (m != null && m.missionId == mission)
                {
                    if (m.firstTime != null) first0 = m.firstTime;
                    if (m.repeat != null) repeat0 = m.repeat;
                }
        var data = (!talked || repeat0 == null) ? first0 : repeat0;
        bool first = !talked;
        DialogueBox.Instance.Show(data, () =>
        {
            talked = true;
            if (first) onFirstDialogueDone?.Invoke();
        });
    }
}
