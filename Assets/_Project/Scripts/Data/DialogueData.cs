// Assets/_Project/Scripts/Data/DialogueData.cs
// Un diálogo = lista de líneas (quién habla + texto). Se crea como asset
// (Create > MarsCrafts > Dialogue) en Assets/_Project/Data/Dialogues/ y se edita en el Inspector.
using System;
using UnityEngine;

public enum Speaker { Vega, Bimo, Mateo, Luna, Commander, System }

public enum Expression { Neutral, Happy, Worried }

[Serializable]
public class DialogueLine
{
    public Speaker speaker;
    public Expression expression;
    [TextArea(2, 5)] public string text;
    [Tooltip("Opcional: id de un elemento de la UI que se anima mientras se muestra esta línea (p. ej. 'energy', 'time', 'science').")]
    public string highlight;
}

[CreateAssetMenu(menuName = "MarsCrafts/Dialogue", fileName = "NewDialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;

    public static string SpeakerName(Speaker s)
    {
        switch (s)
        {
            case Speaker.Vega: return "Dra. Elena Vega";
            case Speaker.Bimo: return "BIMO";
            case Speaker.Mateo: return "Mateo Rojas";
            case Speaker.Luna: return "Luna Torres";
            case Speaker.Commander: return "Comandante";
            default: return "";
        }
    }

    public static Sprite Portrait(Speaker s, Expression e = Expression.Neutral)
    {
        var th = UITheme.Current;
        if (th == null) return null;
        Sprite pick = null;
        switch (s)
        {
            case Speaker.Vega:
                pick = e == Expression.Happy ? th.portraitVegaHappy : e == Expression.Worried ? th.portraitVegaWorried : null;
                return pick != null ? pick : th.portraitVega;
            case Speaker.Bimo:
                pick = e == Expression.Happy ? th.portraitBimoHappy : e == Expression.Worried ? th.portraitBimoAlert : null;
                return pick != null ? pick : th.portraitBimo;
            default: return null;
        }
    }
}
