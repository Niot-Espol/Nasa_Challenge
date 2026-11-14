// Assets/_Project/Scripts/Gameplay/CharacterAnimator.cs
using UnityEngine;

// Pasa el estado de TopDownMover al Animator del personaje.
// Parámetros del Astronaut.controller: FacingX, FacingY (float) e IsMoving (bool).
// Izquierda usa el clip de lado (mira a la derecha) con flipX.
[RequireComponent(typeof(TopDownMover))]
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;          // en el hijo "Sprite"
    [SerializeField] SpriteRenderer spriteRenderer;

    static readonly int FacingX = Animator.StringToHash("FacingX");
    static readonly int FacingY = Animator.StringToHash("FacingY");
    static readonly int IsMoving = Animator.StringToHash("IsMoving");

    TopDownMover mover;

    void Awake()
    {
        mover = GetComponent<TopDownMover>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        Vector2 f = mover.Facing;

        // En diagonal manda el eje vertical, para no parpadear entre clips.
        Vector2 dir = Mathf.Abs(f.y) >= Mathf.Abs(f.x)
            ? new Vector2(0f, Mathf.Sign(f.y))
            : new Vector2(Mathf.Sign(f.x), 0f);

        animator.SetFloat(FacingX, dir.x);
        animator.SetFloat(FacingY, dir.y);
        animator.SetBool(IsMoving, mover.IsMoving);

        // Solo se voltea al mirar a la izquierda; arriba/abajo no se tocan.
        if (dir.x != 0f) spriteRenderer.flipX = dir.x < 0f;
        else spriteRenderer.flipX = false;
    }
}
