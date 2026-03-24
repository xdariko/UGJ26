using UnityEngine;
using UnityEngine.InputSystem;

public class ClickAoeAttack : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float holdAttackInterval = 0.12f;

    private Camera cam;
    private readonly Collider2D[] hits = new Collider2D[64];
    private ContactFilter2D filter;

    private float nextHoldAttackTime;

    private void Awake()
    {
        cam = Camera.main;

        filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = enemyMask;
        filter.useTriggers = true;
    }

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null || cam == null) return;

        bool pressedThisFrame = mouse.leftButton.wasPressedThisFrame;
        bool isHeld = mouse.leftButton.isPressed;

        if (pressedThisFrame)
        {
            PerformAttack(mouse);
            nextHoldAttackTime = Time.time + holdAttackInterval;
            return;
        }

        if (isHeld && Time.time >= nextHoldAttackTime)
        {
            PerformAttack(mouse);
            nextHoldAttackTime = Time.time + holdAttackInterval;
        }
    }

    private void PerformAttack(Mouse mouse)
    {
        Vector2 screenPos = mouse.position.ReadValue();
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        Vector2 point = new Vector2(world.x, world.y);

        int count = Physics2D.OverlapCircle(point, G.ClickRadius, filter, hits);
        bool anyCrit = false;

        for (int i = 0; i < count; i++)
        {
            Enemy enemy = hits[i].GetComponent<Enemy>();
            if (enemy == null) continue;

            bool isCrit = Random.value < G.CritChance;
            int damage = isCrit
                ? Mathf.RoundToInt(G.ClickDamage * G.CritMultiplier)
                : G.ClickDamage;

            enemy.TakeDamage(damage, isCrit);

            if (isCrit)
                anyCrit = true;
        }

        if (anyCrit && G.screenShake != null)
            G.screenShake.Shake(1.2f);
    }
}