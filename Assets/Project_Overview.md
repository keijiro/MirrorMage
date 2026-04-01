# MirrorMage Project Technical Overview

## 1. Project Description
**MirrorMage** is a 2D top-down action "survivor" style game built in Unity 6. The core gameplay focuses on a unique defensive-offensive mechanic: rather than attacking directly, the player uses a magical barrier to reflect enemy projectiles back at them. The project targets players who enjoy bullet-hell dodging mechanics and strategic power-up progression.

**Core Pillars:**
- **Reflective Combat:** Success depends on timing the barrier to redirect incoming spells.
- **Progression Loop:** Defeating enemies grants XP, leading to level-ups and stat upgrades.
- **Visual Polish:** High-juice feedback including screen shakes, hit flashes, and stylized URP effects.

## 2. Gameplay Flow / User Loop
1.  **Boot/Title:** The game starts in the `TitleScreen` scene. The user clicks "Start" via `TitleScreenController`.
2.  **Main Gameplay:** 
    - **Movement:** Player follows the mouse cursor.
    - **Defense/Offense:** Player activates a circular barrier (LMB) to reflect red projectiles. Reflected projectiles turn yellow, move faster, and destroy enemies on contact.
    - **Spawning:** `EnemySpawner` continuously generates enemies that move toward or keep distance from the player while shooting.
3.  **Progression:** Enemies drop XP. Upon leveling up, `LevelUpUI` pauses the game and presents three randomized upgrades (Move Speed, Charge Speed, Barrier Strength).
4.  **Game Over:** If player health reaches zero, the `PlayerController` triggers a death sequence, eventually leading to the `GameOver` scene.

## 3. Architecture
The project follows a component-based architecture with a heavy reliance on Coroutines for stateful animations and gameplay sequences.

- **Main Entry Point:** `Main.unity` scene contains the `PlayerController` and `EnemySpawner` which drive the active session.
- **State Management:** Time-scale manipulation (`Time.timeScale = 0`) is used for pausing during Level Up and Game Over transitions.
- **Event Handling:** The project uses standard Unity `OnTriggerEnter2D` for collision-based interactions (Projectiles hitting Barriers/Enemies).
- **UI Architecture:** Built using **UI Toolkit (UITK)**. Scripts query `VisualElement` via `rootVisualElement.Q<T>` and bind logic to styles/classes.

`Location: Assets/Scripts/`

## 4. Game Systems & Domain Concepts

### Combat & Reflection System
A "Reflection" logic governs combat. Projectiles are non-damaging to enemies until reflected by the player's barrier.
- `Projectile`: Handles movement and the `Reflect` method which flips direction and doubles speed.
- `Barrier`: Attached to the player; triggers `Reflect` on any incoming `Projectile` with the correct tag.
`Location: Assets/Scripts/`

### Enemy AI & Spawning
Enemies use a state-based behavior routine handled via Coroutines.
- `Enemy`: Implements `BehaviorRoutine` for movement (approaching or distancing) and `Shoot` for projectile patterns.
- `EnemySpawner`: Manages wave-like instantiation of enemy prefabs around the player.
`Location: Assets/Scripts/`

### Leveling & Upgrade System
A standard XP-based progression system.
- `PlayerController`: Tracks `currentXP` and `xpToNextLevel`. Triggers the `LevelUpUI`.
- `LevelUpUI`: Manages the UITK-based selection screen, applying stat modifiers (e.g., `moveSpeed`, `barrierCooldown`) directly to the player.
`Location: Assets/Scripts/`

## 5. Scene Overview
- **TitleScreen**: Initial landing page. Uses `TitleScreenController` to handle scene transitions.
- **Main**: The primary gameplay arena. Contains the player, spawners, and the game loop logic.
- **GameOver**: Terminal state scene showing final results and allowing restart.
`Location: Assets/Scenes/`

## 6. UI System
The UI is implemented using **Unity UI Toolkit (UITK)**, utilizing `.uxml` for structure and `.uss` for styling.

- **Player HUD**: Displays health and XP bars using width percentage binding in `PlayerHUD.cs`.
- **Level Up UI**: Features a class-based animation system. It uses `AddToClassList` to trigger USS transitions for "Selected", "Flash", and "Dimmed" states.
- **Game Start Intro**: `GameStartController` manages a stylized cinematic entry using `fadeOverlay` and blinking instruction labels.
- **Styling**: Uses the `Jersey_10` font for a consistent retro-arcade aesthetic.
`Location: Assets/UI/`

## 7. Asset & Data Model
- **Prefabs**: Core entities (`Enemy`, `Projectile`, `Flame_Death_Effect`) are prefabs for easy spawning.
- **Materials/Shaders**: Uses URP-compatible shaders like `ColorPulse.shader` for environmental effects.
- **Animations**: Uses the Unity Animator system. Notable controllers include `PlayerController` and `Enemy_Bat_Controller`.
- **Input**: Configured using the **New Input System**, specifically tracking `Mouse.current.position` and `leftButton`.
`Location: Assets/Prefabs/`, `Assets/Shaders/`, `Assets/Animations/`

## 8. Notes, Caveats & Gotchas
- **Time Scale:** The project uses `Time.timeScale = 0` for Level Up. UI animations must use `unscaledDeltaTime` or `WaitForSecondsRealtime` to function during these pauses.
- **Visuals Child:** `PlayerController` expects a child GameObject named "Visuals" to handle sprite flipping and animations without affecting the root transform's physics/logic.
- **Reflection Logic:** Reflection is "pure" (geometric reflection off the normal). If the barrier's collider isn't perfectly circular or centered, projectiles might reflect in unexpected directions.
- **Tag Dependency:** Ensure projectiles are tagged as `Projectile` and enemies as `Enemy` for the `OnTriggerEnter2D` logic to resolve correctly.