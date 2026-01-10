# Architecture Overview 🏗️

## Mimari Prensipler

1. **Separation of Concerns**: Her sistem kendi sorumluluğuna sahip
2. **ScriptableObject-Driven**: Data ve logic ayrımı
3. **Event-Driven Communication**: Loose coupling için
4. **Composition over Inheritance**: Flexible component yapısı

---

## Sistem Diyagramı

```
┌─────────────────────────────────────────────────────────────────┐
│                        GAME MANAGER                              │
│  (Singleton - Game State, Pause, Scene Management)              │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                      EVENT SYSTEM                                │
│  (Central message bus - decoupled communication)                │
└──────┬──────────┬──────────┬──────────┬──────────┬─────────────┘
       │          │          │          │          │
       ▼          ▼          ▼          ▼          ▼
┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│  PLAYER  │ │  COMBAT  │ │ ELEMENT  │ │  ENEMY   │ │PROCEDURAL│
│  SYSTEM  │ │  SYSTEM  │ │  SYSTEM  │ │  SYSTEM  │ │GENERATION│
└──────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘
```

---

## Core Systems

### 1. GameManager
```csharp
// Singleton pattern
// Responsibilities:
// - Game state (Playing, Paused, GameOver)
// - Scene transitions
// - Run initialization/cleanup
```

### 2. EventSystem
```csharp
// Custom event system veya Unity Events
// Key Events:
// - OnPlayerDamaged(float damage, ElementType element)
// - OnEnemyKilled(EnemyData enemy)
// - OnElementCombined(ElementType a, ElementType b, ElementType result)
// - OnRoomCleared()
// - OnRunEnded(bool victory)
```

### 3. PlayerSystem
```csharp
// Components:
// - PlayerController (Input handling)
// - PlayerStateMachine (State management)
// - PlayerStats (HP, Mana, etc.)
// - PlayerCombat (Attack execution)
// - PlayerMovement (Physics, dash)
```

### 4. CombatSystem
```csharp
// Components:
// - HitboxManager (Damage dealing)
// - HurtboxManager (Damage receiving)
// - ComboHandler (Input sequences)
// - DamageCalculator (Element modifiers)
// - IFrameController (Invincibility)
```

### 5. ElementSystem
```csharp
// Components:
// - ElementDatabase (All element definitions)
// - ElementCombiner (Combination logic)
// - ElementEffectApplier (Status effects)
// - WeaponElementIntegrator (Weapon buffs)
```

### 6. EnemySystem
```csharp
// Components:
// - EnemyAI (Behavior trees / State machines)
// - EnemySpawner (Wave management)
// - EnemyPool (Object pooling)
```

### 7. ProceduralSystem
```csharp
// Components:
// - DungeonGenerator (Room placement)
// - RoomDatabase (Room templates)
// - TileMapper (Tilemap generation)
// - DifficultyScaler (Progressive difficulty)
```

---

## State Machine Architecture

### Player States
```
                    ┌─────────┐
                    │  IDLE   │◄────────────────┐
                    └────┬────┘                 │
                         │ Move Input           │ No Input
                         ▼                      │
                    ┌─────────┐                 │
              ┌────►│   RUN   │─────────────────┘
              │     └────┬────┘
              │          │ Jump Input
              │          ▼
              │     ┌─────────┐
              │     │  JUMP   │◄──────┐
              │     └────┬────┘       │ Double Jump
              │          │────────────┘
              │          │ Land
              │          ▼
              │     ┌─────────┐
              └─────│  LAND   │
                    └────┬────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
        ▼                ▼                ▼
   ┌─────────┐     ┌──────────┐     ┌─────────┐
   │  DASH   │     │  ATTACK  │     │  HURT   │
   │(I-Frame)│     │(Combo)   │     │(Stagger)│
   └─────────┘     └──────────┘     └─────────┘
                         │
                         ▼
                   ┌──────────┐
                   │  DEATH   │
                   └──────────┘
```

### State Interface
```csharp
public interface IState
{
    void Enter();
    void Execute();
    void FixedExecute();
    void Exit();
    bool CanTransitionTo(IState newState);
}
```

---

## Data Flow

