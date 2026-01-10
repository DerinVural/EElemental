# Combat System ⚔️

## Dead Cells Referans Analizi

Dead Cells'den alınacak temel özellikler:
- Hızlı, responsive kontroller
- Cancel mekanikleri (attack → dodge)
- I-frame dodge
- Hitbox/Hurtbox precision
- Input buffering
- Hitstop/Hitlag feedback

---

## Combat Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                         INPUT LAYER                              │
│  (New Input System - Buffered inputs, input queue)              │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                      PLAYER STATE MACHINE                        │
│  (Determines valid actions based on current state)              │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                       COMBO HANDLER                              │
│  (Tracks combo state, validates sequences)                      │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                      HITBOX MANAGER                              │
│  (Activates hitboxes, detects collisions)                       │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                    DAMAGE CALCULATOR                             │
│  (Base damage + Element + Crit + Enemy weakness)                │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                     EFFECT APPLIER                               │
│  (Hitstop, knockback, status effects, VFX/SFX)                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Attack System

### Light Attack
- Hızlı, düşük damage
- Combo starter
- Düşük recovery time

### Heavy Attack
- Yavaş, yüksek damage
- Combo finisher
- Yüksek recovery time
- Armor break potansiyeli

### Attack Data Structure

```csharp
[CreateAssetMenu(fileName = "NewAttack", menuName = "EElemental/Attack")]
public class AttackData : ScriptableObject
{
    [Header("Identity")]
    public string attackName;
    public AttackType type; // Light, Heavy, Special

    [Header("Timing (frames at 60fps)")]
    public int startupFrames = 5;      // Wind-up
    public int activeFrames = 3;        // Hitbox active
    public int recoveryFrames = 10;     // Cool-down

    [Header("Damage")]
    public float baseDamage = 10f;
    public float critMultiplier = 1.5f;
    public float critChance = 0.1f;

    [Header("Knockback")]
    public Vector2 knockbackDirection = Vector2.right;
    public float knockbackForce = 5f;

    [Header("Hitstop")]
    public int hitstopFrames = 3;
    public float screenShakeIntensity = 0.1f;

    [Header("Cancels")]
    public bool canCancelIntoLight = false;
    public bool canCancelIntoHeavy = false;
    public bool canCancelIntoDash = true;
    public int cancelWindowStart = 8; // Frame where cancel becomes available

    [Header("Animation")]
    public string animationTrigger;
    public GameObject hitVFX;
    public AudioClip hitSFX;
}

public enum AttackType
{
    Light,
    Heavy,
    Special,
    Air,
    Dash
}
```

---

## Combo System

### Combo Notation
```
L = Light Attack
H = Heavy Attack
D = Dash
J = Jump

Örnek Combo'lar:
L → L → L        = Basic 3-hit combo
L → L → H        = Launcher combo
L → H            = Quick heavy
J + L            = Air light
D + L            = Dash attack
L → L → D → L    = Cancel into dash attack
```

### Combo Handler Implementation

```csharp
public class ComboHandler : MonoBehaviour
{
    [SerializeField] private ComboData[] availableCombos;
    [SerializeField] private float comboWindowTime = 0.5f;

    private List<AttackInput> inputBuffer = new List<AttackInput>();
    private float lastInputTime;
    private int currentComboIndex = 0;

    public void RegisterInput(AttackInput input)
    {
        float currentTime = Time.time;

        // Reset combo if window expired
        if (currentTime - lastInputTime > comboWindowTime)
        {
            inputBuffer.Clear();
            currentComboIndex = 0;
        }

        inputBuffer.Add(input);
        lastInputTime = currentTime;

        // Check for valid combo continuation
        AttackData nextAttack = GetNextComboAttack();
        if (nextAttack != null)
        {
            ExecuteAttack(nextAttack);
        }
    }

    private AttackData GetNextComboAttack()
    {
        foreach (var combo in availableCombos)
        {
            if (MatchesCombo(combo, inputBuffer))
            {
                currentComboIndex++;
                return combo.attacks[currentComboIndex - 1];
            }
        }
        return null;
    }

    private bool MatchesCombo(ComboData combo, List<AttackInput> inputs)
    {
        if (inputs.Count > combo.sequence.Length) return false;

        for (int i = 0; i < inputs.Count; i++)
        {
            if (inputs[i] != combo.sequence[i]) return false;
        }
        return true;
    }
}

[System.Serializable]
public class ComboData
{
    public string comboName;
    public AttackInput[] sequence;
    public AttackData[] attacks;
}

public enum AttackInput
{
    Light,
    Heavy,
    Special
}
```

