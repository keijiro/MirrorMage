I have investigated the project's source code, architecture, and asset structure. This Unity project, **MirrorMage**, is a 2D top-down action game where the player survives waves of enemies by reflecting projectiles using a magical barrier.

# MirrorMage Technical Overview

## 1. Project Description
**MirrorMage** is a retro-styled 2D survival action game. The player controls a mage who cannot attack directly; instead, they must time the activation of a magical barrier to reflect enemy projectiles back at them. The core experience is defined by:
- **Reflective Combat:** A "parry-first" loop where projectiles become the player's primary weapon.
- **Vampire-Survivors Style Progression:** Players collect XP from defeated enemies to level up and choose permanent stat upgrades.
- **Retro Aesthetic:** Uses a restricted 8-color palette (via `KinoEight`) and dithered shaders for a Lo-Fi feel.

## 2. Gameplay Flow / User Loop
1.  **Boot & Title:** The game starts in the `TitleScreen` scene. The `TitleScreenController` manages the UITK-based menu.
2.  **Core Loop:**
    *   **Movement:** The player follows the mouse cursor via `PlayerController`.
    *   **Defense/Offense:** Enemies (`Enemy.cs`) spawn via `EnemySpawner` and fire projectiles. The player activates the `Barrier` (Left Click) to reflect bullets.
    *   **XP & Leveling:** Reflected bullets kill enemies, dropping XP. Collecting enough XP triggers the `LevelUpUI`.
3.  **Progression:** As the game continues, `EnemySpawner` increases the difficulty (frequency/types).
4.  **Game Over:** Currently, death is logged, and the player can restart by returning to the title or reloading the scene.

## 3. Architecture
The project follows a **Component-Based Pattern** with centralized managers in the scene.

### Entry Points & Core Managers
- **`PlayerController`:** The central hub for player state (Health, XP, Input) and barrier management.
- **`EnemySpawner`:** Handles the spawning logic for different enemy prefabs at randomized intervals.
- **`LevelUpUI`:** Manages the game-state pause and upgrade selection via UITK.

### Design Patterns
- **Observer Pattern (Implicit):** The `LevelUpUI` and `PlayerHUD` query the `PlayerController` for state changes.
- **State Machine (Internal):** Enemies use Coroutines (`BehaviorRoutine`) to transition between "Moving" and "Shooting" states.
- **Physics-Based Reflection:** `Barrier` uses `OnTriggerEnter2D` and `Vector2.Reflect` to calculate return trajectories for projectiles.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Movement System
- `PlayerController`: Uses `Vector3.MoveTowards` to glide toward the world-space mouse position.
- `Enemy`: Uses randomized distances and `keepDistance` thresholds to maintain spacing from the player.

### Reflection & Projectile System
- `Projectile`: A simple transform-based movement script that handles its own collision with enemies (when reflected).
- `Barrier`: The core defensive tool. It calculates the surface normal based on the relative position of the hit projectile and triggers the `Reflect` method.

### Leveling & Upgrade System
- `LevelUpUI`: Pauses the game by manipulating `Time.timeScale` and presents options to modify `PlayerController` variables (Move Speed, Barrier Duration, etc.).

`Location: Assets/Scripts`

## 5. Scene Overview
- **`TitleScreen.unity`:** Contains the main menu, `PulseEffect` for logo juice, and handles the transition to the game scene.
- **`Main.unity`:** The primary gameplay arena. It contains the `Player`, `EnemySpawner`, `Global Volume` (for post-processing), and the `UIDocument` for HUD and Level-Up screens.

## 6. UI System
The project uses **UI Toolkit (UITK)** for all overlay interfaces.
- **`LevelUpUI`:** Uses a UXML layout (`LevelUpUI.uxml`) and USS (`LevelUpUI.uss`). It features animated "Option Cards" that slide in/out using USS classes.
- **`PlayerHUD`:** Provides real-time feedback on Health and XP using progress bar elements.
- **`CooldownBar` (World Space):** A unique non-UITK element. It uses a `SpriteRenderer` on a child of the Player to show the barrier's cooldown directly in the game world.

`Location: Assets/UI`

## 7. Asset & Data Model
- **Prefabs:** Entities like `Enemy`, `Projectile`, and `Flame_Death_Effect` are stored in `Assets/Prefabs`. The `Enemy` prefab is modular, allowing for different `AnimatorControllers` (Bat, Ghoul, Skeleton) to be swapped.
- **Animations:** Standard `Animator` components. The `PlayerController` manually flips the `SpriteRenderer` based on movement delta to save on redundant animation clips.
- **Rendering:** Uses URP with a **2D Renderer**. A `Volume` component carries the `KinoEight` effect to apply the 8-color palette globally.

`Location: Assets/Prefabs`, `Assets/Animations`

## 8. Notes, Caveats & Gotchas
- **Barrier Logic:** The barrier reflection uses `(other.transform.position - transform.position).normalized` as a pseudo-normal. This works because the barrier is circular; if the barrier shape changes to a square, this math will need to be updated.
- **Visuals Child:** The `PlayerController` looks for a child named "Visuals". If renamed or moved, the flipping and sprite-flashing logic will default to the root, which might not have the `SpriteRenderer`.
- **Scaling:** XP requirements scale by 1.25x per level. This is hardcoded in `PlayerController.LevelUp()`.
- **Sorting Layers:** Ensure all sprites are on the `Default` sorting layer with appropriate `Order in Layer` to prevent Z-fighting, as the project uses a flat 2D plane.