```
┌──────────────────┐
│ ScriptableObject │ (Data Definition)
│   - ElementData  │
│   - WeaponData   │
│   - EnemyData    │
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│    MonoBehaviour │ (Runtime Logic)
│   - Controllers  │
│   - Managers     │
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│   Event System   │ (Communication)
│   - Game Events  │
│   - UI Updates   │
└──────────────────┘
```

---

## Folder Structure (Unity)

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs
│   │   │   ├── EventSystem.cs
│   │   │   ├── StateMachine/
│   │   │   │   ├── IState.cs
│   │   │   │   ├── StateMachine.cs
│   │   │   │   └── StateFactory.cs
│   │   │   └── Singleton.cs
│   │   │
│   │   ├── Player/
│   │   │   ├── PlayerController.cs
│   │   │   ├── PlayerMovement.cs
│   │   │   ├── PlayerCombat.cs
│   │   │   ├── PlayerStats.cs
│   │   │   └── States/
│   │   │       ├── PlayerIdleState.cs
│   │   │       ├── PlayerRunState.cs
│   │   │       ├── PlayerJumpState.cs
│   │   │       ├── PlayerDashState.cs
│   │   │       ├── PlayerAttackState.cs
│   │   │       └── PlayerHurtState.cs
│   │   │
│   │   ├── Combat/
│   │   │   ├── HitboxManager.cs
│   │   │   ├── HurtboxManager.cs
│   │   │   ├── ComboHandler.cs
│   │   │   ├── DamageCalculator.cs
│   │   │   └── IFrameController.cs
│   │   │
│   │   ├── Elements/
│   │   │   ├── ElementDatabase.cs
│   │   │   ├── ElementCombiner.cs
│   │   │   ├── ElementEffectApplier.cs
│   │   │   └── Effects/
│   │   │       ├── BurnEffect.cs
│   │   │       ├── FreezeEffect.cs
│   │   │       └── ...
│   │   │
│   │   ├── Weapons/
│   │   │   ├── WeaponBase.cs
│   │   │   ├── WeaponElementIntegrator.cs
│   │   │   └── Types/
│   │   │       ├── Sword.cs
│   │   │       ├── Bow.cs
│   │   │       └── ...
│   │   │
│   │   ├── Enemies/
│   │   │   ├── EnemyBase.cs
│   │   │   ├── EnemyAI.cs
│   │   │   ├── EnemySpawner.cs
│   │   │   └── Types/
│   │   │       └── ...
│   │   │
│   │   ├── Procedural/
│   │   │   ├── DungeonGenerator.cs
│   │   │   ├── RoomPlacer.cs
│   │   │   └── TileMapper.cs
│   │   │
│   │   ├── Progression/
│   │   │   ├── RunManager.cs
│   │   │   └── PermadeathHandler.cs
│   │   │
│   │   └── UI/
│   │       ├── HUDController.cs
│   │       └── ...
│   │
│   ├── ScriptableObjects/
│   │   ├── Elements/
│   │   ├── Weapons/
│   │   ├── Enemies/
│   │   └── Rooms/
│   │
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Weapons/
│   │   ├── Effects/
│   │   └── UI/
│   │
│   ├── Art/
│   │   ├── Sprites/
│   │   ├── Animations/
│   │   └── Tilesets/
│   │
│   ├── Audio/
│   │   ├── Music/
│   │   └── SFX/
│   │
│   └── Scenes/
│       ├── MainMenu.unity
│       ├── Game.unity
│       └── Rooms/
│
├── Plugins/
└── Resources/
```

---

## Design Patterns Kullanımı

| Pattern | Kullanım Yeri | Sebep |
|---------|--------------|-------|
| Singleton | GameManager, AudioManager | Global erişim |
| State | Player, Enemy FSM | Clean state transitions |
| Observer | Event System | Loose coupling |
| Factory | StateFactory, EnemyFactory | Object creation |
| Object Pool | Bullets, Effects, Enemies | Performance |
| Strategy | AI Behaviors | Swappable algorithms |
| Composite | Combo System | Tree-like structures |

---

## Sonraki Adımlar

1. [ ] Core sistemlerin implementasyonu
2. [ ] Player state machine
3. [ ] Temel combat mekanikleri
4. [ ] Element sistemi prototip
5. [ ] Basit procedural generation
6. [ ] İlk playable demo
