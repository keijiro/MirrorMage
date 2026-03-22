# MirrorMage Project Overview

## 1. Project Description
MirrorMage is a 2D top-down action game where the player controls a mage who survives by reflecting enemy projectiles back at them. The core experience centers around precise movement and the strategic use of a "Mirror Barrier" to turn defensive situations into offensive opportunities. It is designed as a fast-paced arcade-style survival experience.

**Core Pillars:**
*   **Reflective Combat:** Projectiles are not just hazards but the primary source of player damage via reflection.
*   **Mouse-Driven Agility:** Fluid movement where the player character continuously follows the cursor.
*   **Resource Management:** Balancing the short-duration barrier and its cooldown to survive bullet-hell patterns.

## 2. Gameplay Flow / User Loop
1.  **Boot/Initialization:** The `Main.unity` scene loads. The `EnemySpawner` begins generating enemies at a set interval outside the camera view.
2.  **Engagement:** Enemies move toward the player and fire patterns of projectiles (spread shots).
3.  **Core Loop:**
    *   **Move:** Player uses the mouse to reposition the character.
    *   **Defend/Attack:** Player activates the `Barrier` (Left Click) to intercept projectiles. 
    *   **Reflect:** Projectiles hitting the active barrier become "Reflected" (increasing speed and changing color) and are redirected based on the angle of impact.
    *   **Eliminate:** Reflected projectiles destroy enemies on contact.
4.  **Attrition:** If the player is hit by an unreflected projectile or an enemy, damage is logged (extendable to a health system).

## 3. Architecture
The project follows a component-based architecture where behavior is localized within specific MonoBehaviors that interact via Unity's Physics2D system (Triggers).

*   **Entry Point:** The `Main.unity` scene contains the `Player`, `EnemySpawner`, and `UI` root.
*   **Input:** Utilizes the **New Input System**. `PlayerController` reads `Mouse.current` for positioning and clicking.
*   **Communication:** 
    *   **Collision-Based:** Most logic (damage, reflection) is triggered by `OnTriggerEnter2D`.
    *   **State-Driven:** The `PlayerController` manages the state of the `Barrier` child object.
*   **Data Flow:** Projectile data (direction, speed, reflected state) is passed to the projectile upon instantiation or reflection.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Movement System
Handles player and enemy positioning.
*   `PlayerController`: Implements cursor-following logic using `Vector3.MoveTowards`.
*   `Enemy`: Implements simple tracking toward the player's transform.
*   `Projectile`: Moves linearly based on a `direction` vector.

`Location: Assets/Scripts`

### Combat & Reflection System
The primary mechanic for interaction.
*   `Barrier`: Detects projectiles and calls the `Reflect` method.
*   `Projectile`: Manages its own state (`isReflected`). When reflected, it uses `Vector2.Reflect` against the normal calculated from the barrier's center to the impact point.
*   `Enemy`: Contains the `Shoot` logic, creating spread patterns using `Quaternion.Euler` rotations.

`Location: Assets/Scripts`

### Spawning System
Manages the game's difficulty and entity lifecycle.
*   `EnemySpawner`: Spawns enemies on a timer at a normalized distance around the center (0,0).
*   `Projectile`: Includes self-cleanup logic when moving too far from the origin.

`Location: Assets/Scripts`

## 5. Scene Overview
*   **Main.unity:** The primary gameplay scene. It contains:
    *   `Camera`: Standard 2D view with `UniversalAdditionalCameraData`.
    *   `Player`: The player entity with a nested `Barrier` object.
    *   `EnemySpawner`: The manager for enemy lifecycle.
    *   `UI`: A `UIDocument` for the HUD/Interface.
*   **Scene Transitions:** Currently, the project operates within a single-scene loop.

`Location: Assets/`

## 6. UI System
The project uses **UI Toolkit (UITK)** for its interface.
*   `UIDocument`: The component on the `UI` GameObject that hosts the interface.
*   `Main.uxml`: Defines the structure of the UI.
*   `DefaultTheme.tss`: Provides the visual styling.
*   `DefaultPanel.asset`: Contains the panel settings for rendering.

`Location: Assets/UI`

## 7. Asset & Data Model
*   **Prefabs:** 
    *   `Enemy.prefab`: Pre-configured with `Enemy.cs`, `Animator`, and `CircleCollider2D`.
    *   `Projectile.prefab`: Pre-configured with `Projectile.cs` and `CircleCollider2D` (Trigger).
*   **Animations:** 
    *   `PlayerController.controller` & `Skeleton_Controller.controller`: Handle basic walk cycles. 
    *   `Player_Walk.anim` & `Skeleton_Walk.anim`: Frame-based 2D animations.
*   **Rendering:** Uses **Universal Render Pipeline (URP)** with a 2D Renderer.
*   **Naming Conventions:** Scripts use PascalCase; private variables in scripts use `_camelCase` with an underscore prefix (e.g., `_barrierTimer`).

`Location: Assets/`

## 8. Notes, Caveats & Gotchas
*   **Reflection Normal:** The `Barrier` calculates the reflection normal based on the vector from the barrier's center to the projectile. This means the projectile's bounce direction is highly sensitive to its position relative to the player at the moment of impact.
*   **Tags:** The system relies heavily on tags (`Player`, `Enemy`, `Projectile`). Ensure any new prefabs have these tags assigned or collision logic will fail.
*   **Z-Axis:** Although 2D, `ScreenToWorldPoint` uses a Z depth of 10f to ensure the camera (at Z: -10) picks up the correct world position, which is then flattened to Z: 0.
*   **Animation Direction:** The `PlayerController` and `Enemy` handle sprite flipping in code via `spriteRenderer.flipX` based on the movement vector, rather than using Animator parameters.