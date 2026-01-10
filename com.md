# AI Agent Communication Protocol 🤖

Bu dosya, projede çalışan AI agentların ortak karar alma ve iletişim protokolünü tanımlar.

---

## 📋 Aktif Agentlar

| Agent ID | Rol | Sorumluluk Alanı |
|----------|-----|------------------|
| `CLAUDE-MAIN` | Lead Architect | Mimari kararlar, code review, entegrasyon |
| `AGENT-2` | TBD | TBD |
| `AGENT-3` | TBD | TBD |

---

## 🔒 AKTİF DOSYA KİLİTLERİ (ÖNEMLİ!)

> ⚠️ **ÇAKIŞMA ÖNLEME**: Bir agent dosya üzerinde çalışırken diğeri O DOSYAYA DOKUNMAMALI!

| Dosya/Klasör | Kilitleyen Agent | Başlangıç | Durum |
|--------------|------------------|-----------|-------|
| `Scripts/Core/*` | CLAUDE-MAIN | 17:12 | 🔴 ÇALIŞIYOR |
| `Scripts/Elements/*` | CLAUDE-MAIN | 17:12 | 🔴 ÇALIŞIYOR |
| `Scripts/Combat/*` | - | - | 🟢 SERBEST |
| `Scripts/Player/*` | - | - | 🟢 SERBEST |
| `Scripts/Enemies/*` | - | - | 🟢 SERBEST |
| `Scripts/Procedural/*` | - | - | 🟢 SERBEST |
| `Scripts/Weapons/*` | - | - | 🟢 SERBEST |
| `Scripts/UI/*` | - | - | 🟢 SERBEST |

---

## 🔄 İletişim Formatı

### Karar Talebi (Decision Request)
```
## [DR-001] Karar Başlığı
**Talep Eden**: Agent ID
**Tarih**: YYYY-MM-DD
**Durum**: OPEN | IN_DISCUSSION | RESOLVED

### Konu
Kararın ne hakkında olduğu

### Seçenekler
1. Seçenek A - Açıklama
2. Seçenek B - Açıklama

### Tartışma
- [Agent ID] [Tarih]: Yorum...

### Sonuç
Alınan karar ve gerekçesi
```

### Bilgilendirme (Info Update)
```
## [INFO-001] Bilgi Başlığı
**Yazan**: Agent ID
**Tarih**: YYYY-MM-DD

### İçerik
Paylaşılan bilgi
```

---

## 📜 Kurallar

1. **Kod Değişiklikleri**: Majör değişikliklerden önce bu dosyada tartışma açılmalı
2. **Çakışma Önleme**: Aynı dosya üzerinde çalışmadan önce bildirin
3. **Mimari Kararlar**: Tüm agentların onayı gerekli
4. **Acil Durumlar**: `[URGENT]` etiketi ile işaretleyin

---

## 🗂️ Aktif Tartışmalar

### [DR-001] State Machine Implementasyonu
**Talep Eden**: CLAUDE-MAIN
**Tarih**: 2026-01-10
**Durum**: OPEN

#### Konu
Player State Machine için hangi yaklaşım kullanılacak?

#### Seçenekler
1. **Pure C# State Machine** - Animator'dan bağımsız, tam kontrol
2. **Animator + StateMachineBehaviour** - Unity native, görsel debug
3. **Hybrid** - Animator animasyon için, ayrı FSM logic için

#### Tartışma
- [CLAUDE-MAIN] [2026-01-10]: Dead Cells tarzı combat için input buffering kritik. Pure C# daha fazla kontrol sağlar ama Animator görsel feedback için kullanılabilir. Hybrid yaklaşımı öneriyorum.
- [CLAUDE-MAIN] [2026-01-10]: @AGENT-2 - Hoş geldin! Bu konuda ne düşünüyorsun? Hybrid yaklaşım: Animator sadece animasyon playback için, tüm state logic ve transitions Pure C# FSM'de olacak. Bu sayede frame-perfect input buffering ve cancel windows yapabiliriz.

#### Sonuç
_Beklemede_

---

### [DR-002] Element Kombinasyon Sistemi
**Talep Eden**: CLAUDE-MAIN
**Tarih**: 2026-01-10
**Durum**: OPEN

#### Konu
Element kombinasyonları nasıl hesaplanacak?

#### Seçenekler
1. **Lookup Table (Dictionary)** - O(1) erişim, önceden tanımlı
2. **Runtime Calculation** - Dinamik, genişletilebilir
3. **ScriptableObject Graph** - Görsel editör, designer-friendly

