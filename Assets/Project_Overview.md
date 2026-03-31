# Project Overview: MirrorMage

## 1. Project Description
MirrorMage is a 2D top-down action survival game where the player controls a mage who survives not by traditional casting, but by reflecting enemy projectiles back at them. The core experience revolves around precision timing of a magical barrier that turns incoming threats into lethal counter-attacks. It features a progression system where players gain experience from defeated enemies to upgrade their mobility and defensive capabilities.

**Core Pillars:**
- **Reflective Combat:** Defense is the best offense; timing the barrier is the primary interaction.
- **Scaling Difficulty:** Enemy spawns and counts scale based on the player's total accumulated XP.
- **Juicy Feedback:** High-impact visual effects for damage, death, and UI interactions using URP and UITK.

## 2. Gameplay Flow / User Loop
1.  **Boot/Title:** The game starts in the `TitleScreen` scene. The user clicks to begin, triggering a fade effect.
2.  **Initialization:** The `Main` scene loads. `PlayerController` initializes health/XP, and `EnemySpawner` begins generating threats outside the screen bounds.
3.  **Active Play:** The player moves via mouse position. Enemies approach and fire projectiles. The player must click the Left Mouse Button to activate the barrier just before impact.
4.  **Reflection & Scoring:** Reflected projectiles speed up and target enemies. Killing an enemy grants XP.
5.  **Level Up:** Upon reaching XP thresholds, `LevelUpUI` pauses the game (Time.timeScale = 0) and presents three upgrade options (Move Speed, Charge Speed, Barrier Strength).
6.  **Death/Reset:** If health reaches zero, a death sequence plays with a time-freeze and shadow fade, leading back to the game flow or restart.

## 3. Architecture
The project follows a decoupled, component-based architecture where the `PlayerController` acts as the central state holder for the session.

- **Central Hub:** `PlayerController` manages player state (Health, XP), movement logic, and the barrier lifecycle.
- **Spawning Logic:** `EnemySpawner` operates independently, polling the player's `totalXP` to scale spawn rates and enemy limits.
- **State Feedback:** Systems use Coroutines for time-based visual feedback (e.g., `DeathRoutine`, `DamageRoutine`, `FlashTitleColor`) rather than complex state machines.
- **Input:** Uses the **New Input System**, specifically polling `Mouse.current` for positioning and clicks.

`Location: Assets/Scripts/`

## 4. Game Systems & Domain Concepts

### Movement System
- `PlayerController`: Implements mouse-follow logic. The player character moves toward the mouse cursor's world position.
- `Enemy`: Implements distance-based movement. Enemies maintain a specific `keepDistance` from the player, moving forward or backward to stay within their effective range.

`Location: Assets/Scripts/`

### Barrier & Reflection System
- `Barrier`: A child object of the player that handles collision detection for reflection.
- `Projectile`: Moves in a set direction. When it hits a `Barrier`, it calls `Reflect()`, which uses `Vector2.Reflect` against the collision normal, doubles the speed, and flags the projectile as "reflected" to damage enemies.

`Location: Assets/Scripts/`

### Scaling Spawner System
- `EnemySpawner`: Manages the game's difficulty curve.
- Calculation: `baseSpawnsPerMinute + (xp * spawnsIncreasePerXP)`.
- It uses a "spawn margin" logic to instantiate enemies just outside the camera's orthographic bounds.

`Location: Assets/Scripts/`

## 5. Scene Overview
- **TitleScreen:** Entry point. Contains `TitleScreenController` and a logo with a ripple shader. Handles the transition to the main game.
- **Main:** The primary gameplay arena. Contains the player, global `EnemySpawner`, `PlayerHUD`, and the `LevelUpUI`.

`Location: Assets/Scenes/`

## 6. UI System
The project uses **UI Toolkit (UITK)** for all interface elements, providing a web-like styling approach via USS and UXML.

- **Player HUD:** `PlayerHUD.cs` binds to the player's health and XP progress, likely updating bars or labels.
- **Level Up Menu:** `LevelUpUI.cs` manages the upgrade screen. It uses `PanelSettings` and `VisualTreeAsset` to render. It features a C#-driven flashing title and CSS-class-based animations (staggered entry).
- **Styling:** Styles are defined in `LevelUpUI.uss` and `Main.uss`, using classes like `.option-card--hidden` for transition effects.

`Location: Assets/UI/`

## 7. Asset & Data Model
- **Prefabs:** Enemies (`Enemy_Bat`, `Enemy`) and effects (`Flame_Death_Effect`) are prefabbed for dynamic instantiation.
- **Animations:** Uses `AnimatorControllers` with triggers (e.g., `Die`) and state-based animations (e.g., `Player_Walk`).
- **Visuals:** Employs URP with custom shaders (`ColorPulse.shader`, `Title_Logo_Ripple.shader`) for "juicy" effects.
- **Naming Convention:** Prefixes like `Icon_`, `Player_`, and `Enemy_` are used to categorize textures and animations.

`Location: Assets/`

## 8. Notes, Caveats & Gotchas
- **Time Scale:** The `LevelUpUI` and `DeathRoutine` manipulate `Time.timeScale`. UI animations and the death performance use `UnscaledTime` to ensure they play while the game world is frozen.
- **Layering:** Projectiles check for `isReflected` to determine if they should damage the player or enemies. Ensure Tags (`Player`, `Enemy`) are correctly assigned on prefabs.
- **Screen Bounds:** `EnemySpawner` relies on `Camera.main.orthographicSize`. Changing camera settings will affect enemy spawn distances.
- **Visuals Transform:** `PlayerController` looks for a child named "Visuals" to apply animations and sprite flipping. If missing, it defaults to the root transform.