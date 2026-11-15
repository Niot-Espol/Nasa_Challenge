// Assets/_Project/Scripts/Gameplay/HubDoor.cs
// Puerta al taller. Se abre después del briefing con la Dra. Vega.
using UnityEngine;

public class HubDoor : Interactable
{
    [SerializeField] bool unlocked;
    [SerializeField] SpriteRenderer doorSprite;
    [SerializeField] Sprite openSprite;                    // arte pendiente (puerta abierta)
    [SerializeField] Color lockedColor = new Color(0.45f, 0.3f, 0.2f);
    [SerializeField] Color openColor = new Color(0.4f, 0.85f, 0.5f);
    [SerializeField] DialogueData lockedMessage;           // BIMO: "Primero habla con la Dra. Vega"

    public override string PromptText => unlocked ? "Entrar al Taller" : "Taller  ·  primero habla con la Dra. Vega";

    protected override void Start()
    {
        base.Start();
        ApplyVisual();
    }

    public void Unlock()
    {
        unlocked = true;
        ApplyVisual();
    }

    protected override void Interact()
    {
        if (unlocked) { AudioManager.Play(Sfx.DoorOpen); SceneFlow.Go(SceneFlow.Workshop); }
        else
        {
            AudioManager.Play(Sfx.Error);
            if (DialogueBox.Instance != null) DialogueBox.Instance.Show(lockedMessage);
        }
    }

    void ApplyVisual()
    {
        if (doorSprite == null) return;
        if (unlocked && openSprite != null) doorSprite.sprite = openSprite;
        if (doorSprite.sprite == null || openSprite == null) doorSprite.color = unlocked ? openColor : lockedColor;
    }
}
