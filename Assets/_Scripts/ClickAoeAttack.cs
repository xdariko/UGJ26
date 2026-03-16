using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickAoeAttack : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;

    private Camera cam;
    private readonly Collider2D[] hits = new Collider2D[64];
    private ContactFilter2D filter;

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
        if (! mouse.leftButton.wasPressedThisFrame) return;

        Vector2 screenPos = mouse.position.ReadValue();
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        Vector2 point = new Vector2(world.x, world.y);

        int count = Physics2D.OverlapCircle(point, G.ClickRadius, filter, hits);
        bool anyCrit = false;

        for (int i = 0; i < count; i++)
        {
            var enemy = hits[i].GetComponent<Enemy>();
            if (enemy == null) continue;

            bool isCrit = Random.value < G.CritChance;
            int damage = isCrit ? Mathf.RoundToInt(G.ClickDamage * G.CritMultiplier) : G.ClickDamage;

            enemy.TakeDamage(damage, isCrit);
            if (isCrit) anyCrit = true;
        }

        if (anyCrit && G.screenShake != null)
            G.screenShake.Shake(1.2f);
    }
}
