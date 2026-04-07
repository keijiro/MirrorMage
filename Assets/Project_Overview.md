# MirrorMage Technical Project Overview

## 1. Project Description
**MirrorMage** is a 2D top-down action survival game where the player controls a mage who survives not by casting fireballs, but by reflecting them. The core experience centers around a "Perfect Guard" mechanic—using a magical barrier to reflect enemy projectiles back at them. The game features an endless wave-based survival loop, XP-based progression, and a distinct visual style utilizing URP 2D and UITK for its interface.

## 2. Gameplay Flow / User Loop
1.  **Boot & Title**: The player starts at `TitleScreen.unity`, where they can view the logo and start the game.
2.  **Main Loop**:
    *   **Movement**: Player moves toward the mouse cursor.
    *   **Survival**: Enemies spawn and shoot projectiles at the player.
    *   **Reflection**: Player activates a temporary `Barrier` to reflect projectiles. Reflected projectiles become lethal to enemies.
    *   **Progression**: Defeating enemies drops XP, filling the level bar.
    *   **Level Up**: Upon leveling up, the game pauses, and a `LevelUpUI` (UITK) appears, allowing the player to choose one of three upgrades (Move Speed, Charge Speed, or Barrier Strength).
3.  **Game Over**: When health reaches zero, a death sequence plays, transitioning to `GameOver.unity` where the player can restart.

## 3. Architecture
The project follows a component-based architecture with a centralized Singleton for cross-scene services (Audio) and an event-driven flow for UI and progression.

*   **Game Management**: The `PlayerController` acts as the primary orchestrator for player state, health, and XP. Scene transitions are handled by specialized controllers like `GameStartController` and `TitleScreenController`.
*   **Audio System**: Uses a Singleton `AudioManager` that persists across scenes via `DontDestroyOnLoad`. It uses a dictionary-based lookup for `AudioID` to play SFX and BGM through designated `AudioMixerGroups`.
*   **UI Integration**: Uses Unity's **UI Toolkit (UITK)**. Scripts like `LevelUpUI` and `PlayerHUD` bind to `UIDocument` elements to update the interface based on gameplay data.
*   **Input**: Utilizes the **New Input System** to track mouse position and button clicks.

## 4. Game Systems & Domain Concepts

### Combat & Reflection System
A physics-based system where projectiles interact with a circular player barrier.
*   `Projectile`: Handles movement, collision detection, and state tracking (is it reflected?).
*   `Barrier`: A triggered collider that calculates reflection vectors based on the collision normal between the projectile and the barrier center.
*   `Enemy`: AI that moves relative to the player and shoots projectiles in patterns (spread, repeat).
*   **Extension**: Add new projectile types by inheriting from or modifying `Projectile`, or create new `Enemy` prefabs with different `Shoot` parameters.
*   **Location**: `Assets/Scripts/`

### Progression & Upgrade System
A state-machine-like loop that manages player growth.
*   `PlayerController`: Tracks `currentXP`, `level`, and applies stat multipliers.
*   `LevelUpUI`: Manages the UITK-based upgrade screen, pausing the game (`Time.timeScale = 0`) and providing callback actions for chosen upgrades.
*   **Extension**: New upgrades can be added by adding buttons to the `LevelUpUI.uxml` and registering new callback methods in `LevelUpUI.cs`.
*   **Location**: `Assets/Scripts/`

### Audio Management
A centralized system for playing sounds using enums and serialized data.
*   `AudioManager`: Singleton manager that handles `AudioSource` pooling (via specific sources for SFX/BGM) and `AudioMixer` routing.
*   `AudioID`: An enum that acts as a strongly-typed key for audio clips.
*   **Design Pattern**: Singleton Pattern.
*   **Location**: `Assets/Scripts/`

## 5. Scene Overview
*   **TitleScreen**: Entry point. Displays the background and logo with ripple shaders. Uses `TitleScreenController`.
*   **Main**: The primary gameplay scene. Contains the `Player`, `EnemySpawner`, and the level bounds.
*   **AudioSystem**: A bootstrap scene or additive scene used to initialize the `AudioManager`.
*   **GameOver**: Displayed upon player death. Shows final stats and offers a retry option.

## 6. UI System
The project uses **UI Toolkit (UITK)** for all menus and HUD elements.
*   **Framework**: UITK (`.uxml` for structure, `.uss` for styling).
*   **HUD**: `PlayerHUD.cs` (or logic within `PlayerController`) updates health and XP bars in real-time.
*   **Menus**: `LevelUpUI` uses classes like `option-card--hidden` and `option-card--selected` to trigger USS-based transitions and animations.
*   **Logic**: UI scripts find elements using `rootVisualElement.Q<T>(name)` and subscribe to events like `.clicked`.
*   **Location**: `Assets/UI/` and `Assets/Scripts/`

## 7. Asset & Data Model
*   **Prefabs**: Enemies (`Enemy.prefab`, `Enemy_Bat.prefab`) and projectiles are stored as prefabs for the `EnemySpawner` and `Enemy` scripts to instantiate.
*   **AudioData**: Audio clips and their settings (volume, reverb) are defined in a serialized list within the `AudioManager` inspector.
*   **ScriptableObjects**: URP settings and Volume Profiles define the visual look, including bloom and color grading for the death sequence.
*   **Naming Conventions**:
    *   Scripts: PascalCase (e.g., `PlayerController.cs`).
    *   Prefabs: PascalCase (e.g., `Enemy_ZombieMage.prefab`).
    *   Sprites: Category_Description (e.g., `Bullet_Red.png`, `Icon_MoveSpeed.png`).

## 8. Notes, Caveats & Gotchas
*   **Time Scaling**: The `LevelUpUI` and `DeathRoutine` manipulate `Time.timeScale`. UI animations and the death sequence use `UnscaledTime` to ensure they continue playing while the game world is paused.
*   **Collision Layers**: Reflection logic depends on the `Projectile` having a `CircleCollider2D` and being on a layer that interacts with the `Barrier`'s trigger.
*   **Visuals Sub-Object**: `PlayerController` expects a child object named "Visuals" to handle animations and sprite flipping independently of the root transform's logic.
*   **UI Layout**: Ensure `PanelSettings` in `Assets/UI/` is correctly assigned to all `UIDocument` components to maintain consistent scaling across resolutions.