# Project Overview: MirrorMage

MirrorMage is a top-down survivor-style action game where the player controls a mage who survives not by casting spells, but by reflecting them. The core experience revolves around timing a magical barrier to deflect incoming enemy projectiles back at them, growing in power through an XP-based level-up system.

## 1. Project Description
MirrorMage is a 2D bullet-reflection survival game. The player must navigate a ritual hall, avoiding enemies and using a temporary shield to parry projectiles. Success depends on precise timing and strategic movement to line up reflected shots with approaching threats.

**Core Pillars:**
- **Reflect over Attack:** Combat is reactive; the player's primary weapon is the enemy's own projectiles.
- **Timing & Precision:** The barrier has a limited duration and a cooldown, requiring careful usage.
- **Scaling Power:** Defeated enemies grant XP, leading to upgrades that enhance speed, cooldowns, and barrier size.

## 2. Gameplay Flow / User Loop
1.  **Boot/Start:** The game starts in the `Main` scene.
2.  **Survival Loop:**
    -   **Movement:** Player follows the mouse cursor.
    -   **Combat:** Enemies spawn and fire at the player.
    -   **Reflection:** Player activates the barrier (Left Click) to deflect bullets.
    -   **XP Collection:** Defeated enemies automatically grant XP.
3.  **Progression:** Reaching XP thresholds triggers the Level Up UI, pausing the game.
4.  **Death/Game Over:** If health reaches zero, the player is destroyed (currently logs "Player Died").

## 3. Architecture
The project follows a component-based architecture where individual MonoBehaviours handle specific domain logic. Coordination is achieved through direct references or tag-based lookups.

- **Central Hub:** `PlayerController` acts as the primary state holder for health, XP, and ability status.
- **Input:** Utilizes the Unity Input System (Package) via `Mouse.current`.
- **UI Interaction:** `UIDocument` and C# scripts bridge the game state to the UI Toolkit (UITK) elements.
- **Physics:** Relies on `CircleCollider2D` triggers for reflection and damage detection.

`Location: Assets/Scripts`

## 4. Game Systems & Domain Concepts

### Movement System
Handles player and enemy translation. The player uses a smooth "follow mouse" approach, while enemies use a direct "track player" logic.
- `PlayerController`: Translates the player towards the world-space mouse position.
- `Enemy`: Simple tracking movement towards the `Player` tag.
- `Location: Assets/Scripts`

### Combat & Reflection System
The defining mechanic where the player's `Barrier` interacts with `Projectile` objects.
- `Barrier`: Manages the activation state, visual pulsing, and the physics trigger that reflects bullets.
- `Projectile`: Handles linear movement. When reflected by a barrier, it calculates a reflection vector using the barrier's normal and increases its speed.
- `Enemy`: Handles shooting patterns (spread shots) and death logic.
- `Location: Assets/Scripts`

### XP & Leveling System
A meta-progression loop that pauses gameplay to offer stat upgrades.
- `PlayerController`: Tracks XP and triggers the level-up event.
- `LevelUpUI`: Manages the UI Toolkit overlay, button bindings, and game pausing (`Time.timeScale = 0`).
- `Location: Assets/Scripts`

## 5. Scene Overview
The project currently operates within a single-scene structure.
- **Main**: Contains the `Player`, `EnemySpawner`, `Global Volume` (for URP post-processing), and the `UI` hierarchy.
- **Flow**: The scene is intended to be the main gameplay loop. Transitioning to other levels is not currently implemented.

## 6. UI System
MirrorMage uses **Unity UI Toolkit (UITK)** for its interface.
- **PlayerHUD**: A screen-space overlay showing Health and XP progress.
- **LevelUpUI**: A modal overlay (`Main.uxml`) that appears during level-up events. It uses USS classes (`.option-card--hidden`, `.option-card--selected`) for animations.
- **CooldownBar**: A world-space Sprite-based UI attached to the player to show barrier availability.
- **Binding**: UI elements are found via `rootVisualElement.Q<T>(name)` and bound to C# callbacks in `Awake`.

`Location: Assets/UI`

## 7. Asset & Data Model
- **Prefabs**: Core entities like `Enemy`, `Projectile`, and `Flame_Death_Effect` are prefabs for easy spawning.
- **Visuals**: Uses 2D Sprites with a dedicated `Visuals` child object on the Player to separate logic from rendering/animation.
- **Animations**: Standard `AnimatorController` setups for walking and death effects.
- **Rendering**: Powered by URP 2D Renderer with a custom `ColorPulse.shader` for background effects and `EightColorController` for post-processed look.

## 8. Notes, Caveats & Gotchas
- **TimeScale**: The Level Up system sets `Time.timeScale = 0`. UI animations must use `unscaledTime` or `WaitForSecondsRealtime` to function during a pause.
- **Visuals Flip**: Sprite flipping is handled in `PlayerController` by comparing current and last frame positions; if the movement is too small, flipping might jitter.
- **Projectile Cleanup**: Projectiles are destroyed if they exceed a distance of 50 units from the origin, regardless of camera view.
- **Barrier Scaling**: Upgrading Barrier Strength increases the actual `transform.localScale` of the barrier object, which also increases its collision radius.