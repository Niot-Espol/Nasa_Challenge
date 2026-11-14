// Assets/_Project/Scripts/Gameplay/TopDownMover.cs
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMover : MonoBehaviour
{
    [SerializeField] float speed = 4f;

    public Vector2 Facing { get; private set; } = Vector2.down; // para animaciones e interacción
    public bool IsMoving => input.sqrMagnitude > 0.01f;          // para gastar energía mientras se mueve
    public bool CanMove { get; set; } = true;                    // se apaga en diálogos o en modo seguro

    Rigidbody2D rb;
    Vector2 input;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        var k = Keyboard.current;
        if (k == null || !CanMove) { input = Vector2.zero; return; }

        float x = (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1 : 0)
                - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1 : 0);
        float y = (k.wKey.isPressed || k.upArrowKey.isPressed ? 1 : 0)
                - (k.sKey.isPressed || k.downArrowKey.isPressed ? 1 : 0);

        input = new Vector2(x, y).normalized; // normalizado: en diagonal no va más rápido
        if (IsMoving) Facing = input;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
