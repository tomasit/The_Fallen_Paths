# Enemy package

## EnemyEventManger

The **EnemyEventManger** class (*Assets/Scripts/Enemy/EnemyEventsManager.cs*) is the parent of all the object <Enemy> you are going to instanciate.
EnemyEventManger take a array of Enemy model (described bellow) : <Enemy[] Enemies>.
This class is going to call the enemy associated scripts in case of behavior switching like : death, movement, target, animation and death.

It will also manager the save for the enemies.

## Animations

The **AnimatorStateMachine** class (*Assets/Scripts/Enemy/AnimatorStateMachine.cs*) is going to manage the animation of each enemies related to their bahevior. You don't need to do anything in order to make this work.

## Model

```cs

[System.Serializable]

public class Enemy {

// Class constructor, no parameters needed

public Enemy() {}

  

// The uuid is for saving data purpose, it's going to be filled automaticaly at the beggining of the game

public string uuid;

// If the enemy is enable he is going to be updated (moving, attacking, etc)

public bool enabled;

// You can choose via the editor which enemy type is it going to be : Random, Guard, RoyalGuard, archer or mage

public EnemyType type;

// You need to assign this object via the Unity Editor. It should be the parent of the enemy.

public GameObject entity;

// All these varaibles are going to be filled automaticaly at the start of the EnemyEventManger script

[HideInInspector] public SpriteRenderer sprite;

[HideInInspector] public Animator animator;

[HideInInspector] public Agent agentMovement;

[HideInInspector] public AEnemyMovement movementManager;

[HideInInspector] public EnemyDetectionManager detectionManager;

[HideInInspector] public BasicHealthWrapper healtWrapper;

[HideInInspector] public EnemyDialogManager dialogManager;

  

// You need to assign theses two variables via the Unity Editor

public RoomProprieties roomProprieties;

public RoomProprieties fleePoints;

}

```

Find this in *Assets/Scripts/Enemy/Enemy.cs*

  

## Agent

  

The IA for 2D pathfining. They are going to have a target (<Transform target>, set in the <AEnemyMovement>) and go for this position.

You don't have to touch at this script. But in you level, you need to add a <NavMeshSurface> Component, which is going to define where the enemy can walk or not. Set the default Area to non walkable.

After this add some colliders with a <NavMeshModifier> marked as Walkable or NoWlakble.

All you need after is to push the bake button on the NavMeshSurface component and you should see blue surfaces, it's where you enemy can move.

  

## AEnemyMovement

  

This class is an abstract class. You cane make your own inherit from it, or take the <ArcherMovementManger>, <GuardMovementManager> or <CivilianMovementManager>.

1. Variables :

The aim of this class is to set a **target** (and give it the the <Agent> class) to the enemy and to activate event triggers (<TriggerCouroutineProcessor> or <TiggerProcessor>) in order to make them interact with the environment.

2. Functions :

The function <void Move()> is called on each fram in <void Update()>.

You have different function for each bahevior of the enmy : <void BasicMovement()> <void AlertMovement()> <void SpotedMovement()> and <void FleeMovement()>. They are abstact in order to make different behavior between enemy types.

The function <bool HasMovedFromLastFrame()> and <int DirectionMovedFromLastFrame()> are here to tell you if the enemy as moved since last frame and in which direction. HasMovedFromLastFrame : on true he has moved, the oposite on flase. DirectionMovedFromLastFrame : 0 no move, 1 moved right, -1 moved left.

  

## EnemyDetectionManager

  

This class will manage the raycast for detection of his target. The class is going to switch the detection state of the enemy depeding of what the raycast detect, what is the state of the enemy and the time.

  

1. Variables :

The **rayCastOffset** variable is to tell where the raycast is going to be shooted, <Vector3.zero> for the middle of the enemy, or adjust it in order to not collide with the own enemy collider.

The **_detectionTarget** is the target the enemy is always looking for. By default it's the player but you can switch it with the <void SetDetectioTarget(Transfrom)> function

The **detectionState** is the detection state of the enemy :

```cs

public enum DetectionState

{

None,

Alert,

Spoted,

Flee,

Freeze,

}

```

