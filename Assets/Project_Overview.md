# Project Overview: Project Reflect

## 1. Project Description
**Project Reflect** is a 2D top-down action prototype where the primary mechanic centers around a defensive barrier capable of reflecting enemy projectiles. Players must navigate a hazardous environment, timing their barrier activation to turn incoming fire back against a persistent wave of spawning enemies. The project serves as a technical demonstration of projectile reflection physics, enemy spawning logic, and mouse-driven character movement within a 2D URP environment.

**Core Pillars:**
*   **Reflective Combat:** Using defense as the primary offense by parrying/reflecting bullets.
*   **Fluid Movement:** Mouse-following movement that allows for precise positioning.
*   **Dynamic Difficulty:** Constant enemy pressure through automated spawning systems.

## 2. Gameplay Flow / User Loop
1.  **Initialization:** The game starts in the `Main` scene. The `EnemySpawner` begins generating enemies at a set distance from the center.
2.  **Navigation:** The player moves the mouse cursor; the player character automatically moves toward the cursor position using `Vector3.MoveTowards`.
3.  **Threat Engagement:** Enemies track the player's position and fire projectiles in spread patterns.
4.  **Action/Counter-Action:** 
    *   The player triggers the **Barrier** (Left Mouse Button).
    *   While the barrier is active, any `Projectile` colliding with it is marked as `isReflected`, has its velocity mirrored via a surface normal, and increases in speed.
    *   Reflected projectiles can destroy `Enemy` units.
5.  **Failure State:** If an un-reflected projectile or an enemy unit touches the player, a hit is registered (currently logged to the console).
6.  **Cooldown Management:** The barrier has a limited duration and a cooldown period, forcing the player to time its use strategically.

## 3. Architecture
The project follows a standard Unity Component-based architecture with a "Manager-less" approach for this prototype, where individual systems handle their own logic via `Update` loops.

*   **Input Handling:** Uses the **New Input System** (Package: `com.unity.inputsystem`). Inputs are polled directly in `PlayerController.cs` using `Mouse.current`.
*   **Movement & Physics:** Uses `Rigidbody2D` for physical presence and `CircleCollider2D` for trigger-based interactions. Movement is calculated manually in `Update` and applied via `transform.position`.
*   **Projectile System:** A "Fire and Forget" system where projectiles move in a linear direction until they collide with a barrier, an enemy, or leave a specific radius from the origin.
*   **Rendering:** Built on the **Universal Render Pipeline (URP)** with a 2D Renderer configuration.

## 4. Game Systems & Domain Concepts

### Player Control System
*   `PlayerController`: Manages mouse-following movement, animation state updates (`MoveX`, `MoveY`, `IsMoving`), and the activation/deactivation logic of the Barrier sub-object.
*   **Extension:** To add new abilities (e.g., a dash), add a new state check in `HandleBarrier` or a similar input polling method.

`Location: Assets/Scripts/PlayerController.cs`

### Combat & Reflection System
*   `Barrier`: A child object of the Player with a trigger collider. On `OnTriggerEnter2D`, it calculates the normal vector from the barrier center to the projectile and calls the reflection logic.
*   `Projectile`: Handles linear movement and collision logic. It maintains an `isReflected` state; if true, it gains a speed boost and becomes lethal to enemies instead of the player.
*   **Pattern:** The system uses a **State-based Projectile** pattern where the projectile's behavior and target-tag filtering change based on the `isReflected` flag.

`Location: Assets/Scripts/`

### Enemy & Spawning System
*   `Enemy`: Simple AI that follows the player's transform and fires projectiles at a fixed `fireRate` using a spread-shot pattern.
*   `EnemySpawner`: Spawns the `Enemy.prefab` at a randomized point on a circle perimeter (defined by `spawnDistance`) at regular intervals.
*   **Extension:** Create new enemy types by inheriting from `Enemy` or creating a ScriptableObject-based stat system for `fireRate` and `spreadAngle`.

`Location: Assets/Scripts/`

## 5. Scene Overview
*   **Main.unity:** The primary gameplay scene. It contains:
    *   **Main Camera:** Set to Orthographic with a background color.
    *   **Player:** The central entity with the `PlayerController` and child `Barrier`.
    *   **EnemySpawner:** A controller object managing the lifecycle of enemy instances.
    *   **Global Volume:** URP Post-processing settings (Bloom/Vignette).

`Location: Assets/Main.unity`

## 6. UI System
The project uses **UI Toolkit (UITK)** for its interface, though the current implementation is minimal.
*   `Main.uxml`: Defines the visual structure of the UI.
*   `DefaultPanel.asset`: Standard UITK panel settings for URP.
*   `DefaultTheme.tss`: The style sheet governing UI appearance.

`Location: Assets/UI/`

## 7. Asset & Data Model
*   **Prefabs:** 
    *   `Enemy.prefab`: Contains `SpriteRenderer`, `CircleCollider2D`, and the `Enemy` script.
    *   `Projectile.prefab`: Contains the `Projectile` script and a dedicated bullet sprite.
*   **Animations:** Uses an `AnimatorController` with a 2D Blend Tree (or similar parameter setup) based on `MoveX` and `MoveY` to handle 4-directional movement (Up, Down, Left, Right).
*   **Rendering:** Uses `Renderer2DData` (URP) to handle 2D lighting and sprite sorting.

`Location: Assets/Prefabs/`, `Assets/Animations/`, `Assets/URP/`

## 8. Notes, Caveats & Gotchas
*   **Hardcoded Boundaries:** Projectiles are destroyed if they exceed a distance of 50 units from `Vector2.zero` (`Projectile.cs`). If the player moves significantly away from the origin, projectiles may despawn prematurely.
*   **Reflection Normal:** The reflection in `Barrier.cs` is calculated based on the vector from the barrier's center to the projectile. This assumes the barrier is perfectly circular.
*   **Trigger Dependencies:** Both the Player and Enemy rely on `OnTriggerEnter2D`. Ensure that physics layers are correctly set if adding new obstacle types to prevent projectiles from reflecting off the player's body instead of the barrier.
*   **Input Latency:** Since `FollowMouse` uses `Vector3.MoveTowards`, the player always trails slightly behind the cursor depending on the `moveSpeed` value.