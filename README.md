# NOVA Line Tower Defense

A comprehensive 3D tower defense game built with Unity, featuring procedural wave generation, strategic turret placement, and immersive sci-fi environments.

## Screenshots

### Main Menu
![Main Menu](Assets/Screenshots/MainMenuScreenshot.png)

### Gameplay
![Gameplay Screenshot](Assets/Screenshots/GamePlay1Screenshot.png)

![Gameplay Screenshot 2](Assets/Screenshots/GamePlay2Screenshot.png)

### Pause Menu
![Pause Menu](Assets/Screenshots/PausedMenuScreenshot.png)

### Instructions
![Instructions UI](Assets/Screenshots/InstructionsScreenshot.png)

## Overview

NOVA Line Tower Defense is a sophisticated tower defense game that challenges players to defend their base against waves of increasingly powerful enemies. The game combines strategic resource management, tactical turret placement, and real-time combat mechanics in a polished sci-fi setting.

## Key Features

### Core Gameplay Systems

**Wave Management System**
- Procedural wave generation with dynamic difficulty scaling
- Boss battles every 5th wave with enhanced rewards
- Progressive enemy health and count increases based on exponential difficulty curves
- Randomized spawn timing to prevent predictable patterns
- Support for both procedural and pre-designed wave configurations

**Strategic Turret Placement**
- Three distinct turret types with unique capabilities and costs
- Real-time placement preview with visual feedback (green/red tinting)
- Collision detection preventing placement in invalid locations
- Keyboard shortcuts (1, 2, 3) for rapid turret selection
- Dynamic cost validation with real-time affordability checks

**Advanced Combat Mechanics**
- Multi-barrel turret systems with staggered firing patterns
- Hitscan damage system for instant hit registration
- Intelligent target acquisition with configurable targeting priorities
- Missile turrets with predictive targeting and physics-based projectiles
- Visual bullet trails and impact effects

### Enemy AI & Progression

**Intelligent Enemy Behavior**
- NavMesh-based pathfinding for realistic movement
- Four distinct enemy types with varying health and characteristics
- Distance-traveled tracking for advanced AI behaviors
- Ragdoll physics system for realistic death animations

**Dynamic Difficulty Scaling**
- Exponential health multipliers (1.2x per wave)
- Progressive enemy count increases (base + wave * 0.75)
- Boss enemies with 2.5x health multipliers
- Balanced reward system scaling with difficulty

### User Interface & Experience

**Comprehensive UI System**
- Real-time currency and life tracking
- Wave progress indicators with boss wave highlighting
- Interactive main menu with instructions panel
- Game over screen with performance statistics
- Pause system with complete game state management

**Audio Integration**
- Centralized audio management system
- Separate volume controls for music and sound effects
- Turret-specific shooting sounds
- Missile thrust and explosion audio feedback

**Input Management**
- Mouse-based turret placement and camera controls
- Right-click camera rotation (disabled during pause/game over)
- Keyboard shortcuts for efficient gameplay
- ESC key functionality for menu navigation

### Technical Architecture

**Performance Optimization**
- Singleton pattern implementation for manager classes
- Efficient object pooling for bullets and effects
- Optimized collision detection using LayerMask systems
- Coroutine-based wave spawning for smooth performance

**Code Quality Features**
- Comprehensive error handling and null checks
- Modular component-based architecture
- Event-driven communication between systems
- Clean separation of concerns across game systems

**Data Management**
- ScriptableObject-based tower configuration system
- PlayerPrefs integration for persistent settings
- Resource management system with validation
- Extensible enemy and wave data structures

## Game Balance & Progression

### Economy System
- Kill rewards: 3 + (current wave) currency per enemy
- Boss kill bonus: 2x normal reward
- Wave completion rewards: 30-150 currency depending on wave type
- Progressive wave bonuses: +5 currency per wave level

### Turret Specifications
- **Basic Turret**: 100 currency, rapid-fire capability
- **Advanced Turret**: 600 currency, higher damage output
- **Missile Turret**: 400 currency, area damage with predictive targeting

### Enemy Scaling
- **Regular Enemies**: 55-75 base health (Types 1-3)
- **Boss Enemies**: 380 base health with exponential scaling
- Health multiplier: 1.2^(wave-1) for progressive difficulty

## Technical Implementation

**Engine**: Unity 6000.1.8f1 with Built-in Render Pipeline  
**Programming Language**: C# with modern language features  
**Architecture Pattern**: Component-Entity-System with Manager singletons  
**Physics**: Unity Physics with NavMesh integration  
**Audio**: Integrated AudioSource management with volume controls  
**UI Framework**: Unity UI (uGUI) with TextMeshPro integration  

## Development Highlights

This project demonstrates proficiency in:
- Advanced Unity development patterns and best practices
- Complex game state management and UI systems
- Performance optimization and memory management
- Modular architecture design for scalability
- Mathematical algorithms for game balance and difficulty progression
- Audio system integration and management
- Input handling and user experience design

## Future Enhancement Possibilities

- Multiplayer support with networking
- Additional turret types and upgrade systems
- Procedural map generation
- Achievement and progression systems
- Mobile platform optimization
- Steam Workshop integration for custom content

## Installation & Building

The game is designed for Windows PC deployment through Unity's build system. All assets are self-contained within the Unity project structure, utilizing both custom implementations and carefully selected Asset Store packages for enhanced visual quality.

---

*This project represents a complete, polished tower defense experience showcasing advanced Unity development skills and comprehensive game design principles.*