---

## Hitbox/Hurtbox System

### Yaklaşım: Collider-based

```csharp
public class HitboxManager : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;

    private Collider2D hitboxCollider;
    private AttackData currentAttack;
    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();

    public void ActivateHitbox(AttackData attack, float duration)
    {
        currentAttack = attack;
        alreadyHit.Clear();
        hitboxCollider.enabled = true;

        StartCoroutine(DeactivateAfter(duration));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyHit.Contains(other)) return;
        if (!IsValidTarget(other)) return;

        alreadyHit.Add(other);

        // Get hurtbox component
        if (other.TryGetComponent<HurtboxManager>(out var hurtbox))
        {
            DamageInfo damage = CalculateDamage();
            hurtbox.TakeDamage(damage);

            ApplyHitEffects(other.transform.position);
        }
    }

    private DamageInfo CalculateDamage()
    {
        return new DamageInfo
        {
            amount = currentAttack.baseDamage,
            knockback = currentAttack.knockbackDirection * currentAttack.knockbackForce,
            hitstopFrames = currentAttack.hitstopFrames,
            attacker = transform.root.gameObject
        };
    }
}

public class HurtboxManager : MonoBehaviour
{
    public event Action<DamageInfo> OnDamageReceived;

    private IFrameController iFrames;

    public void TakeDamage(DamageInfo damage)
    {
        if (iFrames != null && iFrames.IsInvincible) return;

        OnDamageReceived?.Invoke(damage);
    }
}

public struct DamageInfo
{
    public float amount;
    public Vector2 knockback;
    public int hitstopFrames;
    public ElementType element;
    public GameObject attacker;
    public bool isCrit;
}
```

---

## I-Frame (Invincibility Frame) System

### Dash I-Frames

```csharp
public class IFrameController : MonoBehaviour
{
    public bool IsInvincible { get; private set; }

    [SerializeField] private float dashIFrameDuration = 0.3f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Coroutine iFrameCoroutine;

    public void StartDashIFrames()
    {
        if (iFrameCoroutine != null)
            StopCoroutine(iFrameCoroutine);

        iFrameCoroutine = StartCoroutine(IFrameRoutine(dashIFrameDuration));
    }

    public void StartHurtIFrames(float duration)
    {
        if (iFrameCoroutine != null)
            StopCoroutine(iFrameCoroutine);

        iFrameCoroutine = StartCoroutine(IFrameRoutine(duration));
    }

    private IEnumerator IFrameRoutine(float duration)
    {
        IsInvincible = true;

        // Visual feedback: flicker
        float flickerInterval = 0.05f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        spriteRenderer.enabled = true;
        IsInvincible = false;
    }
}
```

---

## Hitstop System

```csharp
public class HitstopManager : MonoBehaviour
{
    private static HitstopManager instance;
    public static HitstopManager Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    public void ApplyHitstop(int frames)
    {
        StartCoroutine(HitstopRoutine(frames));
    }

    private IEnumerator HitstopRoutine(int frames)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.05f; // Near-freeze

        // Wait for frame count (adjusted for slow-mo)
        float waitTime = frames / 60f;
        yield return new WaitForSecondsRealtime(waitTime);

        Time.timeScale = originalTimeScale;
    }
}
```

---

## Input Buffering

```csharp
public class InputBuffer : MonoBehaviour
{
    [SerializeField] private int bufferFrames = 6; // ~100ms at 60fps

    private Queue<BufferedInput> inputQueue = new Queue<BufferedInput>();

    public void BufferInput(AttackInput input)
    {
        inputQueue.Enqueue(new BufferedInput
        {
            input = input,
            frameStamp = Time.frameCount
        });
    }

    public AttackInput? ConsumeBuffer()
    {
        // Remove stale inputs
        while (inputQueue.Count > 0)
        {
            var buffered = inputQueue.Peek();

            if (Time.frameCount - buffered.frameStamp > bufferFrames)
            {
                inputQueue.Dequeue();
                continue;
            }

            return inputQueue.Dequeue().input;
        }

        return null;
    }

    private struct BufferedInput
    {
        public AttackInput input;
        public int frameStamp;
    }
}
```