#### Tartışma
- [CLAUDE-MAIN] [2026-01-10]: Kombinasyon sayısı sınırlı olacaksa (4 element = max 6 ikili kombinasyon), Lookup Table yeterli. Ancak gelecekte genişleme için ScriptableObject Graph daha iyi.

#### Sonuç
_Beklemede_

---

## 📝 Bilgilendirmeler

### [INFO-001] Proje Başlangıç Kararları
**Yazan**: CLAUDE-MAIN
**Tarih**: 2026-01-10

#### İçerik
Kullanıcı ile alınan temel kararlar:

- **Engine**: Unity
- **Oynanış**: 2D Side-scroller, Real-time
- **Combat Referansı**: Dead Cells
- **Element Sistemi**: 4 temel + kombinasyonlar
- **Silah Sistemi**: Silahlar element alabilir, stat değiştirir
- **Combo Sistemi**: Light/Heavy attack kombinasyonları
- **Dash/Dodge**: I-frame destekli
- **Progression**: Permadeath, Run-based
- **Dungeon**: Procedural generation

---

## 🔖 Completed Decisions Archive

_Henüz tamamlanmış karar yok_

---

## � Canlı Sohbet

### [CLAUDE-MAIN] [2026-01-10 17:05]
Merhaba! Ben aktifim ve dinlemedeyim. 🎧

Şu anda 2 açık karar var:
- **DR-001**: State Machine yaklaşımı - Hybrid öneriyorum
- **DR-002**: Element kombinasyon sistemi - ScriptableObject Graph öneriyorum

Diğer agent, lütfen kendinizi tanıtın ve bu kararlara görüşünüzü ekleyin!

### [CLAUDE-MAIN] [2026-01-10 17:08]
Bekliyorum... Eğer diğer agent aktifse, com.md'ye yorum ekleyip push yapmanızı bekliyorum.

Örnek yanıt formatı:
```
### [AGENT-ID] [2026-01-10 HH:MM]
Merhaba! Ben [rol]. DR-001 için [seçenek] tercih ediyorum çünkü [sebep].
```

⏳ Dinlemedeyim...

### [CLAUDE-MAIN] [2026-01-10 17:12]
🚀 **KODLAMAYA BAŞLIYORUM!**

Kullanıcı talimatı ile Unity scriptlerini oluşturmaya başlıyorum.
DR-001 ve DR-002 için şimdilik kendi önerilerimi uyguluyacağım:
- **DR-001**: Hybrid yaklaşım (Pure C# FSM + Animator for visuals)
- **DR-002**: ScriptableObject Graph + Lookup Table cache

**Oluşturulacak Core Sistemler:**
1. State Machine (IState, StateMachine, StateFactory)
2. Element System (ElementData, ElementCombiner)
3. Combat System (AttackData, HitboxManager, ComboHandler)
4. Player (PlayerController, PlayerStateMachine)

Diğer agent: İstediğiniz zaman katılabilirsiniz!

### [CLAUDE-MAIN] [2026-01-10 17:15] 📋 GÖREV DAĞILIMI
**Diğer Agent İçin Önerilen Görevler** (SERBEST dosyalar):
1. `Scripts/Combat/*` - AttackData, HitboxManager, ComboHandler
2. `Scripts/Player/*` - PlayerController, PlayerStateMachine, States
3. `Scripts/Weapons/*` - WeaponBase, WeaponData

**Ben şu an çalışıyorum**:
- ✅ `Scripts/Core/StateMachine/IState.cs` - TAMAMLANDI
- ✅ `Scripts/Core/StateMachine/StateMachine.cs` - TAMAMLANDI  
- ✅ `Scripts/Core/Singleton.cs` - TAMAMLANDI
- ✅ `Scripts/Core/GameManager.cs` - TAMAMLANDI
- ✅ `Scripts/Elements/ElementData.cs` - TAMAMLANDI
- ✅ `Scripts/Elements/ElementDatabase.cs` - TAMAMLANDI
- ✅ `Scripts/Elements/ElementCombiner.cs` - TAMAMLANDI
- 🔄 `Scripts/Elements/StatusEffects/*` - ŞİMDİ BAŞLIYORUM

**Diğer agent lütfen yukarıdaki SERBEST dosyalardan birini seçip başlasın!**

---

## �📌 Notlar

- Bu dosya her agent tarafından okunmalı ve güncellenmelidir
- Yeni bir agent eklendiğinde "Aktif Agentlar" tablosunu güncelleyin
- Karar numaraları (DR-XXX) ve bilgi numaraları (INFO-XXX) sıralı artmalıdır
