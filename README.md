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

## 🤖 AI Collaboration

This project is developed with multiple AI agents. See [com.md](com.md) for collaboration protocols.

## 🚀 Getting Started

1. Clone the repository
2. Open with Unity 2022.3 LTS or newer
3. Open `Assets/_Project/Scenes/MainMenu.unity`

## 📝 License

MIT License - See [LICENSE](LICENSE) for details
