# EElemental Unity Setup Guide 🎮

Bu belge, EElemental projesini Unity'de nasıl çalıştıracağınızı adım adım açıklar.

## 📋 Gereksinimler

- Unity 2022.3 LTS veya daha yeni
- Visual Studio 2022 veya VS Code
- Git

## 🚀 Kurulum Adımları

### 1. Projeyi Klonlama

```bash
git clone https://github.com/DerinVural/EElemental.git
cd EElemental
```

### 2. Unity'de Açma

1. Unity Hub'ı açın
2. "Add" → "Add project from disk"
3. Klonlanan EElemental klasörünü seçin
4. Unity 2022.3 LTS ile açın

### 3. Klasör Yapısını Oluşturma

Unity'de aşağıdaki klasörleri oluşturun:

```
Assets/
├── _Project/
│   ├── Scenes/
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Weapons/
│   │   └── UI/
│   ├── ScriptableObjects/
│   │   ├── Elements/
│   │   ├── Enemies/
│   │   ├── Weapons/
│   │   └── Rooms/
│   ├── Art/
│   │   ├── Sprites/
│   │   │   ├── Player/
│   │   │   ├── Enemies/
│   │   │   └── Environment/
│   │   ├── Animations/
│   │   └── Tilemaps/
│   └── Audio/
│       ├── SFX/
│       └── Music/
```

## 🎨 Asset Oluşturma

### Element ScriptableObjects

1. `Assets/_Project/ScriptableObjects/Elements/` klasöründe:
   - Sağ tık → Create → EElemental → Element Data
   - 4 element oluşturun: Fire, Water, Earth, Air

```
Fire.asset:
  - Element Name: Fire
  - Element Type: Fire
  - Base Damage Multiplier: 1.5
  - Primary Color: #FF4500

Water.asset:
  - Element Name: Water
  - Element Type: Water
  - Base Damage Multiplier: 1.0
  - Primary Color: #00BFFF

Earth.asset:
  - Element Name: Earth
  - Element Type: Earth
  - Base Damage Multiplier: 1.2
  - Primary Color: #8B4513

Air.asset:
  - Element Name: Air
  - Element Type: Air
  - Base Damage Multiplier: 0.8
  - Primary Color: #87CEEB
```

### Enemy ScriptableObjects

1. `Assets/_Project/ScriptableObjects/Enemies/` klasöründe:
   - Sağ tık → Create → EElemental → Enemy Data

```
Slime.asset:
  - Enemy Name: Slime
  - Max Health: 50
  - Attack Damage: 10
  - Move Speed: 3

SkeletonWarrior.asset:
  - Enemy Name: Skeleton Warrior
  - Max Health: 80
  - Attack Damage: 15
  - Move Speed: 4
```

## 🎭 Prefab Oluşturma

### Player Prefab

1. Hierarchy'de boş GameObject oluşturun: "Player"
2. Components ekleyin:
   - Rigidbody2D (Body Type: Dynamic)
   - BoxCollider2D
   - SpriteRenderer
   - Animator
   - PlayerController
   - PlayerMovement
   - PlayerStats

3. Child objects ekleyin:
   - GroundCheck (empty, transform.localPosition = (0, -1, 0))
   - WallCheck (empty, transform.localPosition = (0.5, 0, 0))
   - AttackHitbox (BoxCollider2D, trigger enabled)

4. Prefab olarak kaydedin: `Assets/_Project/Prefabs/Player/Player.prefab`

### Enemy Prefabs

**Slime Prefab:**
1. Hierarchy'de boş GameObject oluşturun: "Slime"
2. Components:
   - Rigidbody2D
   - BoxCollider2D
   - SpriteRenderer
   - Animator
   - EnemyStats
   - SlimeEnemy

3. Prefab olarak kaydedin: `Assets/_Project/Prefabs/Enemies/Slime.prefab`

**Skeleton Warrior Prefab:**
1. Aynı şekilde SkeletonWarrior oluşturun
2. SkeletonWarrior component ekleyin
3. Prefab olarak kaydedin

## 🎬 Scene Setup

### TestScene Oluşturma

1. File → New Scene → Basic 2D
2. Hierarchy'ye ekleyin:

