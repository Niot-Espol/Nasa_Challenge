// Assets/_Project/Scripts/Gameplay/DirectionalSpriteAnimator.cs
// Animación simple por sprites para objetos que se mueven con TopDownMover y NO usan Animator
// (p. ej. el rover: 3 direcciones × 2 frames). Izquierda = derecha con flipX.
// Si los arreglos están vacíos, no toca el sprite (sirve el placeholder actual).
using UnityEngine;

[RequireComponent(typeof(TopDownMover))]
public class DirectionalSpriteAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer target;
    [SerializeField] Sprite[] down;
    [SerializeField] Sprite[] up;
    [SerializeField] Sprite[] side;        // mirando a la derecha
    [SerializeField] float framesPerSecond = 6f;

    TopDownMover mover;
    float t;

    void Awake()
    {
        mover = GetComponent<TopDownMover>();
        if (target == null) target = GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        Vector2 f = mover.Facing;
        bool vertical = Mathf.Abs(f.y) >= Mathf.Abs(f.x);
        Sprite[] set = vertical ? (f.y > 0 ? up : down) : side;
        if (set == null || set.Length == 0 || target == null) return;

        if (mover.IsMoving) t += Time.deltaTime * framesPerSecond; else t = 0;
        target.sprite = set[(int)t % set.Length];
        target.flipX = !vertical && f.x < 0;
    }
}
