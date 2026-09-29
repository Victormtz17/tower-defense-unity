# Tower Defense (Unity/C#)



A tower defense prototype built in Unity, 
<img width="1920" height="976" alt="tdnormal" src="https://github.com/user-attachments/assets/d8f6777e-ba57-4922-b16e-52690f56f170" />
<img width="1920" height="976" alt="giff con flecha" src="https://github.com/user-attachments/assets/9e9d97d0-eb86-4c39-bbc6-8966bfde515e" />

## Features
- BFS pathfinding that reroutes live as walls/towers are placed
- Laser tower (continuous beam, DPS) and mortar tower (real parabolic
  ballistics, splash damage) sharing an abstract base class
- Data-driven enemy waves and scenarios (ScriptableObject-based),
  with win/lose conditions and difficulty cycling
- Enemy animation via a custom PlayableGraph system (bounce, intro/outro,
  blended transitions)

## Built with
Unity 6, C#, Universal Render Pipeline
