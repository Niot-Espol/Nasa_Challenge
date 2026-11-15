// Assets/_Project/Scripts/UI/MarsScreenEffects.cs
// Efectos de pantalla de 04_Mars (todo por código, sin arte):
// - Polvo de la tormenta: dos capas de ruido que se desplazan + tinte naranja, con fundido.
// - Energía baja: viñeta roja que late (más fuerte en modo seguro).
using UnityEngine;
using UnityEngine.UI;

public class MarsScreenEffects : MonoBehaviour
{
    [SerializeField] RawImage dustA;
    [SerializeField] RawImage dustB;
    [SerializeField] Image dustTint;
    [SerializeField] RawImage lowEnergyVignette;

    [SerializeField] float dustMaxAlpha = 0.4f;
    [SerializeField] float tintMaxAlpha = 0.1f;

    float dust;   // 0..1

    void Start()
    {
        var tex = ProceduralSprites.Dust();
        if (dustA != null) dustA.texture = tex;
        if (dustB != null) dustB.texture = tex;
        if (lowEnergyVignette != null) lowEnergyVignette.texture = ProceduralSprites.Vignette();
        Apply(0f, 0f);
    }

    void Update()
    {
        var mc = MissionController.Instance;
        if (mc == null) return;
        var s = mc.State;

        // Polvo con fundido de entrada/salida
        float target = s.DustStormActive ? 1f : 0f;
        dust = Mathf.MoveTowards(dust, target, Time.unscaledDeltaTime / 1.5f);
        float t = Time.unscaledTime;
        if (dustA != null) dustA.uvRect = new Rect(t * 0.12f, t * 0.03f, 1.6f, 0.9f);
        if (dustB != null) dustB.uvRect = new Rect(-t * 0.2f + 0.37f, t * 0.05f + 0.21f, 2.4f, 1.35f);

        // Viñeta de energía baja
        float vig = 0f;
        if (!s.IsOver)
        {
            if (s.InSafeMode) vig = 0.45f + 0.25f * Mathf.Sin(t * 5f);
            else if (s.Energy < s.MaxEnergy * MissionController.LowEnergyFraction) vig = 0.2f + 0.2f * Mathf.Sin(t * 7f);
        }
        Apply(dust, vig);
    }

    void Apply(float d, float vignette)
    {
        if (dustA != null) dustA.color = new Color(1, 1, 1, dustMaxAlpha * d);
        if (dustB != null) dustB.color = new Color(1, 1, 1, dustMaxAlpha * 0.7f * d);
        if (dustTint != null) dustTint.color = new Color(0.85f, 0.45f, 0.2f, tintMaxAlpha * d);
        if (lowEnergyVignette != null) lowEnergyVignette.color = new Color(1f, 0.15f, 0.1f, Mathf.Max(0f, vignette));
    }
}