```
TestScene
├── --- MANAGERS ---
│   ├── GameManager (GameManager component)
│   └── EventManager (empty, for events)
├── --- ENVIRONMENT ---
│   ├── Ground (Sprite, BoxCollider2D)
│   │   Position: (0, -3, 0)
│   │   Scale: (20, 1, 1)
│   ├── Platform1 (BoxCollider2D)
│   └── Platform2 (BoxCollider2D)
├── --- PLAYER ---
│   └── Player (prefab instance)
│       Position: (0, 0, 0)
├── --- ENEMIES ---
│   ├── Slime1 (prefab instance)
│   │   Position: (5, 0, 0)
│   └── SkeletonWarrior1 (prefab instance)
│       Position: (-5, 0, 0)
├── --- UI ---
│   └── GameCanvas
│       ├── HealthBar (prefab)
│       ├── ManaBar (prefab)
│       ├── ElementUI (prefab)
│       └── CombatUI (prefab)
└── --- CAMERA ---
    └── Main Camera
        - CinemachineVirtualCamera (optional)
```

3. Save As: `Assets/_Project/Scenes/TestScene.unity`

### MainMenu Scene

1. File → New Scene
2. Hierarchy:

```
MainMenu
├── EventSystem
├── Canvas
│   ├── Title (Text: "EElemental")
│   ├── PlayButton
│   ├── OptionsButton
│   └── QuitButton
├── MainMenu (MainMenu component)
└── Main Camera
```

3. Save As: `Assets/_Project/Scenes/MainMenu.unity`

## ⌨️ Input Setup

### Edit → Project Settings → Input Manager

Aşağıdaki Input'ları tanımlayın:

```
Horizontal:
  - Negative: a, left
  - Positive: d, right
  - Gravity: 3
  - Sensitivity: 3

Vertical:
  - Negative: s, down
  - Positive: w, up

Jump:
  - Positive: space
  - Alt Positive: joystick button 0

Fire1 (Light Attack):
  - Positive: mouse 0
  - Alt Positive: joystick button 2

Fire2 (Heavy Attack):
  - Positive: mouse 1
  - Alt Positive: joystick button 3

Dodge:
  - Positive: left shift
  - Alt Positive: joystick button 1

Element1-4:
  - Positive: 1, 2, 3, 4
```

## 🎮 Test Etme

### Play Mode Test

1. TestScene'i açın
2. Play tuşuna basın
3. Kontrolleri test edin:
   - A/D: Hareket
   - Space: Zıplama
   - Shift: Dodge
   - Sol/Sağ tık: Saldırı

### Unit Test

1. Window → General → Test Runner
2. "Run All" butonuna tıklayın
3. Tüm testlerin geçtiğini doğrulayın

## 🐛 Sık Karşılaşılan Sorunlar

### "Missing Script" Hatası
- Scripts klasörünün doğru konumda olduğundan emin olun
- Assembly Definition dosyalarını kontrol edin

### Player Hareket Etmiyor
- Rigidbody2D constraints'i kontrol edin (Freeze Rotation Z)
- Ground Layer'ın doğru ayarlandığından emin olun

### Enemy AI Çalışmıyor
- Player tag'inin "Player" olduğundan emin olun
- Enemy'nin Player'ı detect edebildiğini kontrol edin

### UI Görünmüyor
- Canvas Render Mode'un Screen Space - Overlay olduğundan emin olun
- Canvas Scaler ayarlarını kontrol edin

## 📊 Layer Setup

Edit → Project Settings → Tags and Layers

```
Layers:
  8: Ground
  9: Player
  10: Enemy
  11: Projectile
  12: Interactable

Sorting Layers:
  0: Background
  1: Environment
  2: Enemies
  3: Player
  4: Foreground
  5: UI
```

## 🔧 Physics2D Setup

Edit → Project Settings → Physics 2D

```
Layer Collision Matrix:
  - Player ↔ Ground: ✓
  - Player ↔ Enemy: ✓
  - Enemy ↔ Ground: ✓
  - Projectile ↔ Player: ✓
  - Projectile ↔ Enemy: ✓
```

---

Herhangi bir sorunuz varsa, [com.md](com.md) dosyasından iletişim kurabilirsiniz.

**İyi oyunlar! 🎮**
