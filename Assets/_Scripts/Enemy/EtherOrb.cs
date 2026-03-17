using UnityEngine;

public class EtherOrb : MonoBehaviour
{
    public EtherType etherType = EtherType.White;
    public int value = 1;
    public float accel = 25f;
    public float maxSpeed = 18f;
    public float collectDistance = 0.35f;

    Transform target;
    Vector2 vel;

    public void SetTarget(Transform t) => target = t;

    void Update()
    {
        if (target == null) return;

        Vector2 pos = transform.position;
        Vector2 to = (Vector2)target.position - pos;
        float dist = to.magnitude;

        if (dist <= collectDistance)
        {
            Debug.Log($"Collect {etherType} {value} ");
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
