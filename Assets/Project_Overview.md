# MirrorMage Project Overview

This document provides a technical overview of the MirrorMage project, a 2D bullet-reflection action game built with Unity 6 and URP.

## 1. Project Description
MirrorMage is a top-down survival action game where the player controls a mage who cannot attack directly. Instead, the player must use a magical barrier to reflect enemy projectiles back at them. The core experience revolves around precise timing, positioning, and managing ability cooldowns while surviving waves of enemies.

**Core Pillars:**
- **Reflective Combat:** Success is based on turning the environment and enemy attacks against them.
- **High-Juice Feedback:** Heavy use of visual effects, screen shakes, and color-pulsing for combat impact.
- **Progression Loop:** Experience points (XP) gained from defeated enemies allow for mid-game upgrades.

## 2. Gameplay Flow / User Loop
1.  **Boot & Initialization:** The game starts in the `Main.unity` scene. The `EnemySpawner` begins generating enemies at the edge of the screen.
2.  **Survival Loop:**
    - Player moves towards the mouse cursor using `PlayerController`.
    - Enemies spawn and move toward the player, firing projectiles.
    - Player activates the `Barrier` (Left Click) to reflect incoming projectiles.
3.  **Progression:**
    - Defeating enemies grants XP.
    - Upon reaching XP thresholds, `PlayerController` triggers a level-up.
    - `LevelUpUI` pauses the game and presents three upgrade options (Move Speed, Charge Speed, Barrier Strength).
4.  **Death/Shutdown:** If health reaches zero, the player is defeated (current implementation logs death to console).

## 3. Architecture
The project follows a component-based architecture where individual systems handle specific logic (movement, combat, UI) and communicate via direct references or standard Unity events.

- **Central Hub:** `PlayerController.cs` acts as the primary coordinator for player state, health, and XP.
- **State Management:** The game uses `Time.timeScale = 0` for pausing during UI interactions (Level Up).
- **Visual Decoupling:** Visual effects (flashes, shakes) are often handled within the specific component (e.g., `Barrier.cs` or `PlayerController.cs` coroutines) to keep logic and juice close together.

`Location: Assets/Scripts/`

## 4. Game Systems & Domain Concepts

### Barrier & Reflection System
- `Barrier`: Handles the activation/deactivation logic, visual pulsing, and the expansion effect when the shield is first raised.
- `Projectile`: A mobile entity that moves in a direction. It contains a `Reflect` method which doubles its speed and flags it as `isReflected` to enable enemy damage.
- **Extension:** New projectile types can be added by inheriting from or modifying `Projectile.cs`.
- **Pattern:** Geometric reflection using `Vector2.Reflect` based on the normal between the barrier center and the collision point.

`Location: Assets/Scripts/`

### XP & Level Up System
- `PlayerController`: Manages the XP pool and triggers the `LevelUpUI`.
- `LevelUpUI`: A UITK-based system that manages the upgrade selection screen. It uses a staggered entry animation for buttons.
- **Upgrades:** The system currently supports three hardcoded upgrades: `MoveSpeed`, `ChargeSpeed` (Cooldown reduction), and `BarrierStrength` (Duration and Scale).
- **Extension:** To add new upgrades, add a new `SetupButton` call in `LevelUpUI.Awake` and a corresponding selection handler.

`Location: Assets/Scripts/`

### Enemy & Spawning System
- `EnemySpawner`: Periodically instantiates enemy prefabs at a distance from the player.
- `Enemy`: Simple AI that tracks the player's position and handles its own destruction.
- **Extension:** New enemy types can be added to the `EnemySpawner` prefab list.

`Location: Assets/Scripts/`

## 5. Scene Overview
The project currently utilizes a single-scene structure:
- **Main.unity:** Contains the game world, URP Global Volume, Player, Enemy Spawner, and UI overlays.
- **UI Structure:** UI is divided into the `PlayerHUD` (always visible) and `LevelUpUI` (enabled on level up).

## 6. UI System
The project uses **UI Toolkit (UITK)** for its user interface.

- `PlayerHUD`: Displays real-time stats like Health and XP.
- `LevelUpUI`: A full-screen overlay using `.uxml` and `.uss`.
- **Binding Logic:** Components use `rootVisualElement.Q<T>()` to query elements and manually update their styles or values (e.g., `ProgressBar` equivalents for cooldowns).
- **Visuals:** Animations are handled via USS class toggles (e.g., `option-card--hidden` to `option-card--selected`) and C# Coroutines for color interpolation.

`Location: Assets/UI/`

## 7. Asset & Data Model
- **Prefabs:** Key entities like the Player, Enemies, and Projectiles are prefabbed for easy spawning and modification.
- **Sprites:** Assets are organized by character/effect (e.g., `Assets/Sprites/Enemy_Bat_Fly.png`).
- **Materials:** Uses a `BackgroundPulse.mat` with a custom `ColorPulse.shader` for environmental atmosphere.
- **Fonts:** Uses the "Jersey 10" font asset via TextMeshPro/UITK.

## 8. Notes, Caveats & Gotchas
- **Time Scaling:** The Level Up system uses `Time.timeScale = 0`. Any logic that needs to run while the game is paused (like UI animations) must use `Time.unscaledDeltaTime` or `WaitForSecondsRealtime`.
- **Collision Layers:** Reflection logic relies on `OnTriggerEnter2D` and specific Tags (`Enemy`, `Projectile`). Ensure new prefabs have the correct tags assigned.
- **Visuals Child:** The `PlayerController` expects a child GameObject named "Visuals" to handle sprite flipping and hit-flash shakes without affecting the root transform's physics.