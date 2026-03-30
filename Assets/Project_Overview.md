# MirrorMage Project Overview

## 1. Project Description
MirrorMage is a 2D top-down "survivor-like" action game where the player controls a mage who survives not by casting spells directly, but by reflecting enemy projectiles back at them. The core experience centers around timing a defensive barrier to deflect incoming fire, which then becomes a high-speed lethal projectile against enemies. It is designed for short, intense sessions featuring character progression through an XP-based level-up system.

**Core Pillars:**
- **Reflective Combat:** Defense is the best offense; timing the barrier is the primary skill.
- **Strategic Movement:** Maneuvering to bait enemy fire and positioning for optimal reflections.
- **Progression:** Scaling difficulty met with player upgrades (speed, cooldown, barrier size).

## 2. Gameplay Flow / User Loop
1.  **Boot & Initialization:** The game starts in the `Main` scene. The `EnemySpawner` begins tracking player XP to determine spawn rates.
2.  **Survival Loop:**
    *   **Movement:** Player moves via mouse position (following the cursor).
    *   **Combat:** Enemies spawn off-screen, approach the player, and fire projectiles.
    *   **Interaction:** Player activates the `Barrier` (Left Click). If a `Projectile` hits the active barrier, it is "reflected," changing its team, doubling its speed, and targeting enemies.
    *   **Death/Damage:** Projectiles that hit the player cause damage and temporary invincibility.
3.  **Progression Loop:**
    *   Defeated enemies grant XP.
    *   Reaching XP thresholds triggers a `LevelUp`.
    *   The game pauses (`Time.timeScale = 0`), and the `LevelUpUI` presents three upgrade options.
    *   Selecting an upgrade applies modifiers to `PlayerController` and resumes the game.
4.  **Failure:** If player health reaches zero, the game session ends (currently logs to console).

## 3. Architecture
The project follows a **Component-Based** architecture with a **Centralized Player State** pattern.
- **State Management:** `PlayerController` acts as the "Source of Truth" for player stats, health, and XP.
- **Event Handling:** While not using a formal Event Bus, systems interact via direct references or Tag-based collision detection (`OnTriggerEnter2D`).
- **Time Control:** The UI system manages game pausing by manipulating `Time.timeScale`, requiring UI animations to use `unscaledTime`.

`Location: Assets/Scripts/`

## 4. Game Systems & Domain Concepts

### Movement & Input System
Uses the **Unity Input System (New)** to track mouse position and button clicks. 
- `PlayerController`: Moves the player toward the mouse cursor using `Vector3.MoveTowards`.
- `Enemy`: Uses a coroutine-based state machine (`BehaviorRoutine`) to move toward or maintain distance from the player.

`Location: Assets/Scripts/`

### Combat & Reflection System
A "Tennis-style" reflection mechanic governed by physics triggers.
- `Barrier`: Attached to the player; handles the `OnTriggerEnter2D` logic for projectiles. It calculates the reflection normal based on the vector from the player to the impact point.
- `Projectile`: A simple transform-based mover. When `Reflect()` is called, it reverses direction based on a normal, increases speed, and changes its "team" (via the `isReflected` flag).
- `Enemy`: Contains a `Die()` method triggered when hit by a reflected projectile.

`Location: Assets/Scripts/`

### Enemy & Spawning System
- `EnemySpawner`: Manages the game's "Director" logic. It scales the maximum allowed enemies and spawn frequency based on the player's `totalXP`.
- `Enemy`: Self-contained AI. Different enemy types are supported by varying the serialized fields (move distance, bullet count, spread angle) on the prefab.

`Location: Assets/Scripts/`

### Level-Up System
- `PlayerController`: Tracks `currentXP` and `xpToNextLevel`. Incremental level-ups increase requirements by 25%.
- `LevelUpUI`: Triggered by `PlayerController`. It maps button clicks to specific upgrade methods (Move Speed, Charge Speed, Barrier Strength).

`Location: Assets/Scripts/`

## 5. Scene Overview
- **Main.unity:** The single active scene containing:
    - **Player Rig:** Includes the `PlayerController`, `Barrier` (as a child), and `CooldownBar` (World-space UI).
    - **EnemySpawner:** Global manager for enemy lifecycle.
    - **UI Layer:** `UIDocument` instances for the HUD and Level-Up overlays.
    - **Rendering:** URP 2D Renderer with a `Global Light 2D` and a `Kino.EightColorController` on the camera for a stylized 8-color palette look.

## 6. UI System
The project uses **UI Toolkit (UITK)** for primary overlays and **UGUI/World-Space Sprites** for in-game indicators.
- **UI Toolkit (`LevelUpUI`):** Managed via `.uxml` (structure) and `.uss` (styling). It uses USS Class transitions for animations (e.g., `option-card--hidden` to `option-card--selected`).
- **HUD (`PlayerHUD`):** Updates the XP and Health progress bars by querying the `PlayerController` state every frame.
- **World-Space UI (`CooldownBar`):** A custom sprite-based bar attached to the player to show barrier availability without requiring the player to look away from the action.

`Location: Assets/UI/`

## 7. Asset & Data Model
- **Prefabs:**
    - `Enemy.prefab`: The base ghoul/skeleton template.
    - `Enemy_Bat.prefab`: A variant with different movement/shooting stats.
    - `Projectile.prefab`: Shared by all enemies.
- **Scriptable Objects:** The project currently relies on Prefab-based configuration rather than standalone ScriptableObjects for enemy stats.
- **Visuals:** Uses a mix of 2D Sprites and a custom `ColorPulse.shader` for background effects. The `Kino.Eight` post-processing effect is used to enforce a specific color palette across all assets.

## 8. Notes, Caveats & Gotchas
- **Time Scale:** Because `LevelUpUI` sets `Time.timeScale = 0`, all UI animations and `LevelUpUI` coroutines must use `WaitForSecondsRealtime` and `Time.unscaledTime`.
- **Barrier Scaling:** Upgrading "Barrier Strength" increases the `localScale` of the barrier. Since reflection normals are calculated from the player center to the collision point, a larger scale significantly changes the reflection angles.
- **Sprite Flipping:** The `PlayerController` and `Enemy` scripts manually flip the `SpriteRenderer.flipX` based on movement delta. If adding a new visual rig, ensure the "Visuals" child object is correctly referenced.
- **Sorting Layers:** Ensure enemies and projectiles are on correct 2D sorting layers to prevent the player's `Shadow` or `Barrier` from being obscured.