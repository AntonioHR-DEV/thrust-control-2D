# 🚀 Thrust Control

A physics-based 2D lunar lander game built in Unity 6. Navigate your spacecraft through increasingly challenging terrain — manage your fuel carefully, control your descent angle, and touch down safely before time runs out.

**[▶ Play on Itch.io](https://antoniohr-dev.itch.io/thrust-control)**

---

## Gameplay

![Thrust Control Gameplay](Screenshots/gameplay.gif)

---

## Features

- Main Menu and Level Selection Scene
- Physics-based Lander thrust and rotation
- Multiple levels with increasing difficulty and different mechanics (laser beams, gates, force fields)
- Fuel management system with low-fuel warning effects such as pulsing low fuel message and a chromatic abberation pulse post-processing effect
- Coin pickups and fuel refill stations
- Scoring based on landing speed, angle, collected coins and remaining time
- Star based game rating after a completed level
- Laser hazards
- Cinemachine camera with screen shake on crash
- Dynamic 2D lighting on the lander and landing pads
- VFX (Particle System) on Lander Thrust, Explosion, Coin and Fuel Pickup
- Twinkling starfield shader built in Shader Graph
- Touch controls for mobile (with keyboard support on PC)
- Options UI that supports sound effects/music volume change and key rebindings

---

## Controls

| Action | Keyboard | Mobile |
|---|---|---|
| Thrust | `↑` | Thrust button |
| Rotate Left | `←` | Left button |
| Rotate Right | `→` | Right button |
| Pause | `Escape` | — |

---

## Built With

- **Unity 6.3 LTS** — URP 2D
- **Cinemachine** — camera follow and impulse-based screen shake
- **Shader Graph** — custom starfield twinkling effect
- **Unity New Input System** — rebindable controls
- **C#**

---

## Credits

### Art
- **Rock terrain** — "Hand Painted Tiling Textures" by beefpuppy
  Licensed under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)
  https://beefpuppy.itch.io/hptt

- **Coin sprite** — by kawnami1022
  https://kawanami1022.itch.io/coin

- **Space background** — "Seamless Space Backgrounds" by Screaming Brain Studios
  https://screamingbrainstudios.itch.io/seamless-space-backgrounds

### UI
- **UI Pack** — Kenney (no attribution required, credited with appreciation)
  https://www.kenney.nl

### Music
- **"Breach of the Voidline" (Loop)** — by Alkakrab
  https://alkakrab.itch.io/sci-fi-music-pack-vol-3
