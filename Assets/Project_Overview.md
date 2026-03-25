# MirrorMage Project Technical Overview

## 1. Project Description
**MirrorMage** is a 2D top-down action survival game where the player controls a mage who must survive waves of enemies by reflecting projectiles. Unlike traditional shooters, the core mechanic centers around a temporary **Mirror Barrier** that deflects incoming enemy fire, turning it back against the attackers with increased speed and power. The project targets a retro-aesthetic with stylized post-processing and mouse-driven movement.

## 2. Gameplay Flow / User Loop
1.  **Initialization**: The `Main` scene loads, initializing the `EnemySpawner` and the `PlayerController`.
2.  **Movement & Evasion**: The player moves by following the mouse cursor (`PlayerController.FollowMouse`). Enemies spawn at the edges and move toward the player.
3.  **Combat (Defense to Offense)**: Enemies periodically fire projectiles. The player must time the activation of the `Barrier` (Left Mouse Button) to intercept projectiles. 
4.  **Reflection**: Projectiles hitting an active barrier are "Reflected" (`Projectile.Reflect`), changing their tag and direction to target enemies.
5.  **Progression/Difficulty**: The player survives as long as possible while their health (`maxHealth`) remains above zero. Getting hit by unreflected projectiles or enemies triggers an invincibility flash and reduces health.
6.  **Game Over**: When health reaches zero, the player "dies" (logged in console), effectively ending the loop.

## 3. Architecture
The project follows a **Component-Based Architecture** with a central **Manager-less** flow, relying on direct references and tag-based collision detection.

*   **Input Handling**: Uses the **Unity Input System** (Package). `PlayerController` reads `Mouse.current` directly for position and button states.
*   **Collision Interaction**: Relies on `OnTriggerEnter2D` and Unity's Tag system (`Player`, `Enemy`, `Projectile`). 
*   **State Management**: 
    *   **Player State**: Managed within `PlayerController` via booleans (`_isBarrierActive`, `_isInvincible`) and timers.
    *   **Projectile State**: Managed in `Projectile` via the `isReflected` flag, which determines its damage targets.
*   **Post-Processing**: Integrated via URP and the `KinoEight` package to provide a restricted color palette (8-color) effect.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Movement System
Handles player and enemy translation.
*   `PlayerController`: Implements mouse-following logic with a dead-zone check and sprite flipping based on movement direction.
*   `Enemy`: Implements simple "Move Towards Player" logic.

`Location: Assets/Scripts`

### Reflective Combat System
The core mechanic governing projectile interaction.
*   `Barrier`: A child object of the Player that handles the `Reflect` logic. It uses a `CircleCollider2D` to detect projectiles and calculates reflection vectors based on the collision normal.
*   `Projectile`: A reusable prefab that moves in a set direction. It contains a `Reflect` method that doubles its speed and changes its target affinity.

`Location: Assets/Scripts`

### Spawning System
Controls enemy population and difficulty.
*   `EnemySpawner`: A simple spawner that instantiates `Enemy` prefabs at a fixed rate from its own position (or randomized offsets).

`Location: Assets/Scripts`

### Visual Feedback System
Manages animations and effects for game events.
*   `SelfDestruct`: Utility script used for fire-and-forget effects like `Flame_Death_Effect`.
*   `CooldownBar`: A world-space UI element following the player to visualize barrier readiness.

`Location: Assets/Scripts`

## 5. Scene Overview
*   **Main**: The primary gameplay scene. It contains the `Player` prefab (with `Barrier` and `CooldownBar` children), the `EnemySpawner`, a `Global Volume` for post-processing, and a `UI` GameObject for the HUD.
*   **Scene Flow**: Currently a single-scene loop. Transitions are handled via logic (e.g., enemy destruction, player damage) rather than scene loading.

`Location: Assets/Main.unity`

## 6. UI System
The project uses **Unity UI Toolkit (UITK)** for the screen-space HUD and **UGUI/Sprites** for world-space indicators.
*   `PlayerHUD`: A bridge component that connects `PlayerController` data to the `UIDocument` (`Main.uxml`). It updates health and barrier bars via the `hud-root`.
*   `Main.uxml / Main.uss`: Defines the visual layout and styling for the HUD elements.
*   `CooldownBar`: A sprite-based world-space UI attached to the player to show the barrier cooldown progress (`GetCooldownProgress`).

`Location: Assets/UI (UITK) and Assets/Scripts (Logic)`

## 7. Asset & Data Model
*   **Prefabs**:
    *   `Enemy`: Contains the `Enemy` script, `Animator`, and `Rigidbody2D`.
    *   `Projectile`: Contains `Projectile` script and a `CircleCollider2D` set to trigger.
    *   `Flame_Death_Effect`: Visual-only prefab with a `SelfDestruct` timer.
*   **Post-Processing**: Uses a `UniversalRenderPipelineAsset` with a custom `EightColorFeature` (KinoEight) to enforce a retro 8-color look.
*   **Animations**: Handled via `AnimatorControllers`. Key states include `Player_Walk`, `Enemy_Ghoul_Walk`, and `Flame_Death`.
*   **Naming Convention**: Standard Unity PascalCase for scripts and folders; underscores for animation clips (e.g., `Enemy_Ghoul_Walk`).

`Location: Assets/Prefabs, Assets/Animations, Assets/URP`

## 8. Notes, Caveats & Gotchas
*   **Reflect Logic**: The `Barrier` uses `(other.transform.position - transform.position).normalized` as the "normal" for reflection. This assumes a perfectly circular barrier centered on the player.
*   **Sprite Flipping**: The `PlayerController` and `Enemy` scripts manually flip `SpriteRenderer.flipX` based on movement delta. If the base sprite orientation changes, the logic in `UpdateAnimations` must be adjusted.
*   **Input**: Since it uses the New Input System package, `Mouse.current` will fail if the Input System is set to "Both" or "Old" in Project Settings without appropriate changes.
*   **Visuals Hierarchy**: The `PlayerController` looks for a child named "Visuals" to apply animations and damage flashes. Changing the hierarchy names will break visual feedback.