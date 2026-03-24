# Project Overview: MirrorMage

## 1. Project Description
**MirrorMage** is a high-paced 2D top-down survival action game where the player assumes the role of a mage who lacks offensive spells. Instead, the player must survive by using a magical **Mirror Barrier** to reflect incoming enemy projectiles back at their source. The game emphasizes positioning, timing, and defensive-to-offensive transitions. It is built using Unity 6 (6000.3.11f1) with the Universal Render Pipeline (URP) and utilizes the New Input System for modern control schemes.

## 2. Gameplay Flow / User Loop
*   **Startup**: The game begins in the `Main` scene. The player character is initialized at the center, and the `EnemySpawner` begins generating enemies at a set distance.
*   **Survival Loop**: 
    *   **Movement**: The player moves towards the mouse cursor using `PlayerController`.
    *   **Defense**: Enemies track the player and fire spread-shot projectiles. The player must activate the `Barrier` (Left-Click) to reflect these bullets.
    *   **Offense**: Reflected projectiles gain speed and change color to yellow. Only reflected projectiles can damage and kill enemies.
*   **Health & Cooldown**: The player manages a health bar and a barrier cooldown. Activating the barrier consumes its duration, followed by a recharge period tracked by the `CooldownBar` and `PlayerHUD`.
*   **Game Over**: If player health reaches zero via projectile or enemy contact, the game loop terminates (logic found in `PlayerController.TakeDamage`).

## 3. Architecture
The project follows a component-based architecture where behavior is localized within specific scripts that communicate primarily through Unity's Physics2D triggers and direct references.

*   **Input Management**: Uses the **New Input System** (Input System package 1.19.0). Mouse position and button states are polled directly in `PlayerController`.
*   **Combat Logic**: Driven by the `Projectile` class, which handles its own movement and collision logic. It transitions state from "Enemy" to "Reflected" upon hitting a `Barrier`.
*   **Visual Feedback**: Heavy use of Coroutines for feedback loops (flashing, shaking, fading) instead of complex state machines.
*   **UI Integration**: Uses **UIToolkit** for the HUD (`PlayerHUD`) and standard **SpriteRenderers** for in-world elements like the `CooldownBar`.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Player System
Handles movement, health management, and barrier triggering.
*   `PlayerController`: Primary entry point; calculates movement towards mouse, handles invincibility frames, and manages the barrier state machine.
*   `Barrier`: Attached to a child object of the Player. Handles the collision logic for reflecting projectiles and visual "magical" pulsing effects.

**Extension**: To add new player abilities (e.g., a dash), add a new input check in `PlayerController` and a corresponding cooldown timer.

`Location: Assets/Scripts/PlayerController.cs`, `Assets/Scripts/Barrier.cs`

### Enemy & Spawning System
Controls enemy AI and population.
*   `Enemy`: Simple tracking AI that moves toward the player and fires `Projectile` prefabs in a spread pattern.
*   `EnemySpawner`: Spawns enemies at a radius around the map origin on a fixed timer.

**Extension**: Create new enemy types by inheriting from or modifying `Enemy.cs` to change the `Shoot()` pattern (e.g., circular bursts or homing shots).

`Location: Assets/Scripts/Enemy.cs`, `Assets/Scripts/EnemySpawner.cs`

### Projectile & Reflection System
The core mechanic of the game.
*   `Projectile`: Moves in a set direction. It contains a `Reflect()` method that uses `Vector2.Reflect` to calculate bounce trajectories based on the barrier's surface normal.
*   `SelfDestruct`: Utility script used for cleaning up death effects and temporary particles.

**Extension**: New projectile behaviors (like curved paths) can be added by modifying the `Update()` loop in `Projectile.cs`.

`Location: Assets/Scripts/Projectile.cs`

## 5. Scene Overview
*   **Main**: The primary gameplay scene. It contains:
    *   **Player Prefab**: Setup with `Rigidbody2D` and `CircleCollider2D`.
    *   **EnemySpawner**: A singleton-like object that manages enemy waves.
    *   **Global Volume**: Manages URP Post-Processing (Bloom, Lens Distortion, Film Grain) to give the game its distinct "neon" look.
    *   **UIDocument**: Hosts the `Main.uxml` for the screen-space HUD.

`Location: Assets/Main.unity`

## 6. UI System
The project uses a hybrid UI approach:
*   **Screen Space (UIToolkit)**: `Main.uxml` and `PlayerHUD.cs` manage the health bar. It uses a `VisualElement` with percentage-based width scaling.
*   **World Space (Sprites)**: `CooldownBar.cs` manages a sprite-based progress bar parented to the player, allowing players to track barrier readiness without looking away from the action.

**To modify UI**:
*   Edit `Assets/UI/Main.uxml` using the UI Builder.
*   Styles are defined in `Assets/UI/Main.uss`.

`Location: Assets/UI`

## 7. Asset & Data Model
*   **Prefabs**: 
    *   `Enemy.prefab`: Contains the skeleton visuals and `Enemy` logic.
    *   `Projectile.prefab`: Shared by both enemies (red) and player reflections (yellow).
    *   `Flame_Death_Effect.prefab`: Spawned by `Enemy.Die()`.
*   **Animations**: Uses Unity's `Animator` system. Controllers like `PlayerController.controller` manage transitions between idle and walk states based on movement speed.
*   **URP Settings**: `2D Renderer.asset` is configured for 2D lighting and post-processing, utilizing the `Kino` post-processing extensions for a retro aesthetic.

`Location: Assets/Prefabs`, `Assets/URP`

## 8. Notes, Caveats & Gotchas
*   **Reflection Logic**: The `Barrier` uses `OnTriggerEnter2D` to call `Reflect()` on projectiles. If the barrier is deactivated mid-frame, projectiles might pass through.
*   **Visuals Mapping**: `PlayerController` expects a child GameObject named "Visuals" to apply animations and sprite flipping. If this hierarchy changes, `EnsureVisuals()` will fallback to the root, which may break specific animation offsets.
*   **Physics Layers**: Ensure projectiles are on a layer that can collide with the Player (for damage) and the Barrier (for reflection), but reflected projectiles must be checked against the Enemy layer.
*   **Coordinate Space**: Reflection is calculated using the vector from the barrier center to the projectile. For more precise "angled" reflections, the barrier's collider shape is critical.