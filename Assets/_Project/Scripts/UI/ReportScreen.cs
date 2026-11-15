// Assets/_Project/Scripts/UI/ReportScreen.cs
// Pantalla del Reporte (05_Report). Lee GameSession, arma el ReportData y lo muestra.
// El layout estático está en la escena; aquí solo se llenan los contenedores.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportScreen : MonoBehaviour
{
    [Header("Contenedores (en la escena)")]
    [SerializeField] TMP_Text gradeText;
    [SerializeField] Transform starsRow;
    [SerializeField] Transform badgesRow;
    [SerializeField] TMP_Text resourcesText;
    [SerializeField] Transform linesList;
    [SerializeField] Transform comparisonList;

    [SerializeField] float starSize = 72f;

    void Start()
    {
        var design = GameSession.EnsureDesign();
        var result = GameSession.EnsureResult();
        Show(ReportBuilder.Build(design, result));
    }

    public void Show(ReportData rep)
    {
        var th = UITheme.Current;

        if (gradeText != null) gradeText.text = rep.Grade;

        // Estrellas: sprite si existe, si no un "★" de texto
        if (starsRow != null)
        {
            UIKit.Clear(starsRow);
            for (int i = 0; i < 3; i++)
            {
                bool on = i < rep.Stars;
                Sprite s = th != null ? (on ? th.starFull : th.starEmpty) : null;
                if (s != null) UIKit.Icon(starsRow, "Star", s, new Vector2(starSize, starSize));
                else
                {
                    // Sin sprite todavía: cuadro de color (naranja = ganada, gris = vacía)
                    var img = UIKit.Icon(starsRow, "Star", null, new Vector2(starSize, starSize));
                    img.enabled = true;
                    img.color = th != null ? (on ? th.accent : new Color(0.3f, 0.34f, 0.42f)) : Color.white;
                }
            }
        }

        if (badgesRow != null)
        {
            UIKit.Clear(badgesRow);
            if (rep.Badges.Count == 0)
                UIKit.Size(UIKit.Text(badgesRow, "None", "Sin insignias esta vez", 24, TextAlignmentOptions.Left, TextTone.Muted), 400, 40);
            foreach (var b in rep.Badges) AddBadge(b, th);
        }

        if (resourcesText != null)
        {
            int used = Mathf.RoundToInt(rep.TimeUsed);
            resourcesText.text =
                $"Presupuesto: {rep.CostUsed}/{rep.Budget} créditos\n" +
                $"Masa: {rep.MassUsed}/{rep.MassCapacity} kg\n" +
                $"Energía gastada: {rep.EnergyConsumed:0}\n" +
                $"Tiempo: {used / 60}:{used % 60:00} de {Mathf.RoundToInt(rep.TimeLimit) / 60}:{Mathf.RoundToInt(rep.TimeLimit) % 60:00}";
        }

        Fill(linesList, rep.Lines);
        Fill(comparisonList, rep.Comparison);
    }

    void AddBadge(Badge b, UITheme th)
    {
        Sprite s = th == null ? null
                 : b == Badge.Commander ? th.badgeCommander
                 : b == Badge.Efficient ? th.badgeEfficient
                 : th.badgeNoSafeMode;

        var row = UIKit.NewRect("Badge_" + b, badgesRow);
        UIKit.HList(row.gameObject, 8);
        if (s != null) UIKit.Icon(row, "Icon", s, new Vector2(48, 48));
        var t = UIKit.Text(row, "Name", ReportBuilder.BadgeName(b), 28, TextAlignmentOptions.Left, TextTone.Accent);
        UIKit.Size(t, -1, 48);
    }

    static void Fill(Transform list, System.Collections.Generic.List<ReportLine> lines)
    {
        if (list == null) return;
        UIKit.Clear(list);
        foreach (var l in lines)
        {
            // Solo caracteres que tiene cualquier fuente (la pixel font puede no traer símbolos).
            string mark = l.Tone == ReportTone.Good ? "<color=#73E67F><b>+</b></color>  "
                        : l.Tone == ReportTone.Bad ? "<color=#FF6B6B><b>x</b></color>  "
                        : "<color=#A6B3CC>-</color>  ";
            var t = UIKit.Text(list, "Line", mark + l.Text, 26, TextAlignmentOptions.TopLeft);
            var fitter = t.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
