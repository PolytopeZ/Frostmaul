# 🧊 FROSTMAUL — Project Roadmap
> Solo Maze Tower Defense Roguelite · Mobile (iOS & Android) · Unity 6
> 
> **Total estimated duration: ~33 weeks** from Prototype to Global Launch

---

## Overview

| Milestone                                              | Duration | Weeks   |
| ------------------------------------------------------ | -------- | ------- |
| [M1 — Prototype](#m1--prototype)                       | 4 weeks  | W1–W4   |
| [M2 — Alpha](#m2--alpha)                               | 8 weeks  | W5–W12  |
| [M3 — Beta](#m3--beta)                                 | 6 weeks  | W13–W18 |
| [M4 — Soft Launch](#m4--soft-launch)                   | 4 weeks  | W19–W22 |
| [M5 — Global Launch](#m5--global-launch)               | 2 weeks  | W23–W24 |
| [M6 — Post-Launch Season 1](#m6--post-launch-season-1) | 8 weeks  | W25–W33 |

---

## M1 — Prototype
> **Goal:** Validate the core loop — maze building, pathfinding, tower auto-fire, enemy waves. Nothing polished, everything functional.

### 🗂️ Project Setup
- [x] Create Unity 6 project with URP pipeline
- [x] Configure version control (Git + LFS for assets)
- [x] Set up folder structure (`/Art`, `/Audio`, `/Scripts`, `/Data`, `/Prefabs`, `/Scenes`)
- [x] Define code conventions and architecture patterns (ScriptableObject-driven design)
- [x] Set up build targets for iOS and Android

### 🏗️ Grid & Maze System
- [x] Implement 9×16 Tilemap grid with buildable / non-buildable cell flags
- [x] Create cell selection and tower placement via tap input
- [x] Implement Flow Field pathfinding (enemy navigation from top to bottom)
- [x] Implement real-time BFS path validation — block placement if no valid exit exists
- [x] Show path preview overlay (highlighted tiles + path length count) on tower hold
- [x] Handle grid scroll (drag vertically) for tall mazes

### 🏹 Tower System (Core)
- [x] Create `TowerData` ScriptableObject schema (cost, damage, range, attack speed, type)
- [x] Implement 3 prototype towers: Arrow Tower, Frost Spire, Cannon
- [x] Tower auto-targeting logic (nearest enemy in range)
- [x] Tower auto-fire and projectile system
- [ ] Tower placement menu (tap empty cell → show available towers)
- [ ] Tower sell mechanic (50% cost refund)

### 👾 Enemy System (Core)
- [x] Create `EnemyData` ScriptableObject schema (HP, speed, armor, reward gold)
- [x] Implement 2 prototype enemies: Scout, Warrior
- [x] Enemy spawner at top of grid
- [x] Enemy pathfinding using Flow Field vectors
- [x] Enemy death → gold reward → kill count increment
- [x] Enemy escape → life deduction logic

### 🌊 Wave System (Core)
- [x] Create `WaveData` ScriptableObject schema (enemy list, count, spawn interval)
- [x] Implement 5 prototype waves with escalating difficulty
- [x] Build phase → Wave phase → Reward phase state machine
- [x] Basic HUD: lives counter, gold counter, wave counter

### ✅ Prototype Exit Criteria
- [ ] Player can build a maze that delays enemies meaningfully
- [ ] Pathfinding validates correctly on every tower placement
- [ ] 5 waves run from start to finish without crashes
- [ ] Frame rate stable at 60 FPS on test device

---

## M2 — Alpha
> **Goal:** Full playable game loop. All 30 waves, 4 classes, card system, complete tower roster, proper mobile touch UX.

### 🃏 Card System
- [ ] Create `CardData` ScriptableObject schema (type, effect, rarity, description)
- [ ] Implement card draw pool — weighted random draw per wave
- [ ] Implement 7 card types: Tower Unlock, Tower Modifier, Global Modifier, Maze Card, Relic, Curse Card, Boss Skip
- [ ] Build card draw UI — 3-card modal overlay after each wave
- [ ] Implement free re-roll (1 per draw)
- [ ] Track draw history within run to prevent duplicate cards
- [ ] Apply card effects to run state immediately on selection

### 🏹 Tower System (Full)
- [ ] Implement remaining 2 base towers: Shock Tower, Beacon
- [ ] Implement 8 card-unlocked towers: Inferno Obelisk, Phase Cannon, Mirror Shard, Void Trap, Echo Tower, Root Maw, Bounty Pylon, Hypercane
- [ ] Tower upgrade system — Tier 1, Tier 2 fork (Path A / Path B), Tier 3
- [ ] Upgrade UI — tap placed tower → show upgrade options with cost and description
- [ ] Tower synergy detection — check adjacent cells on placement, apply synergy bonus
- [ ] Synergy glow visual indicator on valid adjacent pairs
- [ ] Tower aura / support radius visualizer (Beacon)

### 🏰 Builder Classes (4 of 6)
- [ ] Implement class selection screen at run start
- [ ] Implement Cryomancer class — starting towers, card pool bias, starting bonus
- [ ] Implement Pyromancer class — starting towers, card pool bias, starting bonus
- [ ] Implement Technomancer class — starting towers, card pool bias, starting bonus
- [ ] Implement Naturalist class — starting towers, card pool bias, starting bonus
- [ ] Class-specific card pool weighting system

### 👾 Enemy System (Full)
- [ ] Implement remaining 7 enemy types: Brute, Shade (invisibility), Golem (slow immunity), Splitter, Healer, Flyer, Juggernaut (boss)
- [ ] Enemy armor system — physical / magical / heavy resistance modifiers
- [ ] Flyer pathfinding — ignore grid walls, straight line to exit
- [ ] Healer aura — periodic HP regen to nearby enemies
- [ ] Shade invisibility cycle — toggle renderer and collider on timer
- [ ] Splitter death → spawn 2 Scouts at death position
- [ ] Juggernaut per-wave unique mechanic system (data-driven)

### 🌊 Wave System (Full 30 Waves)
- [ ] Author all 30 wave data assets (enemy composition, timing, boss waves at W5/10/15/20/25/30)
- [ ] Wave mutation system — random mutation drawn every 3 waves, revealed 1 wave early
- [ ] Implement 8 mutations: Armored, Swarm, Invisible Passage, Air Raid, Haunted, Gold Rush, Headless, Temporal
- [ ] Mutation announcement UI — banner displayed with icon and description before the affected wave
- [ ] Endless Mode — post-wave-30 loop with +15% HP/damage per wave

### 🎮 Mobile Touch Controls (Full)
- [ ] Tap-to-place with placement confirmation step
- [ ] Double-tap tower → quick-sell
- [ ] Tap placed tower → info popup (stats, synergies, upgrade, sell)
- [ ] Drag scroll on grid — smooth vertical scroll with momentum
- [ ] Safe area insets for notch and dynamic island
- [ ] Touch target sizing audit — all interactive elements ≥ 72×72 dp

### 💾 Save System
- [ ] Run state serialization to JSON (mid-run save on app backgrounding)
- [ ] Run state restoration on app relaunch
- [ ] Meta-progression data saved to persistent storage
- [ ] Save file migration versioning system

### ✅ Alpha Exit Criteria
- [ ] Full 30-wave run completable from start to end
- [ ] Card system functional with all 7 card types
- [ ] 4 Builder Classes selectable and functional
- [ ] All enemy types spawning and behaving correctly
- [ ] No progression-blocking bugs

---

## M3 — Beta
> **Goal:** Feature-complete game. All 6 classes, full roguelite loop, meta-progression, audio, visual polish, performance targets met.

### 🏰 Builder Classes (Remaining 2)
- [ ] Implement Necromancer class — shade spawn mechanic, card pool, starting bonus
- [ ] Implement Arcanist class — mana system, card pool, starting bonus
- [ ] Class unlock via Chronicle Forge tree (Necromancer and Arcanist locked by default)

### 📈 Meta-Progression — The Forge
- [ ] Implement Essence currency — earned at run end based on score formula
- [ ] Build The Forge hub screen (between-run persistent upgrade UI)
- [ ] Implement Arsenal tree — 8 nodes unlocking new towers to card pool
- [ ] Implement Mastery tree — 10 nodes (start gold, start lives, card draw bonuses, etc.)
- [ ] Implement Chronicle tree — 6 nodes (classes, relic slots, curse immunity)
- [ ] Forge node dependency graph — unlock order enforcement
- [ ] Forge node preview — show what the node unlocks before spending

### 🏆 Scoring & Leaderboards
- [ ] Score calculation system (waves × 100 + kills × 10 + lives × 500 × difficulty)
- [ ] End-of-run score screen with breakdown
- [ ] Local leaderboard (top 10 stored on device)
- [ ] Game Center (iOS) and Google Play Games (Android) integration
- [ ] Leaderboard filtered by class and difficulty

### 🔓 Milestone Unlocks
- [ ] Wave 10 clear → unlock Hard difficulty
- [ ] Wave 20 clear → unlock Brutal difficulty
- [ ] Wave 30 clear → unlock Endless Mode
- [ ] All 6 classes cleared → unlock Arcanist Curse Relic set
- [ ] Wave 40 Endless → unlock boss skin "Corrupted Duke Frostmaul"
- [ ] All Relics collected → unlock "Collector" title

### 🎨 Visual Polish
- [ ] Final grid tile art — stone/ice aesthetic per zone
- [ ] All 13 tower final sprites + idle animations
- [ ] All 9 enemy final sprites + walk animations
- [ ] Boss entrance animation (Juggernaut)
- [ ] Tower upgrade visual transformation (per tier)
- [ ] Projectile and hit VFX for all damage types (physical, ice, lightning, fire, arcane)
- [ ] AoE splash VFX (Cannon, Hypercane, Nova Pulse)
- [ ] Freeze burst VFX (Frost Spire death trigger)
- [ ] UI background art — Forge hub, class selection, main menu

### 🎵 Audio Implementation
- [ ] 6 per-class background music tracks — adaptive intensity layering
- [ ] Tower attack SFX × 13
- [ ] Tower placement SFX (generic + per-type variant)
- [ ] Tower upgrade SFX × 3 tiers
- [ ] Enemy footstep SFX × 9 types
- [ ] Enemy death SFX × 9 types
- [ ] Boss entrance fanfare
- [ ] UI SFX — card flip, wave start, life lost, victory, defeat
- [ ] Master volume / music / SFX sliders in settings
- [ ] Haptic feedback — configurable on/off, 3 distinct patterns

### ⚙️ Settings & Accessibility
- [ ] Settings screen: audio, haptics, font size, high contrast, colorblind mode
- [ ] High contrast mode — all UI elements increase contrast
- [ ] Colorblind mode — enemy silhouettes guaranteed distinct without color reliance
- [ ] Dynamic font scaling — respect device text size setting

### 📱 Performance Optimization
- [ ] Profile and hit 60 FPS on iPhone 12 / Samsung Galaxy A54
- [ ] Profile and hit 30 FPS on iPhone 8 / 3GB RAM Android
- [ ] Sprite atlasing — batch draw calls for grid tiles and towers
- [ ] Object pooling — enemy, projectile, VFX pools (zero runtime allocation)
- [ ] Flow Field caching — only recompute on maze change, not every frame
- [ ] APK / IPA size under 150 MB — asset compression audit

### ✅ Beta Exit Criteria
- [ ] All 6 classes functional
- [ ] Meta-progression fully playable (Forge, scoring, leaderboards)
- [ ] Performance targets met on all test devices
- [ ] Audio fully implemented
- [ ] No crashes in 10 consecutive full runs (internal QA)

---

## M4 — Soft Launch
> **Goal:** Real-world validation in 1 regional market (e.g. Canada or Australia). Fix issues, balance the game, stabilize.

### 🚀 Store Preparation
- [ ] App Store Connect setup — app ID, certificates, provisioning profiles
- [ ] Google Play Console setup — app ID, signing keys
- [ ] App icon (1024×1024) — all required sizes generated
- [ ] Screenshots × 6 per platform in portrait format
- [ ] App Preview video (15 sec) for App Store
- [ ] Store listing copywriting — title, subtitle, description, keywords (ASO)
- [ ] Privacy policy page published
- [ ] Age rating questionnaire completed

### 💰 Monetization Integration
- [ ] Apple StoreKit 2 integration — one-time IAPs + consumables
- [ ] Google Play Billing Library integration
- [ ] IAP catalog creation: Premium ($4.99), Skin Packs, Seasonal Pack, Essence Boost
- [ ] IAP purchase flow — tap, confirm, restore purchases
- [ ] Receipt validation (server-side or StoreKit verification)
- [ ] Paywall placements — Premium prompt after Wave 10 (first time)

### 📊 Analytics & Monitoring
- [ ] Firebase Analytics integration — event tracking (run start, wave cleared, card picked, run end)
- [ ] Custom funnels: Onboarding → First Run → First Win → First IAP
- [ ] Crash reporting — Firebase Crashlytics
- [ ] Session length and retention tracking (D1, D7, D30)
- [ ] A/B test framework setup for future paywall experiments

### 🐛 QA & Balancing
- [ ] Full regression test pass — all features, all classes, all difficulties
- [ ] Edge case testing — grid full, 0 lives, card pool exhausted, Endless Mode
- [ ] Balance pass 1 — enemy HP/speed, tower DPS, gold economy, card weights
- [ ] Balance pass 2 — act on Soft Launch retention data (after 2 weeks live)
- [ ] Balance pass 3 — act on Soft Launch wave completion rate data
- [ ] Localization — English final pass (grammar, tone, consistency)

### ✅ Soft Launch Exit Criteria
- [ ] D1 retention ≥ 40%
- [ ] D7 retention ≥ 15%
- [ ] Average session length ≥ 8 minutes
- [ ] Crash-free rate ≥ 99.5%
- [ ] No critical IAP bugs reported

---

## M5 — Global Launch
> **Goal:** Worldwide store release. Marketing push. Community seeding.

### 🌍 Global Release
- [ ] Localization — French, German, Spanish, Portuguese, Japanese, Korean, Chinese (Simplified)
- [ ] RTL layout support check (Arabic / Hebrew — optional stretch goal)
- [ ] Global App Store and Google Play rollout (staged rollout — 10% → 50% → 100%)
- [ ] Feature request submission to Apple (App Store featuring)
- [ ] Google Play editorial submission

### 📣 Marketing & Community
- [ ] Launch trailer (30–60 sec) — gameplay focused, vertical format for mobile ads
- [ ] Short-form content — 3× TikTok/Reels gameplay clips
- [ ] Press kit — screenshots, description, key art, contact
- [ ] Outreach to 10 mobile game content creators
- [ ] Reddit presence — r/AndroidGaming, r/iosgaming, r/roguelikes launch posts
- [ ] Discord server launch — announcement channel, feedback channel, leaderboard channel

### 🔔 Push Notifications
- [ ] Daily challenge notification ("New challenge run available")
- [ ] Seasonal event notification setup
- [ ] Opt-in prompt timing — after first successful run (not on first launch)

### ✅ Global Launch Exit Criteria
- [ ] Available in all target regions without store restrictions
- [ ] All 7 localizations live and reviewed
- [ ] No P0/P1 bugs in first 48 hours
- [ ] Launch trailer published across all channels

---

## M6 — Post-Launch Season 1
> **Goal:** Keep players engaged. New content, seasonal challenge, cosmetics, and data-driven improvements.

### 🗓️ Season 1 Content
- [ ] Design and implement 2 new towers (unique mechanics, card pool integration)
- [ ] Design and implement 1 new boss Juggernaut type (unique wave mechanic)
- [ ] Design and implement 1 new wave mutation
- [ ] Design 1 new Builder Class skin (cosmetic only)
- [ ] Design 3 new Tower Skin Pack variants
- [ ] Design 1 exclusive Season 1 Relic (earnable via seasonal challenges only)

### 🏆 Seasonal Challenge System
- [ ] Seasonal challenge run — fixed seed, specific modifiers, special ruleset
- [ ] Season challenge leaderboard (separate from main leaderboard)
- [ ] Season challenge UI — dedicated entry point on main menu
- [ ] Season timer — countdown to end of season
- [ ] Season reward distribution — exclusive cosmetics at end of season

### 🔄 Live Ops Infrastructure
- [ ] Remote config system (Firebase Remote Config) — balance values adjustable without update
- [ ] Daily challenge system — generated seed + ruleset, rotates every 24 hours
- [ ] Weekly challenge system — harder, rotates every 7 days
- [ ] Challenge reward pipeline → Essence grants

### 🐛 Post-Launch Fixes & Polish
- [ ] Act on D30 retention data — identify and fix drop-off points
- [ ] Balance pass 4 — act on Endless Mode difficulty data
- [ ] Performance patch — any regressions on new devices (iPhone 16 series etc.)
- [ ] Community bug fixes — top 5 reported issues from Discord

### 📊 Season 1 Success Metrics
- [ ] D30 retention ≥ 10%
- [ ] Season challenge participation ≥ 20% of active players
- [ ] Season 1 IAP revenue — seasonal pack conversion ≥ 3% of active players
- [ ] Average run count per player per week ≥ 3

---

## Appendix — Technical Debt & Ongoing Tasks

> These tasks run continuously across all milestones.

### 🔧 Engineering Health
- [ ] Unit tests for pathfinding (BFS validation, Flow Field correctness)
- [ ] Unit tests for card pool weighting and draw logic
- [ ] Unit tests for score calculation
- [ ] CI/CD pipeline — automated build on push to `main` (Unity Cloud Build or GitHub Actions)
- [ ] Automated smoke test — launch game, complete wave 1, no crash

### 📋 Documentation
- [ ] Architecture overview doc (systems, data flow, ScriptableObject schemas)
- [ ] Onboarding guide for new contributors
- [ ] Balance spreadsheet — all tower stats, enemy stats, card weights
- [ ] Changelog maintained per build

---

*Last updated: February 2026 — Frostmaul GDD v1.0*
