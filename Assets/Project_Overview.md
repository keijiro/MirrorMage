# Project Overview: MirrorMage

## 1. Project Description
MirrorMage is a 2D top-down "survivors-like" action game where the player controls a mage who uses a magical barrier to reflect enemy projectiles. Unlike traditional shooters, the core mechanic revolves around defensive reflection—turning enemy attacks back against them to clear waves and level up. The game targets players who enjoy skill-based positioning and "bullet-hell" style patterns.

**Core Pillars:**
- **Reflect and Redirect:** Defense is the best offense; projectiles become more powerful when reflected.
- **Positioning Mastery:** The player follows the mouse, requiring precise movement to align reflections.
- **Progression-Loop:** Defeating enemies grants XP, leading to upgrades that enhance mobility and barrier efficiency.

## 2. Gameplay Flow / User Loop
- **Boot:** The game initializes in the `Main` scene with the `EnemySpawner` active.
- **Engagement:** 
    - The player moves by following the mouse cursor.
    - Enemies spawn and fire multi-shot patterns at the player.
    - The player activates the **Barrier** (Left Click) to protect themselves and reflect projectiles.
- **Progression:**
    - Reflected projectiles kill enemies.
    - Enemies drop XP (automatically collected) upon death.
    - Reaching the XP threshold pauses the game and triggers the **LevelUpUI**.
- **Upgrade:** The player chooses one of three upgrades (Move Speed, Charge Speed, Barrier Strength) to improve their build.
- **Failure:** If the player takes too much damage (HP reaches 0), the run ends (currently logs "Player Died").

## 3. Architecture
The project follows a decoupled, component-based architecture where systems communicate via direct references (assigned in inspector) or basic Unity `OnTrigger` events.

### Main Entry Points
- `PlayerController.cs`: The central hub for player state (HP, XP, Level) and input handling.
- `EnemySpawner.cs`: Manages the game's difficulty curve and enemy instantiation.
- `LevelUpUI.cs`: Manages game-state transitions (pausing/unpausing) and the upgrade logic.

### Design Patterns
- **Manager-less Design:** While there isn't a global "GameManager" singleton, the `PlayerController` acts as the primary data container and system orchestrator.
- **State Polling/Events:** UI elements (HUD) poll the player's state, while the Level Up system uses a synchronous "Show/Hide" pattern that modifies `Time.timeScale`.
- **Object Interaction:** Uses 2D Physics triggers for the reflection logic, where the `Barrier` script calculates the reflection vector using the geometric normal of the collision.

`Location: Assets/Scripts/`

## 4. Game Systems & Domain Concepts

### Movement & Controls
- `PlayerController`: Implements mouse-follow movement logic. Instead of direct WASD, the player character smoothly interpolates towards the mouse position.
- Uses the **New Input System** to read mouse clicks and positions.
`Location: Assets/Scripts/PlayerController.cs`

### Barrier & Reflection System
- `Barrier`: Attached to a child object of the Player. It manages its own visual lifecycle (pulsing, flashing, expanding) and physical collider state.
- `Projectile`: A generic projectile class that stores `direction` and `isReflected` state. When reflected by a barrier, it doubles its speed and changes its `isReflected` flag to target enemies.
- **Logic:** Reflection is handled in `Barrier.OnTriggerEnter2D` using `Vector2.Reflect`.
`Location: Assets/Scripts/`

### Enemy AI & Spawning
- `Enemy`: Operates using a Coroutine-based behavior tree (`BehaviorRoutine`). It alternates between moving towards the player and firing a burst of projectiles.
- `EnemySpawner`: (Logic inferred) Periodically instantiates the `Enemy.prefab` at designated points or random offsets.
`Location: Assets/Scripts/`

### Damage & Health System
- The player has a health bar and an "invincibility" state triggered after taking damage.
- `TakeDamage` triggers a `DamageRoutine` coroutine that handles sprite flashing and screen/visual shake.
`Location: Assets/Scripts/PlayerController.cs`

## 5. Scene Overview
- `Main.unity`: The primary gameplay scene. It contains:
    - **Player:** With child objects for Visuals, Barrier, and CooldownBar.
    - **EnemySpawner:** Responsible for enemy lifecycle.
    - **UI:** Contains the `UIDocument` for the HUD and the `LevelUpUI` overlay.
    - **Global Volume:** Handles URP post-processing (Kino Eight-Color effect).

## 6. UI System
The project uses **UI Toolkit (UITK)** for its interface, defined in `.uxml` and `.uss` files.

- **HUD (`PlayerHUD.cs`):** (Inferred) Manages the visibility of player stats.
- **Level Up UI (`LevelUpUI.cs`):**
    - Uses a `UIDocument` with a hidden-by-default overlay.
    - **Styling:** Uses USS class toggles (`.option-card--hidden`, `.option-card--selected`) for animations.
    - **Binding:** Buttons are queried via `Q<Button>` and assigned callbacks for upgrades.
    - **Extension:** To add new upgrades, add a button to `LevelUpUI.uxml` and a corresponding method in `LevelUpUI.cs`.

`Location: Assets/UI/`

## 7. Asset & Data Model
- **Prefabs:**
    - `Enemy.prefab`: Base unit for all enemies.
    - `Projectile.prefab`: Used by both enemies and reflected by players.
    - `Flame_Death_Effect.prefab`: Visual-only prefab spawned when an enemy dies.
- **Visual Assets:**
    - Uses a 2D URP pipeline.
    - Sprite-based animations managed by `AnimatorControllers`.
- **Post-Processing:**
    - Uses the `Kino Eight-Color` post-processing effect via a `Global Volume` to achieve a retro/limited-palette aesthetic.

## 8. Notes, Caveats & Gotchas
- **Visuals Child:** The `PlayerController` looks for a child named "Visuals" to handle flipping and shaking. If this object is missing, it defaults to its own Transform, which may result in shaking the collider/camera attachment.
- **Reflection Timing:** The reflection logic is "pure" physics-based. If a projectile is moving too fast and the frame rate is low, it might pass through the barrier collider (ensure "Continuous" collision is used on fast projectiles).
- **Time Scale:** The Level Up system sets `Time.timeScale = 0`. Any animations or logic intended to run during Level Up must use `unscaledDeltaTime` or be set to "Unscaled" in the Animator.
- **XP Scaling:** `xpToNextLevel` increases by 25% per level. Be mindful of potential overflow or impossible grinds if game sessions last very long.