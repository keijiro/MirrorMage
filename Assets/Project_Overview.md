# MirrorMage Project Overview

## 1. Project Description
MirrorMage is a 2D top-down "survivor-like" action game with a unique defensive twist. Instead of direct attacks, the player controls a mage who survives by reflecting enemy projectiles using a timed magical barrier. The project features a distinct retro aesthetic achieved through a custom 8-color post-processing palette and pixel-art assets.

**Core Pillars:**
*   **Timed Defense:** Success depends on timing the barrier to reflect bullets back at enemies.
*   **Progression:** A standard XP and Level-Up loop where defeating enemies increases power and heals the player.
*   **Retro Aesthetic:** High-contrast 8-color visuals with pulsating effects.

## 2. Gameplay Flow / User Loop
1.  **Boot/Initialization**: The `Main` scene loads, initializing the `Player`, `EnemySpawner`, and URP-based visual stack.
2.  **Survival Loop**: 
    *   The player moves towards the mouse cursor.
    *   `EnemySpawner` continuously spawns enemies that chase the player and fire projectiles.
    *   The player uses Left Click to activate a `Barrier`.
3.  **Conflict & Resolution**:
    *   Projectiles hitting the active `Barrier` are reflected, speed up, and can kill enemies.
    *   Projectiles hitting the `Player` without a barrier deal damage.
    *   Killed enemies drop XP, which contributes to leveling up via `PlayerController.GainXP`.
4.  **State Transitions**:
    *   **Level Up**: Heals the player and increases the XP requirement for the next level.
    *   **Death**: (Placeholder) Logic exists in `PlayerController` for when health reaches zero.

## 3. Architecture
The project follows a component-based architecture where individual MonoBehaviours handle specific domain logic, coordinated by a central `PlayerController`.

*   **Central Coordinator**: `PlayerController` manages player state (health, XP, movement) and serves as the bridge for UI and sub-systems.
*   **Input**: Uses the **Unity Input System** (New). Movement is derived from `Mouse.current.position` and barrier activation from `Mouse.current.leftButton`.
*   **Visuals**: A custom URP pipeline using the `EightColorFeature` (KinoEight) post-processing effect to enforce the 8-color palette.
*   **Interaction**: Physics-based triggers (`OnTriggerEnter2D`) handle projectile reflection and damage.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Combat & Defense System
Handles the interaction between enemies, projectiles, and the player's defensive barrier.
*   `Barrier`: Manages the lifecycle of the shield (activation, expansion effects, and fading). It uses `Vector2.Reflect` to redirect projectiles.
*   `Projectile`: A moving trigger that tracks whether it has been "reflected" to determine its damage target (Player vs Enemy).
*   `Enemy`: Simple AI that chases the player and fires projectiles using a spread-shot pattern.

`Location: Assets/Scripts`

### Progression System
Manages the player's growth and survival stats.
*   `PlayerController`: Contains the XP logic. XP is granted by `Enemy.Die()`.
*   `LevelUp`: Triggered when `currentXP` exceeds `xpToNextLevel`, scaling the difficulty (XP requirement) and rewarding the player (Full Heal).

`Location: Assets/Scripts`

### Visual Feedback System
Provides juice and game-state information through animations and procedural effects.
*   `CooldownBar`: A world-space UI element tracking the `Barrier`'s availability.
*   `SelfDestruct`: Utility for fire-and-forget visual effects (e.g., `Flame_Death_Effect`).
*   `ColorPulse.shader`: A custom shader used for background environmental effects.

`Location: Assets/Scripts, Assets/Shaders`

## 5. Scene Overview
*   **Main**: The core gameplay scene.
    *   **Player**: Contains the visual hierarchy, `Barrier` object, and `CooldownBar`.
    *   **EnemySpawner**: Manages the rate and prefab used for spawning waves.
    *   **Global Volume**: Hosts the URP `Volume` profile with `EightColorController`.
    *   **UI**: A `UIDocument` providing the HUD.

`Location: Assets/Main.unity`

## 6. UI System
The project utilizes **UI Toolkit (UITK)** for the main HUD.
*   **Framework**: `UIDocument` and `VisualTreeAsset` (`Main.uxml`).
*   **HUD Logic**: `PlayerHUD.cs` queries the UXML for elements like `healthBarFill` and `xpBarFill`.
*   **Binding**: Updates are performed in `Update()` by calculating the percentage of health/XP and modifying the `style.width` (LengthUnit.Percent) of the fill elements.
*   **In-Game UI**: The `CooldownBar` is a world-space Sprite-based system rather than UITK, allowing it to follow the player's position smoothly.

`Location: Assets/UI, Assets/Scripts/PlayerHUD.cs`

## 7. Asset & Data Model
*   **Prefabs**:
    *   `Enemy.prefab`: Pre-configured with movement speed and projectile types.
    *   `Projectile.prefab`: Shared by both enemies and reflected player attacks.
*   **Animations**: `AnimatorControllers` manage simple frame-based sprite animations for walking and death.
*   **Rendering**: The **Universal Render Pipeline (URP)** is configured for 2D. The `2D Renderer` includes a custom `EightColorFeature` renderer feature.
*   **Shaders**: Uses standard Sprite shaders and a custom `ColorPulse` shader for environmental flair.

`Location: Assets/Prefabs, Assets/URP, Assets/Animations`

## 8. Notes, Caveats & Gotchas
*   **Sprite Flipping**: Flipping is handled in code via `_spriteRenderer.flipX` based on movement direction, rather than animation states.
*   **Collision Layers**: The game relies heavily on Tags (`Player`, `Enemy`, `Projectile`). Ensure any new assets are tagged correctly or triggers will fail.
*   **Visual Shake**: Both the Player and Enemy have hard-coded "shake and flash" routines in their scripts (`DamageRoutine` and `DeathRoutine`) which manipulate the `Visuals` transform directly.
*   **Projectiles**: Once a projectile is reflected, its speed is doubled and its color changes to yellow. It can only be reflected once (`if (isReflected) return`).
*   **Screen Bounds**: `Projectile.cs` has a simple distance-based cleanup from `Vector2.zero` (50 units) to prevent memory leaks.