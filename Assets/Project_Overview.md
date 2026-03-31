# MirrorMage Project Overview

## 1. Project Description
**MirrorMage** is a high-octane 2D top-down action game where the player controls a mage whose primary defense is a magical reflection barrier. Instead of traditional projectile attacks, the player must time the activation of their barrier to reflect enemy bullets back at them, turning the enemies' own strength into their downfall. The game features a rogue-like progression system with experience points, leveling, and permanent upgrades during a run.

**Core Pillars:**
- **Reflective Combat:** Gameplay revolves around the "Reflect" mechanic rather than direct shooting.
- **Precision Timing:** Successful play requires managing a short-lived barrier with a significant cooldown.
- **Escalating Threat:** Swarms of enemies (Bats, Ghouls, Skeletons) increase in density and bullet complexity.
- **Arcade Progression:** Fast-paced level-up cycles with meaningful stat choices.

## 2. Gameplay Flow / User Loop
1.  **Boot & Title:** The player starts at the `TitleScreen` scene, featuring a logo ripple effect and a pulse-shaded background.
2.  **The Run:** Transition to the `Main` scene where the `EnemySpawner` begins generating waves of enemies.
3.  **Active Combat:**
    - Player moves the mage toward the mouse cursor.
    - Enemies fire projectiles in patterns (single, spread, or repeated bursts).
    - Player activates the `Barrier` (Left Click) to reflect projectiles, turning them yellow and increasing their speed/damage.
4.  **Progression:**
    - Defeated enemies drop XP (added directly to `PlayerController`).
    - Upon reaching an XP threshold, the game pauses, and the `LevelUpUI` (UITK) appears.
    - Player chooses between Move Speed, Charge Speed (Cooldown), or Barrier Strength (Duration/Scale).
5.  **Failure/Shutdown:** If health reaches zero, the run ends (currently logs to console, returning to Title or restarting is the intended loop).

## 3. Architecture
The project follows a **Component-Based Architecture** with a central **Player-Centric state** and **Reactive UI**.

- **Main Entry Point:** The `Main.unity` scene initializes the `PlayerController` and `EnemySpawner`.
- **State Management:** `PlayerController` acts as the "Single Source of Truth" for health, XP, and barrier state. UI elements query this state rather than maintaining their own.
- **Communication Pattern:** 
    - **Polling:** `PlayerHUD` and `CooldownBar` poll the player's state in `Update`.
    - **Events/Calls:** `PlayerController` explicitly calls `LevelUpUI.Show()` when XP thresholds are met.
    - **Physics/Triggers:** Collision-based interaction between `Projectile`, `Barrier`, and `Enemy` handles the combat logic.

`Location: Assets/Scripts/`

## 4. Game Systems & Domain Concepts

### Reflection Combat System
The heart of the game, governed by the interaction between the player's barrier and enemy bullets.
- `Barrier`: A child object of the player that triggers reflection logic in projectiles.
- `Projectile`: Handles its own movement and geometric reflection.
- `Reflect()`: A method in `Projectile` that flips the `isReflected` flag, reverses direction based on normals, and doubles speed.
`Location: Assets/Scripts/`

### Enemy AI & Spawning
Enemies use a Coroutine-based behavior system for non-blocking state management.
- `EnemySpawner`: Handles the instantiation of enemy prefabs around the player.
- `Enemy`: Uses `BehaviorRoutine()` to cycle between "Move to/from Player" and "Shoot Patterns".
- `Shoot()`: Supports `bulletCount` and `spreadAngle` for complex bullet-hell-lite patterns.
`Location: Assets/Scripts/`

### Progression & Leveling
A standard XP-to-Level curve that triggers UI interruptions.
- `PlayerController.GainXP()`: Increases XP and checks against `xpToNextLevel` (which scales by 25% per level).
- `LevelUpUI`: Manages the UITK overlay, pauses the game via `Time.timeScale = 0`, and applies stat multipliers.
`Location: Assets/Scripts/`

## 5. Scene Overview
- **TitleScreen:** Contains the `TitleScreenController`, `PulseEffect` for lighting, and a UI Toolkit document for the start menu. Uses specialized shaders for the logo.
- **Main:** The primary gameplay arena. Includes the `PlayerController` setup, `EnemySpawner`, and the `Kino.Eight` post-processing setup for a stylized retro look.
`Location: Assets/Scenes/`

## 6. UI System
The project uses **UI Toolkit (UITK)** for primary screens and **Sprite-based World UI** for immediate feedback.

- **UI Toolkit (`LevelUpUI`):** Managed via `LevelUpUI.uxml` and `LevelUpUI.uss`. It uses USS Class transitions (e.g., `option-card--hidden` to `option-card--selected`) for animations.
- **HUD (`PlayerHUD`):** A UITK-based overlay showing health and XP bars.
- **World-Space UI (`CooldownBar`):** A custom script that scales a `SpriteRenderer` transform to show the barrier's cooldown progress directly next to the player character.
`Location: Assets/UI/, Assets/Scripts/`

## 7. Asset & Data Model
- **Prefabs:** Core entities (`Enemy`, `Projectile`, `Flame_Death_Effect`) are prefabbed for easy spawning by managers.
- **Animations:** Uses `AnimatorControllers` with simple state machines for "Walk" and "Fly" cycles. `Flame_Death_Effect` is a one-shot animation with a `SelfDestruct` script.
- **Visual Style:** Powered by URP 2D and the `KinoEight` post-processing effect which limits the color palette and applies a dither/retro filter.
- **Shaders:** Custom Shader Graph files (`ColorPulse`, `Title_Logo_Ripple`) provide "juice" to the title screen and background.
`Location: Assets/Prefabs/, Assets/Animations/, Assets/Shaders/`

## 8. Notes, Caveats & Gotchas
- **Time Scaling:** `LevelUpUI` sets `Time.timeScale = 0`. Any logic intended to run while the level-up screen is open (like the title flashing) must use `Time.unscaledTime`.
- **Reflection Logic:** Reflection is currently handled by `Projectile` detecting the `Barrier` tag. If the barrier's scale is increased via upgrades, the physics collision matrix must ensure the trigger still fires correctly.
- **Movement:** The player uses a simple `MoveTowards` to the mouse position. If the player is "stuck," check the `distance > 0.1f` threshold in `PlayerController`.
- **Visuals Child:** `PlayerController` expects a child GameObject named "Visuals" to handle flipping and animations; if this structure is broken, it defaults to the root transform.