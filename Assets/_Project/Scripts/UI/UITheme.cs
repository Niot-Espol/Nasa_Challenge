// Assets/_Project/Scripts/UI/UITheme.cs
// ÚNICO lugar donde se define el aspecto de la interfaz.
// Cuando llegue el arte: abrir Assets/_Project/Resources/UITheme.asset y arrastrar los sprites.
// Todo elemento con ThemedImage / ThemedText se actualiza solo (si un sprite está vacío, usa el color plano).
using System;
using TMPro;
using UnityEngine;

// Tipos de elemento visual. Agregar aquí si aparece uno nuevo.
public enum UIElement
{
    Panel,          // marco general (9-slice)
    PanelDark,      // panel secundario / fondo de lista
    Button,         // botón normal (9-slice)
    ButtonPrimary,  // botón principal (Lanzar, Empezar)
    DialogueBox,    // caja de diálogo con retrato
    FactPopup,      // popup de dato NASA
    Slot,           // tarjeta de opción del taller
    SlotSelected,   // tarjeta elegida
    BarFrame,       // marco de barra (energía, tiempo, presupuesto)
    BarFill,        // relleno de barra
    Overlay,        // fondo oscuro detrás de popups
}

[Serializable]
public class UIElementStyle
{
    public UIElement element;
    public Sprite sprite;                       // vacío = color plano
    public Color color = Color.white;           // tinte (o color plano si no hay sprite)
    public Image9Slice slice = Image9Slice.Sliced;
    [Tooltip("Grosor del borde 9-slice: 1 = píxeles a ×4, 2 = ×2, 4 = ×1 (más alto = borde más fino).")]
    public float borderThinness = 1f;
}

public enum Image9Slice { Simple, Sliced, Tiled }

[CreateAssetMenu(menuName = "MarsCrafts/UI Theme", fileName = "UITheme")]
public class UITheme : ScriptableObject
{
    [Header("Elementos (sprite 9-slice + color)")]
    public UIElementStyle[] elements;

    [Header("Botones (estados)")]
    public Sprite buttonHover;
    public Sprite buttonPressed;

    [Header("Texto")]
    public TMP_FontAsset font;                  // textos largos (Nunito)
    public TMP_FontAsset titleFont;             // títulos y números cortos (Pixelify Sans)
    public Color textColor = new Color(0.95f, 0.95f, 0.95f);
    public Color textMuted = new Color(0.65f, 0.7f, 0.8f);
    public Color buttonText = new Color(0.06f, 0.09f, 0.15f);   // texto sobre botones claros

    [Header("Colores de estado")]
    public Color accent = new Color(1f, 0.55f, 0.15f);   // naranja del traje del protagonista
    public Color good = new Color(0.45f, 0.9f, 0.5f);
    public Color warning = new Color(1f, 0.8f, 0.3f);
    public Color error = new Color(1f, 0.4f, 0.4f);

    [Header("Íconos de recursos (16x16)")]
    public Sprite iconBudget;
    public Sprite iconMass;
    public Sprite iconEnergy;
    public Sprite iconTime;
    public Sprite iconComms;
    public Sprite iconScience;
    public Sprite iconWarning;                   // aviso de BIMO
    public Sprite iconSafeMode;
    public Sprite iconNoSignal;
    public Sprite iconLock;                      // misiones bloqueadas

    [Header("Reporte")]
    public Sprite starFull;
    public Sprite starEmpty;
    public Sprite badgeCommander;
    public Sprite badgeEfficient;
    public Sprite badgeNoSafeMode;

    [Header("Título")]
    public Sprite logo;                          // ~192x64
    public Sprite titleBackground;               // 320x180
    public Sprite workshopBackground;            // fondo del Taller 320x180
    public Sprite missionEarth;                  // tarjetas 64x64
    public Sprite missionMoon;
    public Sprite missionMars;

    [Header("Lanzamiento")]
    public Sprite launchBackground;              // plataforma 320x180
    public Sprite spaceBackground;               // estrellas 320x180
    public Sprite rocket;                        // 32x64
    public Sprite[] rocketFlame;                 // frames de la llama (se animan en bucle)
    public Sprite[] launchSmoke;                 // frames del humo del despegue
    public Sprite earth;                         // 64x64
    public Sprite mars;                          // 64x64
    public Sprite moon;                          // 64x64 (misión de la Luna)
    public Sprite cruiseShip;                    // 32x32

    [Header("Taller")]
    public Sprite roverPreview;                  // rover de perfil 96x96 (un solo dibujo)

    [Header("Retratos (neutral / alegre / preocupado-alarmado)")]
    public Sprite portraitVega;
    public Sprite portraitVegaHappy;
    public Sprite portraitVegaWorried;
    public Sprite portraitBimo;
    public Sprite portraitBimoHappy;
    public Sprite portraitBimoAlert;

    // --- Acceso global ---
    static UITheme current;
    public static UITheme Current
    {
        get
        {
            if (current == null) current = Resources.Load<UITheme>("UITheme");
            return current;
        }
    }

    public UIElementStyle Get(UIElement e)
    {
        if (elements != null)
            foreach (var s in elements) if (s.element == e) return s;
        return null;
    }
}
