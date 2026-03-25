using System.Collections;
using UnityEngine;

public class EnemyJumpPatrol : MonoBehaviour
{
    [Header("Enable")]
    [SerializeField] private bool enabledJumpPatrol = true;

    [Header("Idle")]
    [SerializeField] private float idleTimeMin = 1.2f;
    [SerializeField] private float idleTimeMax = 2.5f;

    [Header("Jump Move")]
    [SerializeField] private float jumpDuration = 0.45f;
    [SerializeField] private float jumpHeight = 1.25f;

    [Header("Jump Range From Current Position")]
    [SerializeField] private float jumpRadiusMin = 1.0f;
    [SerializeField] private float jumpRadiusMax = 2.5f;
    [SerializeField] private int maxPointTries = 25;

    [Header("Links")]
    [SerializeField] private Transform spriteRoot;
    [SerializeField] private Animator animator;

    [Header("Animator Params")]
    [SerializeField] private string jumpTriggerName = "Jump";

    private int jumpTriggerHash;

    private SpawnShape spawnShape;
    private float minRadius;
    private float maxRadius;
    private Vector2 rectHalfSize;

    private Enemy enemy;
    private Coroutine jumpRoutine;

    private bool isDead;
    private bool isJumping;
    private bool isSetupDone;

    private Vector3 spriteStartLocalPos;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (spriteRoot == null && animator != null)
            spriteRoot = animator.transform;

        if (spriteRoot != null)
            spriteStartLocalPos = spriteRoot.localPosition;

        jumpTriggerHash = Animator.StringToHash(jumpTriggerName);

        //Debug.Log($"[EnemyJumpPatrol] Awake on {name}. Animator found: {animator != null}", this);
    }

    private void Start()
    {
        if (enemy != null)
            enemy.OnDied += HandleDeath;
    }

    private void OnDestroy()
    {
        if (enemy != null)
            enemy.OnDied -= HandleDeath;
    }

    private void OnDisable()
    {
        StopJumpRoutine();

        if (spriteRoot != null)
            spriteRoot.localPosition = spriteStartLocalPos;
    }

    private void HandleDeath()
    {
        isDead = true;
        enabledJumpPatrol = false;

        StopJumpRoutine();

        if (spriteRoot != null)
            spriteRoot.localPosition = spriteStartLocalPos;
    }

    private void StopJumpRoutine()
    {
        if (jumpRoutine != null)
        {
            StopCoroutine(jumpRoutine);
            jumpRoutine = null;
        }

        isJumping = false;
    }

    public void Setup(EnemySpawnEntry data)
    {
        spawnShape = data.spawnShape;
        minRadius = data.minRadius;
        maxRadius = data.maxRadius;
        rectHalfSize = data.rectHalfSize;
        isSetupDone = true;

        //Debug.Log(
        //    $"[EnemyJumpPatrol] Setup on {name}: shape={spawnShape}, minRadius={minRadius}, maxRadius={maxRadius}, rectHalfSize={rectHalfSize}",
        //    this
        //);

        StopJumpRoutine();

        if (enabledJumpPatrol && gameObject.activeInHierarchy && !isDead)
            jumpRoutine = StartCoroutine(JumpLoop());
    }

    private IEnumerator JumpLoop()
    {
        if (!isSetupDone)
        {
            Debug.LogError($"[EnemyJumpPatrol] Setup was not called on {name}. Jump patrol will not work.", this);
            yield break;
        }

        while (enabledJumpPatrol && !isDead)
        {
            float idleDelay = Random.Range(idleTimeMin, idleTimeMax);
            yield return new WaitForSeconds(idleDelay);

            if (isDead || isJumping)
                continue;

            Vector2 center = G.circleCenter.position;
            Vector2 current = transform.position;
            Vector2 target = GetNearbyJumpPoint(center, current);

            if ((target - current).sqrMagnitude < 0.01f)
            {
                Debug.LogWarning($"[EnemyJumpPatrol] No valid jump point found for {name}. Current={current}", this);
                continue;
            }

            //Debug.Log($"[EnemyJumpPatrol] Jump from {current} to {target}", this);
            yield return JumpTo(target);
        }

        jumpRoutine = null;
    }

    private IEnumerator JumpTo(Vector2 target)
    {
        isJumping = true;

        Vector2 start = transform.position;
        Vector2 direction = target - start;

        FlipToDirection(direction);

        if (animator != null)
        {
            animator.ResetTrigger(jumpTriggerName);
            animator.SetTrigger(jumpTriggerHash);
        }
        else
        {
            Debug.LogWarning($"[EnemyJumpPatrol] Animator is missing on {name}", this);
        }

        float time = 0f;

        while (time < jumpDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / jumpDuration);

            Vector2 groundPos = Vector2.Lerp(start, target, t);
            transform.position = new Vector3(groundPos.x, groundPos.y, transform.position.z);

            if (spriteRoot != null)
            {
                float yOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
                spriteRoot.localPosition = new Vector3(
                    spriteStartLocalPos.x,
                    spriteStartLocalPos.y + yOffset,
                    spriteStartLocalPos.z
                );
            }

            yield return null;
        }

        transform.position = new Vector3(target.x, target.y, transform.position.z);

        if (spriteRoot != null)
            spriteRoot.localPosition = spriteStartLocalPos;

        isJumping = false;
    }

    private void FlipToDirection(Vector2 direction)
    {
        if (spriteRoot == null)
            return;

        if (Mathf.Abs(direction.x) < 0.01f)
            return;

        Vector3 scale = spriteRoot.localScale;
        scale.x = direction.x > 0f ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        spriteRoot.localScale = scale;
    }

    private Vector2 GetNearbyJumpPoint(Vector2 center, Vector2 current)
    {
        for (int i = 0; i < maxPointTries; i++)
        {
            float distance = Random.Range(jumpRadiusMin, jumpRadiusMax);
            float angle = Random.Range(0f, Mathf.PI * 2f);

            Vector2 candidate = current + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

            if (IsPointAllowed(center, candidate))
                return candidate;
        }

        return current;
    }

    private bool IsPointAllowed(Vector2 center, Vector2 point)
    {
        float distToCenterSqr = (point - center).sqrMagnitude;
        float minRadiusSqr = minRadius * minRadius;

        if (distToCenterSqr < minRadiusSqr)
            return false;

        if (spawnShape == SpawnShape.Ring)
        {
            float maxRadiusSqr = maxRadius * maxRadius;
            return distToCenterSqr <= maxRadiusSqr;
        }

        if (spawnShape == SpawnShape.Rect)
        {
            return point.x >= center.x - rectHalfSize.x &&
                   point.x <= center.x + rectHalfSize.x &&
                   point.y >= center.y - rectHalfSize.y &&
                   point.y <= center.y + rectHalfSize.y;
        }

        return false;
    }
}