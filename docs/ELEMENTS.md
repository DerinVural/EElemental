# Element System 🔥💧🌍💨

## Genel Bakış

Element sistemi oyunun temel mekaniğidir. Oyuncu elementleri silahlarına entegre eder ve düşmanlara karşı kullanır.

---

## Temel Elementler

| Element | Sembol | Renk | Temel Etki |
|---------|--------|------|------------|
| **Fire** 🔥 | `F` | Kırmızı/Turuncu | Burn (DoT) |
| **Water** 💧 | `W` | Mavi | Slow |
| **Earth** 🌍 | `E` | Kahverengi/Yeşil | Stun |
| **Air** 💨 | `A` | Beyaz/Açık Mavi | Knockback |

---

## Element Kombinasyonları

### İkili Kombinasyonlar

```
Fire + Water  = Steam  (💨🔥) → Blind + Light DoT
Fire + Earth  = Magma  (🌋)  → Heavy DoT + Slow
Fire + Air    = Inferno(🔥🔥) → Spread Fire + AoE
Water + Earth = Mud    (🟤)  → Heavy Slow + Root
Water + Air   = Ice    (❄️)  → Freeze + Shatter
Earth + Air   = Sand   (⏳)  → Blind + Chip Damage
```

### Kombinasyon Matrisi

|       | Fire | Water | Earth | Air |
|-------|------|-------|-------|-----|
| Fire  | -    | Steam | Magma | Inferno |
| Water | Steam| -     | Mud   | Ice |
| Earth | Magma| Mud   | -     | Sand |
| Air   | Inferno| Ice | Sand  | - |

---

## Element Data Structure

```csharp
[CreateAssetMenu(fileName = "NewElement", menuName = "EElemental/Element")]
public class ElementData : ScriptableObject
{
    [Header("Identity")]
    public string elementName;
    public ElementType type;
    public Sprite icon;
    public Color primaryColor;

    [Header("Combat Stats")]
    public float baseDamageMultiplier = 1f;
    public float critChanceBonus = 0f;
    public float critDamageBonus = 0f;

    [Header("Status Effect")]
    public StatusEffectData statusEffect;
    public float effectChance = 0.3f;
    public float effectDuration = 3f;

    [Header("Combinations")]
    public ElementCombination[] combinations;

    [Header("VFX/SFX")]
    public GameObject hitVFX;
    public AudioClip hitSFX;
}

[System.Serializable]
public class ElementCombination
{
    public ElementType combinesWith;
    public ElementData result;
}

public enum ElementType
{
    None,
    // Base Elements
    Fire,
    Water,
    Earth,
    Air,
    // Combined Elements
    Steam,
    Magma,
    Inferno,
    Mud,
    Ice,
    Sand
}
```

---

## Status Effects

### Burn (Fire)
```csharp
public class BurnEffect : StatusEffect
{
    // Damage over time
    // Spreads to nearby enemies (small chance)
    // Visual: Fire particles

    float damagePerTick = 5f;
    float tickInterval = 0.5f;
    float spreadRadius = 2f;
    float spreadChance = 0.1f;
}
```

### Slow (Water)
```csharp
public class SlowEffect : StatusEffect
{
    // Reduces movement speed
    // Reduces attack speed
    // Visual: Blue tint, water drips

    float movementReduction = 0.4f; // 40% slower
    float attackSpeedReduction = 0.2f; // 20% slower
}
```

### Stun (Earth)
```csharp
public class StunEffect : StatusEffect
{
    // Cannot move or attack
    // Short duration
    // Visual: Stars, rocks

    float stunDuration = 1f;
    bool interruptsActions = true;
}
```

### Knockback (Air)
```csharp
public class KnockbackEffect : StatusEffect
{
    // Pushes enemy away
    // Brief airborne state
    // Visual: Wind trails

    float knockbackForce = 10f;
    float airborneTime = 0.3f;
}
```

---

## Weapon Element Integration

Silahlar element aldığında şu değişiklikler olur:

