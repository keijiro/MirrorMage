# MirrorMage Project Overview

## 1. Project Description
MirrorMage is a high-octane 2D top-down action game where players control a mage whose primary defense is a reflective barrier. Instead of traditional projectile attacks, the player must reflect enemy bullets back at them to survive and clear waves. It is designed for players who enjoy "bullet hell" mechanics with a defensive twist, focusing on timing and positioning. The core pillars of the experience are **Reflexive Combat**, **Risk-Reward Barrier Management**, and **Persistent Progression** via a level-up system.

## 2. Gameplay Flow / User Loop
*   **Boot & Title**: The player starts at the `TitleScreen` scene, where they can view instructions and start the game.
*   **Main Game Loop**: 
    1.  **Movement**: The player follows the mouse cursor to navigate the arena.
    2.  **Survival**: Enemies spawn and shoot projectiles at the player.
    3.  **Barrier Usage**: The player activates a timed barrier (Left Click) to reflect projectiles. 
    4.  **Progression**: Killing enemies grants XP. Reaching an XP threshold triggers a `LevelUpUI` overlay.
    5.  **Upgrade**: The player selects one of three permanent buffs (Speed, Cooldown, or Barrier Strength), resuming the action.
*   **Death**: Upon losing all health, a dramatic death sequence plays, transitioning to the `GameOver` scene.
*   **Restart**: The player can return to the title screen or retry from the game over menu.

## 3. Architecture
The project uses a component-based architecture with several manager singletons and scene-based separation for global systems.

*   **Audio Management**: A persistent `AudioManager` singleton handles SFX and BGM across all scenes. It is loaded via the `AudioSceneLoader` which ensures the `AudioSystem` scene is present.
*   **Input Handling**: Uses the New Input System for mouse-based movement and click actions.
*   **UI Architecture**: Built using Unity UI Toolkit (UITK) for all screens, utilizing `.uxml` and `.uss` files for layout and styling.
*   **Game State**: Primarily managed within `PlayerController` (HP/XP) and `EnemySpawner` (Wave logic).

`Location: Assets/Scripts/`

## 4. Game Systems & Domain Concepts

### Combat & Reflection System
*   `Projectile`: Represents enemy bullets that travel in a direction. It contains a `Reflect` method that changes its owner and speed upon hitting a barrier.
*   `Barrier`: Attached to the player; it uses a `CircleCollider2D` (trigger) to detect projectiles and calculate reflection vectors based on the collision normal.
*   `Enemy`: AI that moves towards or away from the player based on a "keep distance" parameter and fires staggered projectile bursts.

`Location: Assets/Scripts/`

### Level-Up & Progression
*   `PlayerController`: Acts as the data owner for XP and Level. It calculates the `xpToNextLevel` using a multiplier (25% increase per level).
*   `LevelUpUI`: A UI controller that pauses the game (`Time.timeScale = 0`) and presents a choice of three upgrades.
*   `Upgrade Logic`: Upgrades are applied directly to the `PlayerController` properties (e.g., `moveSpeed *= 1.15f`).

`Location: Assets/Scripts/`

### Enemy Spawning
*   `EnemySpawner`: Manages the instantiation of enemy prefabs at randomized positions outside the camera view. It scales difficulty over time by decreasing spawn intervals.

`Location: Assets/Scripts/`

## 5. Scene Overview
*   **TitleScreen**: Entry point. Contains `TitleScreenController`.
*   **Main**: The core gameplay arena. Contains the player, enemy spawner, and HUD.
*   **AudioSystem**: A specialized scene containing the `AudioManager` and `AudioListener`. It is loaded additively and marked with `DontDestroyOnLoad`.
*   **GameOver**: Displayed upon player death. Shows final stats and retry options.

`Location: Assets/Scenes/`

## 6. UI System
The project uses **Unity UI Toolkit (UITK)** for its modern layout and styling capabilities.

*   **UI Structure**: Each screen consists of a `UIDocument` component linking to a `.uxml` file.
*   **Binding**: Logic is handled in C# scripts (e.g., `PlayerHUD.cs`, `LevelUpUI.cs`) which query the root `VisualElement` using `Q<T>()` to bind labels, bars, and buttons.
*   **Styling**: CSS-like `.uss` files handle hover states, animations (using transition classes like `option-card--hidden`), and layout.
*   **Screens**:
    *   `Main`: HUD showing HP and XP progress bars.
    *   `LevelUpUI`: Modal overlay with choice buttons.
    *   `Title_Screen` / `GameOver`: Full-screen menus.

`Location: Assets/UI/`

## 7. Asset & Data Model
*   **Prefabs**: 
    *   `Enemy.prefab`: Base enemy with customizable movement and shooting parameters.
    *   `Projectile.prefab`: Shared projectile with variable speed and sprites.
*   **AudioID**: An enum-based system (`AudioID.cs`) used to reference sounds in the `AudioManager` without hard-coded strings.
*   **Animations**: Uses the Animator component for character walks (`Player_Walk.anim`) and visual effects like `Flame_Death.anim`.
*   **Shaders**: Custom URP shaders for visual juice, including `ColorPulse.shader` for background effects and `Title_Logo_Ripple.shader`.

`Location: Assets/Prefabs/`, `Assets/Shaders/`

## 8. Notes, Caveats & Gotchas
*   **Time Scaling**: The `LevelUpUI` and `DeathRoutine` use `Time.timeScale = 0`. Any logic or animations (like the `AudioManager` or UI transitions) intended to run during these states must use `UnscaledTime`.
*   **Scene Loading**: The `AudioSystem` scene must be present for any sound to play. If running the `Main` scene directly in the editor, ensure the `AudioSceneLoader` is active to additively load the audio system.
*   **Visuals Hierarchy**: The `PlayerController` expects a child object named "Visuals" to handle animations and sprite flipping; if missing, it defaults to the root transform.
*   **Reflection Logic**: Reflection is calculated using the vector from the barrier's center to the projectile. Precise timing is required as the barrier has a short duration and a significant cooldown.