The enemy movement manager is based on theses variables

All the **Clocks** of the enemy is the time they spent to detect or to forget their target.

The **debug** show the raycasts in the editor scene view.

  

2. Functions :

The function <void SetDetectionDistance(float)> set the size of the raycats the enemy is going to throw, so the minimum size of the gap between the enemy and his target for not being detected.

The <float GetDetectionDistance()> return the size of the detection raycast throwed by the enemy.

The <void SetDetectionTarget(Tranform)> assign a target where the enemy is going to watch. And if he is already in spoted state, the enemy is going to go to the target.

The <Transform GetDetectionTarget()> return an instance of the transform of the target.

The <void SetState(DetectionState)> assign a new state to the enemy if you want to change it with another way than the **clocks**.

The <DetectionState GetState()> return the current detection state of the enemy.

  

## BasicHealthWrapper

  

Encupsulation of the <class Health> class (*Scripts/Health.cs*).

  

1. Variables :

The <int health> is the current health of the enemy. When it's equal to 0 the enemy dies.

The <int maxHealth> is the health at the beginning of a game of an enemy.

  

2. Functions :

The <void Hit(uint)> function can be call to toggle the animation of the enemy and give him some damage. The uint parameter is the damages given to the player.

  

## EnemyDialogManager

  

This class manage the way the enemy dialogs are going to be displayed : which dialog and when.

For exemple when the enemy detection state become alerted or spoted, a pop-up dialog appaear with the "Alerted" or "Spoted" dialog called in the <TmpDialog dialogs> component (see bellow).

And other dialogs spawn randomly when the detection state is None.

  

1. Variables :

The <TmpDialog dialogs> is the instance of all the dialogs the enemy can display.

The <GameObject popUpPrefab> is the prefab for display a pop up image behind the text and <GameObject dialogPrefab> is the transparent one for simple dialogs.

The <float _timeRandomDialog> is the time to wait for display a new random dialog if there are some sheduled.

The <float _probablityDialog> is the luck for a dialog to pop after _timeRandomDialog time.

The <float _clockDialog> the clock which run for sheduled dialogs.

The <float _durationDialog> is the time a dialog is displayed.

  

2. Functions :

The <IEnumerator PlayRandomDialog(string dialogType, GameObject dialogBox, float lifeTimeDialog)> play a dialog who starts with the dialogType string and is followed by a number (1, 2, 3 etc) as much as there is in the dialogs member variable.

The <IEnumerator PlayThisDialog(string dialogName, GameObject dialogBox, float lifeTimeDialog)> play the dialog called as the dialogName variable. With the dialogBox prefab and is going to stay for lifeTimeDialog time.

The <void SetDialog(string dialogName, GameObject dialogBox, bool instantDialog)> function set a dialog to display. The instantDialog variable tels the script if it is going to be play right know, or if it's going to be sheduled randomly.

By default the enemy shedule a random dialog called "Dialogs X" in the dialogs memeber variable.

  

## RoomProprieties

  

Is just a class with an array of points : <Transform[] targets> where the enemy can go in the room.

If this object is placed in the <RoomProprieties roomProprieties> of the <Enemy> in the <EnemyEventsManager> script, the points are going to be the patern of the enemy when he his oon <DetectionState.None>.

If the object is placed in the <RoomProprieties fleePoints> of the <Enemy> in the <EnemyEventsManager> script, the points are going to be the points where an <DetectionState.Spoted> enemy who is scared by his target is going to go. For exemple if a <EnemyType.Random> is on <DetectionState.Spoted> he is going to alert the guard of the presence of the player, and after he is going go to the fleepoint in the oposite direction of the player.

  

## Usage

  

- Create an empty object.

- Add the EnemyEventManger and an AnimatorStateMachine class into it.

- As childs, add eny enemy prefab you want in your scene (find them in *Assets/Prefabs/Enemy*).

- In the parent add one column for each enemy in the scene.

- Don't touch at the <uuid> variable, choose the <type> of enemy they are, put the root object of each enemy in the <entity> slot.

- And if you want their roomProprieties and fleePoints variables. (If you don't put any of theses two the enmy is going to stay where he is and move if he found a target).