---

## Damage Calculation Formula

```csharp
public static class DamageCalculator
{
    public static float Calculate(
        float baseDamage,
        float attackMultiplier,
        float elementMultiplier,
        float critMultiplier,
        float weaknessMultiplier,
        float defense)
    {
        // Base calculation
        float damage = baseDamage * attackMultiplier;

        // Element bonus
        damage *= elementMultiplier;

        // Crit
        damage *= critMultiplier;

        // Weakness
        damage *= weaknessMultiplier;

        // Defense reduction (diminishing returns)
        float reduction = defense / (defense + 100f);
        damage *= (1f - reduction);

        return Mathf.Max(1f, damage); // Minimum 1 damage
    }
}
```

---

## Combat States

```csharp
public class PlayerAttackState : IState
{
    private PlayerStateMachine stateMachine;
    private AttackData currentAttack;
    private int currentFrame;
    private bool hitboxActivated;

    public void Enter()
    {
        currentFrame = 0;
        hitboxActivated = false;

        // Play animation
        stateMachine.Animator.SetTrigger(currentAttack.animationTrigger);
    }

    public void Execute()
    {
        currentFrame++;

        // Startup frames - vulnerable, no hitbox
        if (currentFrame < currentAttack.startupFrames)
        {
            return;
        }

        // Active frames - hitbox on
        if (currentFrame < currentAttack.startupFrames + currentAttack.activeFrames)
        {
            if (!hitboxActivated)
            {
                stateMachine.HitboxManager.ActivateHitbox(currentAttack);
                hitboxActivated = true;
            }
            return;
        }

        // Recovery frames - can cancel after window
        if (currentFrame >= currentAttack.cancelWindowStart)
        {
            // Check for buffered inputs
            HandleCancelWindow();
        }

        // Attack complete
        if (currentFrame >= GetTotalFrames())
        {
            stateMachine.TransitionTo(stateMachine.IdleState);
        }
    }

    private void HandleCancelWindow()
    {
        var bufferedInput = stateMachine.InputBuffer.ConsumeBuffer();

        if (bufferedInput == AttackInput.Light && currentAttack.canCancelIntoLight)
        {
            // Continue combo
            stateMachine.ComboHandler.RegisterInput(bufferedInput.Value);
        }
        else if (stateMachine.Input.DashPressed && currentAttack.canCancelIntoDash)
        {
            // Cancel into dash
            stateMachine.TransitionTo(stateMachine.DashState);
        }
    }

    private int GetTotalFrames()
    {
        return currentAttack.startupFrames +
               currentAttack.activeFrames +
               currentAttack.recoveryFrames;
    }
}
```

---

## Örnek Combo Chain: L → L → L

```
Frame Timeline (60fps):

Light 1:
[0-4]   Startup   (5 frames)
[5-7]   Active    (3 frames) ← Hitbox ON
[8-17]  Recovery  (10 frames)
        Cancel window starts at frame 8

Light 2 (buffered):
[8-12]  Startup   (5 frames)
[13-15] Active    (3 frames) ← Hitbox ON
[16-25] Recovery  (10 frames)
        Cancel window starts at frame 16

Light 3 (buffered):
[16-20] Startup   (5 frames)
[21-23] Active    (3 frames) ← Hitbox ON
[24-33] Recovery  (10 frames)
        No more cancels → return to Idle
```

---

## Sonraki Adımlar

1. [ ] PlayerStateMachine base implementation
2. [ ] Attack states (Light, Heavy)
3. [ ] HitboxManager & HurtboxManager
4. [ ] IFrameController
5. [ ] InputBuffer
6. [ ] ComboHandler
7. [ ] Basic enemy to test combat
8. [ ] Hitstop & screen shake
