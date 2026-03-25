# MirrorMage Project Overview

## 1. Project Description
MirrorMage is a 2D top-down "Survivor-like" action game where the player controls a mage who survives by reflecting enemy projectiles back at them. Instead of direct attacks, the core mechanic revolves around a timed "Barrier" that parries incoming bullets, turning them into high-speed lethal counter-attacks. The project is designed for a frantic, high-intensity experience featuring neon-like 2D visuals and a progression-based upgrade loop.

## 2. Gameplay Flow / User Loop
1.  **Boot & Initialization**: The game starts in the `Main` scene. The `PlayerController` and `EnemySpawner` initialize, and the `PlayerHUD` binds to the player's stats.
2.  **Engagement**: The player moves using the mouse (automatic follow) and activates the barrier with the Left Mouse Button to parry incoming red bullets.
3.  **Reflect & Destroy**: Parried bullets become yellow reflected projectiles that seek out enemies. Destroyed enemies grant XP.
4.  **Level Up**: Upon reaching an XP threshold, the game pauses (`Time.timeScale = 0`), and the `LevelUpUI` (UITK) appears, allowing the player to choose between three upgrades (Move Speed, Charge Speed, or Barrier Strength).
5.  **Scaling**: As the player levels up, the `EnemySpawner` continues to spawn enemies, and the player's barrier and stats evolve to match the increasing difficulty.

## 3. Architecture
The project follows a decoupled, component-based architecture where systems communicate primarily through direct references or Unity's `OnTriggerEnter2D` events.

### Game Management & Entry Point
The project currently uses a single-scene architecture where high-level managers like `EnemySpawner` and `LevelUpUI` exist as persistent or scene-resident objects.
*   `PlayerController`: Acts as the central hub for player state (Health, XP, Level) and input handling.
*   `EnemySpawner`: Manages the lifecycle of enemies, calculating off-screen spawn points based on camera bounds.

`Location: Assets/Scripts/`

## 4. Game Systems & Domain Concepts

### Combat & Reflection System
The defining system where projectiles change ownership and behavior upon collision with a specific trigger.
*   `Projectile`: Handles linear movement and collision logic. It has an `isReflected` state that determines if it damages the player or enemies.
*   `Barrier`: A timed shield that calculates the reflection normal based on the projectile's impact position relative to the player.
*   `Reflect()`: A method in `Projectile` that flips the `isReflected` flag, doubles the speed, and reverses direction using `Vector2.Reflect`.

`Location: Assets/Scripts/`

### Enemy System
A modular enemy behavior system that handles movement, periodic firing, and death sequences.
*   `Enemy`: Manages AI (follow player), weapon firing (spread shots), and a multi-stage `DeathRoutine` (shake, flash, then spawn effect).
*   `SelfDestruct`: A utility component used on death effects (like `Flame_Death_Effect`) to clean up the hierarchy after an animation completes.

`Location: Assets/Scripts/`

### Progression & XP System
A classic RPG-style level-up system integrated with the UI.
*   `PlayerController`: Stores XP, Level, and thresholds. It triggers the `LevelUpUI` when `currentXP >= xpToNextLevel`.
*   `LevelUpUI`: Manages the `Time.timeScale` and upgrade selection logic using a UITK-based modal.

`Location: Assets/Scripts/`

## 5. Scene Overview
The project currently utilizes a single active scene:
*   `Main`: Contains the `Player`, `EnemySpawner`, `Main Camera` (with URP Post-Processing), and the `UIDocument` for the HUD and Level-Up screens.
*   **Post-Processing**: Uses `KinoEight` for a stylized 8-bit/limited-palette color effect and a custom `ColorPulse` shader on the background.

## 6. UI System
The project uses **Unity UI Toolkit (UITK)** for all interface elements.
*   `PlayerHUD`: A persistent overlay that queries `VisualElement` widths (`healthBarFill`, `xpBarFill`) to reflect player stats every `Update`.
*   `LevelUpUI`: A modal system using `.uxml` and `.uss`. It uses USS classes (e.g., `option-card--hidden`, `option-card--selected`) to drive high-performance UI animations without the standard Animator component.
*   `CooldownBar`: A separate World-Space Sprite-based UI component for immediate visual feedback on the barrier's availability.

`Location: Assets/UI/`

## 7. Asset & Data Model
*   **Prefabs**: Core entities (Player, Enemy, Projectile) are prefabs. The `Enemy` prefab is configured with a `projectilePrefab` and a `deathEffectPrefab`.
*   **Animations**: Uses the standard `Animator` system for character walking and death effects.
*   **URP 2D**: The project is built on the Universal Render Pipeline with a 2D Renderer, utilizing `Light2D` for ambient effects and `Global Volume` for post-processing.
*   **Naming Convention**: Scripts use PascalCase, while private variables follow `_camelCase` with an underscore prefix.

## 8. Notes, Caveats & Gotchas
*   **Reflection Logic**: The reflection is purely geometric (`Vector2.Reflect`). If the barrier is too small or the projectile is too fast, collision detection might skip frames; ensure "Continuous" collision is used on fast projectiles.
*   **Time Scaling**: Leveling up sets `Time.timeScale = 0`. UI animations in `LevelUpUI` must use `unscaledTime` or `WaitForSecondsRealtime` to function correctly while the game is paused.
*   **Input**: The project uses the **New Input System** (Package: `com.unity.inputsystem`). Mouse positions are read via `Mouse.current.position`.
*   **Sprite Direction**: The `PlayerController` and `Enemy` scripts manually flip the `SpriteRenderer.flipX` based on the horizontal movement vector rather than using separate animation clips for left/right.