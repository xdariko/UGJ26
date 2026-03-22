using UnityEngine;

public class EtherOrb : MonoBehaviour
{
    public EtherType etherType = EtherType.White;
    public int value = 1;
    public float accel = 25f;
    public float maxSpeed = 18f;
    public float collectDistance = 0.35f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite purpleSprite;

    Transform target;
    Vector2 vel;

    public void SetTarget(Transform t) => target = t;

    public void Setup(EtherType type, int orbValue, Transform t)
    {
        etherType = type;
        value = orbValue;
        target = t;
        ApplyVisual();
    }

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        ApplyVisual();
    }

    void ApplyVisual()
    {
        if (spriteRenderer == null) return;

        switch (etherType)
        {
            case EtherType.White:
                spriteRenderer.sprite = whiteSprite;
                break;

            case EtherType.Red:
                spriteRenderer.sprite = redSprite;
                break;

            case EtherType.Purple:
                spriteRenderer.sprite = purpleSprite;
                break;
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector2 pos = transform.position;
        Vector2 to = (Vector2)target.position - pos;
        float dist = to.magnitude;

        if (dist <= collectDistance)
        {
            //Debug.Log($"Collect {etherType} {value} ");
            G.AddEther(etherType, value);
            Destroy(gameObject);
            return;
        }

        Vector2 dir = to / Mathf.Max(dist, 0.0001f);
        vel += dir * (accel * Time.deltaTime);

        float speed = vel.magnitude;
        if (speed > maxSpeed) vel = vel / speed * maxSpeed;

        transform.position += (Vector3)(vel * Time.deltaTime);
    }
}
