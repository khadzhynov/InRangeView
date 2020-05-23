# InRangeView
InRange indication prototype

## Problem
We have several units on some gameplay field. Each unit has "range" parameter defined. Unit can be moved to any position inside range. Range is not just circular area - it indicates distance, which unit can pass. And there are not-walkable obstacles.

How to show to the player, which point on the field is reachable right now, and which is not?

## Possible solutions:
### 1 - Raycast colliders
#### Perform analyzis
We can analyze field colliders with raycasts, performed initially in all 360 directions around. Meeting the obstacle, we should recursively raycast from all its corners, keeping in mind total rays length sum. In this way, we can achieve an area border, consists from list of points.

#### Represent result to player
We can procedurally build a mesh, covering all avaliable points, and present it to the player.

### 2 - Split area and use NavMesh
#### Perform analyzis
We can split circular range into many points, then use NavMesh CalculatePath functionality for every point. In this way, we can get a matrix of all available points inside given range.

#### Represent result to player
We can put a particles in all of matrix points.

## Implementation
Solution #2 was choosen for prototype, because its far less expensive in terms of human hours for implementation. Weakest point of this solution is performance (while solution #1 can potentially be a way more efficient in performance). It turns out, NavMesh is powerfull enough to process up to 7000 points at frame on desctop (Intel Core i5 7th gen), keeping framerate acceptable. 

### Possible optimisation solutions
1. Needs to test, if raycasting each point against colliders and excluding overlaped points would be more efficient, than NavMesh call.
2. Perform NavMesh calls not every frame, but when necessary.
3. Async calculations.

### Conclusion:
Solution #2 is fully acceptable for desktop games, while not used in realtime, but only updated on unit selection.


### WebGL demo
https://bryarey.itch.io/inrange-unity-demo

### YouTube video
https://youtu.be/imVBSzPDit0