```csharp
public class WeaponElementIntegrator : MonoBehaviour
{
    public WeaponData baseWeapon;
    public ElementData currentElement;

    public WeaponStats GetModifiedStats()
    {
        WeaponStats stats = baseWeapon.baseStats.Clone();

        if (currentElement != null)
        {
            // Damage modifier
            stats.damage *= currentElement.baseDamageMultiplier;

            // Crit bonuses
            stats.critChance += currentElement.critChanceBonus;
            stats.critDamage += currentElement.critDamageBonus;

            // Add element type for damage calculation
            stats.elementType = currentElement.type;
        }

        return stats;
    }

    public void ApplyElementToWeapon(ElementData element)
    {
        currentElement = element;
        UpdateVisuals();
        OnElementChanged?.Invoke(element);
    }
}
```

### Element + Weapon Stat Örnekleri

| Weapon | Base DMG | + Fire | + Water | + Earth | + Air |
|--------|----------|--------|---------|---------|-------|
| Sword  | 100      | 120 +Burn | 90 +Slow | 110 +Stun | 95 +KB |
| Bow    | 60       | 70 +Burn | 55 +Slow | 65 +Stun | 75 +KB |
| Staff  | 80       | 100 +Burn | 85 +Slow | 75 +Stun | 90 +KB |

---

## Element Combiner System

```csharp
public class ElementCombiner : MonoBehaviour
{
    [SerializeField] private ElementDatabase database;

    private Dictionary<(ElementType, ElementType), ElementData> combinationLookup;

    private void Awake()
    {
        BuildLookupTable();
    }

    private void BuildLookupTable()
    {
        combinationLookup = new Dictionary<(ElementType, ElementType), ElementData>();

        foreach (var element in database.allElements)
        {
            foreach (var combo in element.combinations)
            {
                var key1 = (element.type, combo.combinesWith);
                var key2 = (combo.combinesWith, element.type);

                if (!combinationLookup.ContainsKey(key1))
                    combinationLookup[key1] = combo.result;
                if (!combinationLookup.ContainsKey(key2))
                    combinationLookup[key2] = combo.result;
            }
        }
    }

    public ElementData TryCombine(ElementType a, ElementType b)
    {
        if (combinationLookup.TryGetValue((a, b), out ElementData result))
        {
            EventSystem.Instance.OnElementCombined?.Invoke(a, b, result.type);
            return result;
        }
        return null;
    }

    public bool CanCombine(ElementType a, ElementType b)
    {
        return combinationLookup.ContainsKey((a, b));
    }
}
```

---

## Element Weakness System (Opsiyonel)

Düşmanların element zayıflıkları olabilir:

```
Fire → weak to Water (1.5x damage)
Water → weak to Earth (1.5x damage)
Earth → weak to Air (1.5x damage)
Air → weak to Fire (1.5x damage)
```

```csharp
public class ElementalDamageCalculator
{
    private static readonly Dictionary<ElementType, ElementType> Weaknesses = new()
    {
        { ElementType.Fire, ElementType.Water },
        { ElementType.Water, ElementType.Earth },
        { ElementType.Earth, ElementType.Air },
        { ElementType.Air, ElementType.Fire }
    };

    public static float CalculateDamage(float baseDamage, ElementType attackElement, ElementType targetWeakness)
    {
        float multiplier = 1f;

        if (Weaknesses.TryGetValue(targetWeakness, out ElementType weak))
        {
            if (weak == attackElement)
                multiplier = 1.5f; // Super effective
        }

        return baseDamage * multiplier;
    }
}
```

---

## Visual Feedback

Her element için:

1. **Particle Effects**: Hit anında element-specific parçacıklar
2. **Trail Effects**: Silah swing sırasında element rengi trail
3. **Screen Effects**: Kombine element kullanıldığında subtle screen flash
4. **Enemy Tint**: Status effect aktifken düşman renk değişimi

---

## Ses Tasarımı

| Element | Hit Sound | Ambient Loop |
|---------|-----------|--------------|
| Fire | Whoosh + Crackle | Fire burning |
| Water | Splash + Drip | Water flow |
| Earth | Thud + Rumble | Stone grinding |
| Air | Swoosh + Whistle | Wind howl |

---

## Sonraki Adımlar

1. [ ] ElementData ScriptableObject implementasyonu
2. [ ] StatusEffect base class
3. [ ] Her element için StatusEffect implementasyonu
4. [ ] ElementCombiner sistemi
5. [ ] WeaponElementIntegrator
6. [ ] VFX sistem entegrasyonu
