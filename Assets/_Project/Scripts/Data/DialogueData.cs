// Assets/_Project/Scripts/Data/DialogueData.cs
// Un diálogo = lista de líneas (quién habla + texto). Se crea como asset
// (Create > MarsCrafts > Dialogue) en Assets/_Project/Data/Dialogues/ y se edita en el Inspector.
using System;
using UnityEngine;

public enum Speaker { Vega, Bimo, Mateo, Luna, Commander, System }

[Serializable]
public class DialogueLine
{
    public Speaker speaker;
    [TextArea(2, 5)] public string text;
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

    public static Sprite Portrait(Speaker s)
    {
        var th = UITheme.Current;
        if (th == null) return null;
        switch (s)
        {
            case Speaker.Vega: return th.portraitVega;
            case Speaker.Bimo: return th.portraitBimo;
            default: return null;
        }
    }
}
