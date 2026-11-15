// Assets/_Project/Scripts/Gameplay/EnvironmentZone.cs
// Zona del mapa que cambia las condiciones mientras el rover está dentro (misión de la Luna):
//   · Shadow:   sombra permanente → los paneles solares no recargan y el frío gasta energía.
//   · NoSignal: dentro del cráter la Tierra no se ve → sin señal (no se puede escanear),
//               salvo que el rover lleve el relé vía orbitador.
// Necesita un Collider2D marcado como trigger. La primera vez que entras, BIMO lo explica.
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnvironmentZone : MonoBehaviour
{
    public enum Kind { Shadow, NoSignal }

    [SerializeField] Kind kind = Kind.Shadow;
    [SerializeField] float coldDrainPerSecond = 2f;   // solo sombra

    // Varias zonas del mismo tipo pueden solaparse: se cuenta cuántas ocupa el rover
    static int shadowCount, noSignalCount;
    static bool shadowExplained, noSignalExplained;

    void Reset() => GetComponent<Collider2D>().isTrigger = true;

    // Se reinician al cargar la escena (los estáticos sobreviven entre escenas)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { shadowCount = noSignalCount = 0; shadowExplained = noSignalExplained = false; }
    void Awake() { if (Time.timeSinceLevelLoad < 0.1f) { shadowCount = noSignalCount = 0; shadowExplained = noSignalExplained = false; } }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<TopDownMover>() == null) return;
        var mc = MissionController.Instance;
        if (mc == null || mc.State.IsOver) return;
        var s = mc.State;

        if (kind == Kind.Shadow)
        {
            if (shadowCount++ > 0) return;
            s.SetInShadow(true, coldDrainPerSecond);
            mc.PostMessage(s.Design.UsesSolar ? "Sombra permanente: sin Sol para los paneles y mucho frío." : "Sombra permanente: hace mucho frío, pero el RTG sigue dando energía.");
            if (!shadowExplained)
            {
                shadowExplained = true;
                mc.Explain(s.Design.UsesSolar
                    ? new[] { "¡Entramos en sombra permanente! Aquí nunca llega el Sol: nuestros paneles solares NO recargan.",
                              $"Y el frío extremo gasta {coldDrainPerSecond:0} de energía por segundo para mantener el rover caliente. ¡Entra, escanea y sal rápido!" }
                    : new[] { "¡Entramos en sombra permanente! Hace muchísimo frío y eso gasta energía.",
                              "Por suerte nuestro RTG no depende del Sol: sigue recargando en la oscuridad." });
            }
        }
        else
        {
            if (noSignalCount++ > 0) return;
            bool relay = s.Design.BlackoutSeconds <= 20f;
            if (relay)
            {
                mc.PostMessage("Dentro del cráter: el relé vía orbitador mantiene la señal.");
                if (!noSignalExplained) { noSignalExplained = true; mc.Explain(new[] { "Desde el fondo del cráter no se ve la Tierra... pero el orbitador de relé sí nos ve. ¡Seguimos con señal!" }); }
                return;
            }
            mc.CommsZoneActive = true;
            s.SetCommsDown(true);
            AudioManager.Play(Sfx.CommsLost);
            mc.PostMessage("Sin señal: desde el cráter la Tierra queda bajo el horizonte.");
            if (!noSignalExplained)
            {
                noSignalExplained = true;
                mc.Explain(new[] { "¡Perdimos la señal! Desde el fondo del cráter la Tierra queda bajo el horizonte y la antena directa no la alcanza.",
                                   "Sin señal no podemos enviar datos, así que aquí no podemos escanear. Con un relé vía orbitador sí podríamos." });
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<TopDownMover>() == null) return;
        var mc = MissionController.Instance;
        if (mc == null) return;
        var s = mc.State;

        if (kind == Kind.Shadow)
        {
            if (--shadowCount > 0) return;
            shadowCount = 0;
            s.SetInShadow(false);
            mc.PostMessage(s.Design.UsesSolar ? "De vuelta al Sol: los paneles vuelven a recargar." : "Salimos de la sombra.");
        }
        else
        {
            if (--noSignalCount > 0) return;
            noSignalCount = 0;
            if (!mc.CommsZoneActive) return;
            mc.CommsZoneActive = false;
            s.SetCommsDown(false);
            AudioManager.Play(Sfx.CommsBack);
            mc.PostMessage("Señal recuperada: la Tierra vuelve a estar a la vista.");
        }
    }
}
