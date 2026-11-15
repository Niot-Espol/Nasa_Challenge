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
            mc.PostMessage(s.Design.UsesSolar ? Loc.T("Sombra permanente: sin Sol para los paneles y mucho frío.", "Permanent shadow: no sunlight for the panels and very cold.") : Loc.T("Sombra permanente: hace mucho frío, pero el RTG sigue dando energía.", "Permanent shadow: it's very cold, but the RTG keeps supplying power."));
            if (!shadowExplained)
            {
                shadowExplained = true;
                mc.Explain(s.Design.UsesSolar
                    ? new[] { Loc.T("¡Entramos en sombra permanente! Aquí nunca llega el Sol: nuestros paneles solares NO recargan.", "We're in permanent shadow! Sunlight never reaches here: our solar panels will NOT recharge."),
                              Loc.T($"Y el frío extremo gasta {coldDrainPerSecond:0} de energía por segundo para mantener el rover caliente. ¡Entra, escanea y sal rápido!", $"And the extreme cold uses {coldDrainPerSecond:0} energy per second to keep the rover warm. Get in, scan and get out fast!") }
                    : new[] { Loc.T("¡Entramos en sombra permanente! Hace muchísimo frío y eso gasta energía.", "We're in permanent shadow! It's extremely cold and that uses energy."),
                              Loc.T("Por suerte nuestro RTG no depende del Sol: sigue recargando en la oscuridad.", "Luckily our RTG doesn't depend on the Sun: it keeps recharging in the dark.") });
            }
        }
        else
        {
            if (noSignalCount++ > 0) return;
            bool relay = s.Design.BlackoutSeconds <= 20f;
            if (relay)
            {
                mc.PostMessage(Loc.T("Dentro del cráter: el relé vía orbitador mantiene la señal.", "Inside the crater: the orbital relay keeps the signal."));
                if (!noSignalExplained) { noSignalExplained = true; mc.Explain(new[] { Loc.T("Desde el fondo del cráter no se ve la Tierra... pero el orbitador de relé sí nos ve. ¡Seguimos con señal!", "Earth can't be seen from the crater floor... but the relay orbiter can see us. We still have signal!") }); }
                return;
            }
            mc.CommsZoneActive = true;
            s.SetCommsDown(true);
            AudioManager.Play(Sfx.CommsLost);
            mc.PostMessage(Loc.T("Sin señal: desde el cráter la Tierra queda bajo el horizonte.", "No signal: from the crater, Earth is below the horizon."));
            if (!noSignalExplained)
            {
                noSignalExplained = true;
                mc.Explain(new[] { Loc.T("¡Perdimos la señal! Desde el fondo del cráter la Tierra queda bajo el horizonte y la antena directa no la alcanza.", "We lost the signal! From the crater floor Earth is below the horizon and the direct antenna can't reach it."),
                                   Loc.T("Sin señal no podemos enviar datos, así que aquí no podemos escanear. Con un relé vía orbitador sí podríamos.", "Without signal we can't send data, so we can't scan here. With an orbital relay we could.") });
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
            mc.PostMessage(s.Design.UsesSolar ? Loc.T("De vuelta al Sol: los paneles vuelven a recargar.", "Back in the sunlight: the panels are recharging again.") : Loc.T("Salimos de la sombra.", "We're out of the shadow."));
        }
        else
        {
            if (--noSignalCount > 0) return;
            noSignalCount = 0;
            if (!mc.CommsZoneActive) return;
            mc.CommsZoneActive = false;
            s.SetCommsDown(false);
            AudioManager.Play(Sfx.CommsBack);
            mc.PostMessage(Loc.T("Señal recuperada: la Tierra vuelve a estar a la vista.", "Signal restored: Earth is in view again."));
        }
    }
}
