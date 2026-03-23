# MirrorMage Project Overview

## 1. Project Description
**MirrorMage** is a 2D top-down action game where the player controls a mage who survives not by attacking directly, but by reflecting enemy projectiles back at them. The core experience centers on timing and positioning, using a temporary magic barrier to turn an overwhelming bullet hell into a counter-offensive. It is designed as a focused arcade-style survival prototype using Unity’s Universal Render Pipeline (URP) and the New Input System.

## 2. Gameplay Flow / User Loop
1.  **Boot**: The game starts in the `Main` scene.
2.  **Survival**: The `EnemySpawner` continuously generates enemies at the edge of the screen that move toward the player.
3.  **Engagement**: Enemies fire projectiles at the player. The player moves using mouse-based following logic.
4.  **Reflection**: The player activates a `Barrier` (Left-Click) for a short duration. Projectiles hitting the active barrier are reflected, speed up, and can then destroy enemies.
5.  **Failure/Reset**: If a projectile or enemy touches the player while the barrier is down, a hit is logged (currently a debug log for the prototype phase).
6.  **Loop**: Survive as long as possible against increasing enemy pressure.

## 3. Architecture
The project follows a **Component-Based** architecture with a **Delegated Responsibility** pattern for collision and combat logic.
*   **Input**: Managed via the New Input System, specifically polling `Mouse.current` for positioning and clicking.
*   **Collision Logic**: Distributed across `Projectile.cs` and `Barrier.cs`. The barrier handles the calculation of the reflection vector, while the projectile handles its own state changes (e.g., `isReflected`) and damage application.
*   **State Management**: `PlayerController` acts as a local state machine for the player, toggling between "Normal" and "Protected" (Barrier Active) states.
*   **Entry Point**: The `Main` scene contains the `Player` object and an `EnemySpawner`.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Movement System
*   `PlayerController`: Implements a "Follow Mouse" mechanic where the character moves toward the world position of the cursor.
*   `Enemy`: Implemented with simple linear interpolation/translation toward the player's transform.

### Reflection Combat System
*   `Barrier`: A child object of the player that handles trigger detection for projectiles. It calculates the reflection normal based on the vector from the barrier center to the projectile.
*   `Projectile`: A moving entity that stores a `direction` and a `speed`. When reflected, its speed is doubled and its `isReflected` flag is set to true, allowing it to damage `Enemy` tags.

### Enemy & Spawning System
*   `EnemySpawner`: Periodically instantiates enemies at a fixed distance in a random circular direction around the origin.
*   `Enemy`: Handles shooting logic (spread fire) and a `DeathRoutine` that includes a visual shake/flicker effect before spawning a death prefab.

`Location: Assets/Scripts`

## 5. Scene Overview
*   **Main**: The primary gameplay scene. It contains the 2D Renderer setup, the player character, the spawner, and global volume settings for post-processing (Kino Eight).
*   **Scene Flow**: Currently a single-scene loop. All gameplay initialization occurs via `Start()` methods on MonoBehaviours within this scene.

`Location: Assets/Main.unity`

## 6. UI System
The project uses **UI Toolkit (UITK)** for its interface.
*   `Main.uxml`: Defines the visual layout of the UI.
*   `DefaultPanel.asset`: Contains the panel settings for the UI Toolkit, linking the UXML to the game view.
*   `DefaultTheme.tss`: Provides the styling for the UI elements.
*   **Integration**: Currently, the UI is primarily a static overlay/HUD defined in the UXML.

`Location: Assets/UI`

## 7. Asset & Data Model
*   **Prefabs**:
    *   `Enemy.prefab`: Contains the `Enemy` script, Animator, and 2D Collider.
    *   `Projectile.prefab`: A simple sprite with a trigger collider and the `Projectile` script.
    *   `Flame_Death_Effect.prefab`: A visual-only prefab used by `SelfDestruct` to clean up after the death animation.
*   **Animations**: Uses Unity's `AnimatorController` for sprite-based animations (Player walk, Enemy walk, Flame death).
*   **Rendering**: Uses **URP 2D Renderer**. Post-processing is handled by `DefaultVolume.asset`, utilizing the `Kino.Eight` extension for a stylized retro look.

`Location: Assets/Prefabs`, `Assets/Animations`, `Assets/URP`

## 8. Notes, Caveats & Gotchas
*   **Reflection Normal**: The `Barrier` calculates the reflection normal using `(other.transform.position - transform.position).normalized`. This assumes the barrier is a perfect circle centered on the player. If the barrier sprite/collider is non-uniform, reflection angles might feel counter-intuitive.
*   **Death Timing**: The `SelfDestruct` script has a hardcoded delay of `0.44f` seconds, specifically timed to match the 16-frame "Flame_Death" animation at 36fps. Changing the animation length requires updating this script.
*   **Screen Bounds**: The `Projectile` script destroys itself if it moves further than 50 units from `Vector2.zero`. If the player moves significantly far from the origin, projectiles might despawn prematurely or stay alive too long.
*   **Input**: The `PlayerController` uses `Mouse.current` directly in `Update`. This is efficient for prototypes but would need a wrapper for remapping or controller support.

`Location: Assets/Scripts`