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

    bool talked;

    protected override void Interact()
    {
        if (DialogueBox.Instance == null) return;
        var data = (!talked || repeat == null) ? firstTime : repeat;
        bool first = !talked;
        DialogueBox.Instance.Show(data, () =>
        {
            talked = true;
            if (first) onFirstDialogueDone?.Invoke();
        });
    }
}
