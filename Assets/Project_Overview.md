# MirrorMage Technical Project Overview

## 1. Project Description
**MirrorMage** is a 2D action-survival game where the player controls a mage with the unique ability to reflect enemy projectiles back at them. The core experience centers on a "Risk vs. Reward" loop: players are vulnerable while moving towards enemies but can become a powerful offensive force by timing their barrier activation to catch and redirect incoming fire.

**Core Pillars:**
*   **Reflective Combat:** Defense is the best offense. Catching bullets with the barrier is the primary way to defeat enemies.
*   **Precision Timing:** The barrier has a short duration and a significant cooldown, requiring tactical usage.
*   **Dynamic Survival:** Players must balance movement (following the mouse) with the stationary-like commitment of barrier deployment.

## 2. Gameplay Flow / User Loop
1.  **Boot & Initialization:** The game starts in the `Main` scene. The `EnemySpawner` begins generating enemies at a distance.
2.  **Movement:** The player follows the mouse cursor position in world space.
3.  **Threat Engagement:** Enemies approach and fire multi-shot spread patterns at the player.
4.  **Barrier Interaction:** 
    *   Player presses Left Mouse Button to activate the `Barrier`.
    *   If a `Projectile` hits the active barrier, it is reflected, its speed is doubled, and it changes color (indicating it is now lethal to enemies).
5.  **State Transitions:** 
    *   **Barrier Active:** Player is protected from projectiles and contact damage.
    *   **Cooldown:** After 2 seconds, the barrier expires and enters a 5-second cooldown (tracked via `CooldownBar`).
    *   **Damage/Death:** If hit while the barrier is down, the player takes damage and enters a brief invincibility/flicker state.
6.  **Termination:** The loop continues until the player's health reaches zero (monitored by `PlayerHUD`).

## 3. Architecture
The project follows a **Component-Based Architecture** where logic is decentralized into specific behaviors that interact via Unity's Physics2D system and Tag-based filtering.

**Main Entry Points:**
*   **PlayerController:** The central hub for player input, health, and state management (Active/Cooldown/Invincible).
*   **EnemySpawner:** Manages the game's difficulty floor by continuously instantiating enemy prefabs.
*   **Input System:** Uses the new `com.unity.inputsystem` for mouse tracking and button events.

**Data Flow:**
*   **Input -> PlayerController:** Physical mouse/click data drives movement and barrier state.
*   **Barrier -> Projectile:** Upon collision, the Barrier calls `Reflect()` on the Projectile, passing a surface normal.
*   **Projectile -> Enemy:** Reflected projectiles trigger `Die()` on Enemy components.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Combat & Reflection System
A physics-driven system where projectiles change "ownership" and properties based on collision with a player-managed barrier.
*   `Projectile`: Handles movement and the reflection math using `Vector2.Reflect`.
*   `Barrier`: A child object of the Player that manages its own visual "Expansion Effect" and detects projectile triggers.
*   `Enemy`: AI that tracks the player and fires projectiles using a `spreadAngle` calculation.

**Extension:** To add new projectile types (e.g., homing), create a subclass of `Projectile` and override the movement logic while maintaining the `Reflect()` interface.
`Location: Assets/Scripts`

### Health & Feedback System
Manages entity lifecycles and provides visual cues for damage and state changes.
*   `PlayerController`: Implements a `DamageRoutine` coroutine that handles screen shake (on visuals) and sprite flickering.
*   `Enemy`: Implements a `DeathRoutine` that includes a pre-death shake, HDR-white flash, and instantiation of a `Flame_Death_Effect`.

**Pattern:** The system uses **Coroutines** for time-based visual states (flashing, shaking, fading) to keep the `Update` loops clean.
`Location: Assets/Scripts`

## 5. Scene Overview
The project currently utilizes a single-scene structure:
*   `Main`: Contains the player, global URP settings, UI Document, and the `EnemySpawner`.
*   **Scene Flow:** Currently no scene transitions are implemented; the game operates as a single-level survival loop.
*   **Constraints:** The `EnemySpawner` relies on `Vector2.zero` as the center of the world for its spawn radius calculations.

`Location: Assets/Main.unity`

## 6. UI System
The project uses a hybrid approach: **UITK (UI Toolkit)** for HUD elements and **Sprite-based world-space UI** for gameplay indicators.

### HUD (UITK)
*   `PlayerHUD`: Binds to `Main.uxml`. It queries a `VisualElement` named `healthBarFill` and scales its width based on the player's health percentage.
*   `Main.uss`: Defines the styling and layout for the HUD.

### Gameplay Indicators (UGUI/Sprites)
*   `CooldownBar`: A world-space object attached to the Player. It uses `SpriteRenderer` masking and scale transforms to show the barrier's cooldown progress directly above the character.

**Extension:** To add a "Score" display, add a Label to `Main.uxml` and update it via `PlayerHUD.cs`.
`Location: Assets/UI` and `Assets/Scripts/PlayerHUD.cs`

## 7. Asset & Data Model
*   **Prefabs:** The `Enemy` and `Projectile` prefabs are the primary units of content. They are modular and contain their own SFX/VFX triggers.
*   **Animations:** Uses `AnimatorController` for state-driven sprite animations (`PlayerController`, `Skeleton_Controller`).
*   **Rendering:** Powered by **URP (Universal Render Pipeline)**. The `2D Renderer` asset handles light and post-processing.
*   **Naming Convention:** Scripts are PascalCase, while assets in folders generally follow the `Category_SpecificName` (e.g., `Flame_Death.anim`) or PascalCase convention.

`Location: Assets/Prefabs`, `Assets/Animations`, `Assets/URP`

## 8. Notes, Caveats & Gotchas
*   **Visuals Parenting:** The `PlayerController` expects a child object named `"Visuals"`. It applies shakes and flips to this child rather than the root to avoid disrupting physics or movement.
*   **Reflection Math:** The `Reflect` logic in `Barrier.cs` calculates the normal from the barrier center to the projectile. This means the angle of reflection is determined by *where* on the circle the bullet hits, not just the bullet's incoming angle.
*   **Z-Axis:** Although it is a 2D game, the `PlayerController` manually sets `worldPos.z = 0` to prevent the player from disappearing behind the camera or background planes.
*   **Tag Dependency:** The system relies heavily on the `Player`, `Enemy`, and `Projectile` tags. Changing these tags in the editor without updating `OnTriggerEnter2D` logic will break combat.