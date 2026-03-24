# Project Overview: MirrorMage

## 1. Project Description
MirrorMage is a 2D top-down action game built in Unity using the Universal Render Pipeline (URP). The core experience revolves around a "reflect and conquer" mechanic where the player, a mage, cannot directly attack enemies but must use a magical barrier to reflect enemy projectiles back at them. The project targets standalone platforms and emphasizes kinetic movement, timing-based defense, and satisfying visual feedback through shader-like effects and animations.

## 2. Gameplay Flow / User Loop
1.  **Boot & Initialization**: The game starts in the `Main.unity` scene. The `PlayerController` and `EnemySpawner` initialize, and the `PlayerHUD` binds to the player's state.
2.  **Movement & Evasion**: The player moves towards the mouse cursor position using `PlayerController.FollowMouse()`. Enemies (`Enemy.cs`) spawn and track the player, firing bursts of projectiles.
3.  **Reflection Mechanic**: The player monitors the `CooldownBar`. When ready, a left-click activates the `Barrier`. Any projectile hitting the barrier is reflected using geometric normals, gaining speed and a "reflected" status.
4.  **Combat Resolution**: Reflected projectiles can kill enemies on contact. If the player is hit by unreflected projectiles or enemies, they take damage and experience a screen-shake/flash effect.
5.  **Game State**: The loop continues until the player's health reaches zero or the session is closed.

## 3. Architecture
The project follows a decoupled, component-based architecture where individual actors manage their own logic and communicate via triggers or direct references managed in the scene.

### Core Gameplay Loop
*   `PlayerController`: Central hub for player state (HP, Barrier cooldown) and input processing.
*   `EnemySpawner`: Simple timer-based factory for instantiating enemy prefabs.
*   `Projectile`: Data container and movement logic for bullets, handling the "Reflected" state transition.

### Event & Data Flow
*   **Collision-Driven**: Most gameplay logic (damage, reflection) is triggered via `OnTriggerEnter2D`.
*   **UI Binding**: `PlayerHUD` uses a polling-style update (via `Update`) to reflect `PlayerController` stats onto UITK elements.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Reflection System
A specialized combat system where defense is the only offense.
*   `Barrier`: Manages the visual activation (pulses, fades) and the physical reflection logic.
*   `Projectile`: Handles the physics of reflection using `Vector2.Reflect` and modifies its own properties (speed, color, damage target) once reflected.
*   **Pattern**: Strategy pattern for projectile behavior—switching from "Hurt Player" to "Hurt Enemy" based on the `isReflected` flag.
`Location: Assets/Scripts`

### Health & Invincibility System
Manages actor survivability and feedback.
*   `PlayerController`: Implements a `DamageRoutine` coroutine that handles visual flashing and camera-shake-like jitter on the player's visual child object.
*   `Enemy`: Implements a `DeathRoutine` with a pre-destruction "shake and flash" sequence followed by a sprite-based death effect.
`Location: Assets/Scripts`

### Spawning System
*   `EnemySpawner`: Manages the lifecycle of enemy waves. It uses simple `Instantiate` calls at calculated intervals.
`Location: Assets/Scripts`

## 5. Scene Overview
*   **Main**: The primary gameplay scene. It contains the `URP` Global Volume for post-processing (Kino Eight), a 2D Global Light, the `Player` prefab, and the `EnemySpawner`.
*   **Scene Flow**: Currently a single-scene experience. Scene transitions are not explicitly implemented in the provided scripts, suggesting a focused "survival arena" scope.

## 6. UI System
The project utilizes **Unity UI Toolkit (UITK)** for its interface.
*   `PlayerHUD`: A bridge component that finds the `UIDocument` and updates the health bar and barrier progress.
*   `Main.uxml`: Defines the layout of the HUD, including the health and cooldown visuals.
*   `Main.uss`: Provides the styling for the UI elements.
*   `CooldownBar`: A separate World-Space Sprite-based UI component attached to the player for immediate visual feedback on the barrier status.

**Modification**: To add a new UI screen, create a new `.uxml` file, add it to the `UI` folder, and create a corresponding script to manage its `VisualElement` references.
`Location: Assets/UI`

## 7. Asset & Data Model
*   **Prefabs**: 
    *   `Enemy.prefab`: Contains the `Enemy` script, `Animator`, and `Rigidbody2D`.
    *   `Projectile.prefab`: A simple trigger-based sprite.
*   **Animations**: 
    *   Uses `AnimatorControllers` for the Player, Skeleton (Enemy), and Death Effects. 
    *   Movement is handled via sprite flipping in code (`flipX`) based on the movement vector.
*   **Post-Processing**: Uses `Kino Eight` for a stylized retro/limited-color look, configured in the `Global Volume`.
*   **Naming Convention**: Scripts are PascalCase, while assets generally follow PascalCase or underscores for specific variants (e.g., `Flame_Death`).

`Location: Assets/Prefabs, Assets/Animations`

## 8. Notes, Caveats & Gotchas
*   **Visual-Physics Separation**: The `PlayerController` separates the root `Transform` (for physics/movement) from the `Visuals` child. Damage shakes only the `Visuals` child to avoid messing with physics-based movement.
*   **Barrier Reflection**: The reflection angle is calculated based on the vector from the barrier's center to the projectile. If the projectile is inside the barrier when it activates, it might reflect in unexpected directions.
*   **FlipX Logic**: Character flipping is handled in `UpdateAnimations` (Player) and `Update` (Enemy) by checking `direction.x`. If characters appear to face the wrong way, check the default orientation of the source sprites.
*   **SelfDestruct Utility**: The `SelfDestruct` script is used for one-shot animation effects (like `Flame_Death_Effect`) to prevent memory leaks, using a hardcoded delay that matches the animation length.