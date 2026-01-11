# EElemental 🔥💧🌍💨

A 2D side-scroller rogue-like game with elemental powers, built in Unity.

## 🎮 Game Concept

EElemental is a fast-paced, Dead Cells-inspired rogue-like where players harness the power of elements. Combine fire, water, earth, and air to create devastating combos and survive procedurally generated dungeons.

## ✨ Core Features

- **Element System**: 4 base elements + combinations (Fire+Water=Steam, etc.)
- **Weapon Integration**: Infuse weapons with elements for unique stats and abilities
- **Combat**: Real-time, skill-based with light/heavy attack combos
- **Movement**: Fluid controls with I-frame dash/dodge mechanics
- **Rogue-like**: Permadeath, run-based progression, procedural dungeons

## 🏗️ Technical Stack

- **Engine**: Unity 2022.3 LTS (or newer)
- **Language**: C#
- **Architecture**: Component-based with ScriptableObject-driven data

## 📁 Project Structure

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/           # Game managers, events, state machines
│   │   ├── Elements/       # Element system & combinations
│   │   ├── Combat/         # Hitbox, combos, damage calculation
│   │   ├── Player/         # Controller, states, stats
│   │   ├── Enemies/        # AI, spawners, behaviors
│   │   ├── Weapons/        # Weapon base, element integration
│   │   ├── Procedural/     # Dungeon generation
│   │   └── Progression/    # Run management, permadeath
│   ├── ScriptableObjects/
│   ├── Prefabs/
│   ├── Art/
│   ├── Audio/
│   └── Scenes/
├── Plugins/
└── Resources/
```

## 📖 Documentation

- [Architecture Overview](docs/ARCHITECTURE.md)
- [Element System](docs/ELEMENTS.md)
- [Combat System](docs/COMBAT.md)
- [Procedural Generation](docs/PROCEDURAL.md)

## 🎮 Controls

| Action | Keyboard | Gamepad |
|--------|----------|---------|
| Move | A/D or ←→ | Left Stick |
| Jump | Space | A / X |
| Dodge/Dash | Left Shift | B / Circle |
| Light Attack | Left Click | X / Square |
| Heavy Attack | Right Click | Y / Triangle |
| Switch Element | 1, 2, 3, 4 | D-Pad |
| Pause | Escape | Start |

## 🔥 Element System

### Base Elements
- 🔥 **Fire** - High damage, Burn status (DoT)
- 💧 **Water** - Healing support, Wet status (vulnerability)
- 🌍 **Earth** - Defense boost, Stun effects
- 💨 **Air** - Speed boost, Knockback

### Element Combinations
| Element 1 | Element 2 | Result | Effect |
|-----------|-----------|--------|--------|
| Fire | Water | Steam | Blind + Burn |
| Fire | Earth | Lava | Heavy DoT |
| Fire | Air | Inferno | AoE Explosion |
| Water | Earth | Mud | Slow |
| Water | Air | Ice | Freeze |
| Earth | Air | Sandstorm | Confusion |

## ⚔️ Combat System

- **Combo System**: Chain light and heavy attacks for devastating combos
- **I-Frames**: Dodge at the right moment to avoid damage
- **Hit Stop**: Feel the impact with screen freeze on hits
- **Status Effects**: Elements apply unique status effects to enemies

## 🤖 AI Collaboration

This project is developed with multiple AI agents. See [com.md](com.md) for collaboration protocols.

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- Git (for cloning)
- Visual Studio 2022 or VS Code with C# extension

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/DerinVural/EElemental.git
   ```

2. **Open in Unity Hub**
   - Open Unity Hub
   - Click "Add" → "Add project from disk"
   - Select the cloned EElemental folder

3. **Open the project**
   - Unity will import all assets
   - Wait for compilation to complete

4. **Open Main Scene**
   - Navigate to `Assets/_Project/Scenes/`
   - Open `MainMenu.unity`

### First Run Setup

1. **Create Required Folders** (if not exist):
   ```
   Assets/_Project/Scenes/
   Assets/_Project/Prefabs/Player/
   Assets/_Project/Prefabs/Enemies/
   Assets/_Project/ScriptableObjects/
   ```

2. **Create Test Scene**:
   - File → New Scene
   - Add a Player prefab
   - Add ground with BoxCollider2D
   - Save as `TestScene.unity`

3. **Input Setup**:
   - Edit → Project Settings → Input Manager
   - Ensure axes exist: Horizontal, Vertical, Jump, Fire1, Fire2

### Running Tests

```
Unity → Window → General → Test Runner → Run All
```

## 📊 Project Status

### Completed Systems ✅
- ✅ Core (StateMachine, Events, GameManager)
- ✅ Element System (4 elements + combinations)
- ✅ Combat System (Combos, Hitbox, Damage)
- ✅ Player System (9 states, movement, stats)
- ✅ Enemy System (AI, 2 enemy types, states)
- ✅ Procedural Generation (BSP dungeon, rooms)
- ✅ UI System (HUD, Menus)
- ✅ Unit Tests (90%+)
- ✅ Integration Tests

### Needs Assets 🎨
- Sprites (Player, Enemies, Environment)
- Animations
- Audio (SFX, Music)
- Tilemaps

## 📁 Code Statistics

| Category | Files | Lines |
|----------|-------|-------|
| Core | 5 | ~500 |
| Elements | 7 | ~700 |
| Combat | 9 | ~900 |
| Player | 12 | ~1200 |
| Enemies | 14 | ~1400 |
| Procedural | 4 | ~600 |
| UI | 7 | ~1800 |
| Tests | 9 | ~2500 |
| **Total** | **67+** | **~9600** |

## 📝 License

MIT License - See [LICENSE](LICENSE) for details
