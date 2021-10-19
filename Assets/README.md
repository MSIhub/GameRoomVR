# Setup

### Allow 'unsafe' Code
1. Edit -> Project Settings -> Player -> Other Settings
	=> enable "Allow 'unsafe' Code"

### Add Package Dependencies
1. Add the following Packages in the Packagemanager:
	* Input System
	* OpenXR Plugin

### Setup OpenXR
1. Project Settings-> XR Plug-in Managment->
	* Select OpenXR
	-> Click on the Red icon next to it and let it fix all issues
2. Project Settings-> XR Plug-in Managment->OpenXR->Features
	* Select the controllers you want to support ( tested with HTC Vive Controller and Valve Index Controller, Oculus Quest 1)
	
### Create an AppId
1. Go to https://dashboard.photonengine.com/
2. Select "Create A New App"
3. Select Photon Type Fusion, and enter the name, etc. and create.
4. Copy the created AppId.
5. Select Assets/Photon/Fusion/Resources/PhotonAppSettings.asset in the project.
6. Paste the AppId into "App Id Realtime"

# Hello-Fusion-VR

## Starting Scene
Load the starting scene ("VR_Start") to launch the game. This scene will start Fusion in Singleplayer-mode. This way all the interactions work without any special non-networked code. Select one of the buttons to start as Host or Client and load the proper Game scene. 

## The Player
Hand.cs: Updates Pose according to input. Handles grabbing / dropping of interactables.

Interactable.cs: A grabbable object. Handles velocity tracking towards hand while grabbed.

HighlightCollector.cs: Attached to hand. Uses OnTriggerEnter / Leave to collect Highlightable objects.

Highlightable.cs: Component being searched for by HighlightCollector. Also enables / disabled visual highlight.

SpawnLocalPlayer.cs: Gives user the option to spawn VR or PC rig ( usefull for multiplayer testing on one machine )

## The Local Rigs
PlayerInputHandler.cs: Collects the input and sends it on to Fusion.
LocalController.cs: writes the user input into the InputData according to Actions assigned

PCInput.cs: A Debug PC input script to test multiplayer features more easily. This does not handle all features a Controller / VR player could and only has one hand.

## Input
The input struct ( found in InputData.cs ) is seperated into the main struct ( InputData ) and the indiviual hands ( InputDataController ).
Position and Rotation are saved as is in local space.
Buttons are sorted into two categories: "Actions" and "States"

* "Actions" are One-time actions such as, button down events, or Teleport.
	To avoid loosing input in the case of a packetloss we switch the bits whenever the action is performed and then check uppon receiving if the bit has changed.
	After preprocessing the Actions the bit is 1 for the tick when the action was performed.
	The previous Actions need to be saved as [Networked] for prediction and resimulation to work.
* "States" are contiuous actions such as Teleport Mode is activated. The bit is 1 as long as the button is held.