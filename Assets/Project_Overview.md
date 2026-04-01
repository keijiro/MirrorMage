Based on the project structure, source code, and assets, here is the technical documentation for MirrorMage.

# MirrorMage Project Overview

## 1. Project Description
MirrorMage is a 2D top-down action "bullet-reflection" game where the player assumes the role of a mage who lacks offensive spells but possesses a powerful magical barrier. The core experience centers on defensive-to-offensive conversion: surviving waves of projectiles by reflecting them back at enemies.

**Core Pillars:**
- **Reflective Combat:** Success is tied to the timing and positioning of the barrier rather than direct attacks.
- **Vampire-Survivors Style Progression:** XP-based leveling with mid-game upgrades to survive increasing difficulty.
- **High-Juice Feedback:** Heavy use of time-scale manipulation, screen shakes, and color flashing to communicate impacts and deaths.

## 2. Gameplay Flow / User Loop
1.  **Boot/Title:** The game starts in `TitleScreen.unity`, featuring a rippling logo and a character preview.
2.  **Main Loop:**
    - Player moves towards the mouse cursor.
    - `EnemySpawner` generates enemies outside the screen based on total XP.
    - Enemies fire projectiles at the player.
    - Player activates the `Barrier` (LMB) to reflect projectiles.
    - Reflected projectiles kill enemies, granting XP.
3.  **Progression:** Upon reaching XP thresholds, `LevelUpUI` pauses the game, offering three stat upgrades (Move Speed, Charge Speed, Barrier Strength).
4.  **Failure:** If the player takes lethal damage, a "Death Routine" triggers (time freeze, dark overlay, black fill transition), leading to `GameOver.unity`.
5.  **Restart:** Users can retry from the Game Over screen to return to the Main scene.

## 3. Architecture
The project follows a component-based architecture where the `PlayerController` acts as the central hub for state, but specific behaviors (Barrier, UI, Spawning) are delegated to dedicated MonoBehaviors.

- **Central Hub:** `PlayerController.cs` manages HP, XP, Level, and the Barrier's high-level state.
- **State Management:** Uses `Time.timeScale = 0` for pausing during Level Up and specific frames of the death sequence.
- **Input:** Utilizes the **New Input System**, specifically `Mouse.current` for movement (screen-to-world conversion) and barrier activation.
- **Scene Flow:** Managed via standard `SceneManager.LoadScene`, often preceded by UI-based fade-out coroutines.

## 4. Game Systems & Domain Concepts

### Combat & Reflection System
A two-stage projectile system where bullets change ownership and properties upon collision with a specific layer/tag.
- `Barrier.cs`: Detects `Projectile` tags; calculates reflection normals based on the vector from the barrier center.
- `Projectile.cs`: Implements `Reflect(Vector2 normal)`. Upon reflection, it toggles `isReflected = true`, doubles its speed, changes color to yellow, and becomes lethal to `Enemy` tags.
`Location: Assets/Scripts/`

### Enemy AI & Spawning
Uses a "state-machine-in-a-coroutine" pattern to handle movement and staggered firing.
- `Enemy.cs`: Coroutine-based `BehaviorRoutine` that alternates between moving towards/away from the player and a multi-shot firing sequence.
- `EnemySpawner.cs`: Dynamic difficulty scaling. It increases `baseSpawnsPerMinute` and `baseMaxEnemies` linearly based on the player's `totalXP`.
`Location: Assets/Scripts/`

### Leveling & Upgrade System
A standard "Survivors" upgrade loop.
- `PlayerController.GainXP()`: Tracks current and total XP. Levels up when `currentXP >= xpToNextLevel`.
- `LevelUpUI.cs`: Manages the UITK overlay, pauses the game, and applies modifiers (multipliers) to `PlayerController` fields.
`Location: Assets/Scripts/`

## 5. Scene Overview
- `TitleScreen.unity`: Initial entry point. Contains `TitleScreenController.cs` which handles the "Press Start" logic and logo ripple shaders.
- `Main.unity`: The primary gameplay arena. Contains the `Player`, `EnemySpawner`, and a `Global Volume` for URP post-processing.
- `GameOver.unity`: The failure state scene. Features a unique volume profile and a "Retry" button.
`Location: Assets/Scenes/`

## 6. UI System
The project exclusively uses **UI Toolkit (UITK)** for all interfaces, leveraging `.uxml` for structure and `.uss` for styling.
- **Framework:** `UIDocument` components attached to GameObjects.
- **Key Components:**
    - `PlayerHUD.cs`: Drives the `Main.uxml` file, updating the health bar, XP bar, and cooldown icon in real-time.
    - `LevelUpUI.cs`: Handles dynamic CSS class manipulation (e.g., `option-card--hidden` to `option-card--selected`) to drive animations without the Legacy Animator.
    - `CooldownBar.cs`: A world-space Sprite-based UI used for immediate feedback near the player character.
- **Transitions:** UI scripts often use `yield return new WaitForSecondsRealtime()` to allow UI animations to finish while the game world is paused.
`Location: Assets/UI/` and `Assets/Scripts/`

## 7. Asset & Data Model
- **Prefabs:**
    - `Enemy.prefab`: Base unit for all enemies.
    - `Projectile.prefab`: Shared by both enemies (harmful) and the player (after reflection).
- **Animations:** 2D Sprite animations using `Animator` and `AnimationClip`. The player's death animation is a 16-frame sequence at 12fps.
- **Shaders:** Custom ShaderGraph files like `Title_Logo_Ripple.shader` (Time-based vertex/UV offset) and `ColorPulse.shader` for background environmental effects.
- **Data Organization:** Simple folder-based taxonomy. No ScriptableObject-based databases are currently used for enemy stats; they are tuned directly on prefabs.
`Location: Assets/Prefabs/`, `Assets/Shaders/`

## 8. Notes, Caveats & Gotchas
- **Time Scale Dependency:** Many scripts use `Time.unscaledDeltaTime` or `WaitForSecondsRealtime` during the death sequence and level-up screen. If adding new systems (like particles), ensure they are set to "Unscaled" if they should play while the game is "frozen."
- **Visuals Childing:** `PlayerController` expects a child GameObject named `"Visuals"`. It dynamically searches for `Animator` and `SpriteRenderer` on this child to separate logic from rendering.
- **Reflection Logic:** Reflection is "pure." The `Projectile` does not target enemies; it reflects perfectly off the barrier's circular bounds. The player must aim their *body position* to angle the return shots.
- **Layer/Tag Dependency:** The reflection system relies on the `Projectile` and `Enemy` tags. Changing these strings in the Inspector will break the combat loop.