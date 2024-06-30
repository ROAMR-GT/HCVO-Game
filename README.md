# Human-Centric Velocity-Obstacle Algorithm And VR Experimental Setup Codebase

Corresponding Author: Aakash Bajpai, abajpai1@umbc.edu, aakash.bajpai@jhuapl.edu
Feel free to reach out with questions or assistance in setup!

Georgia Tech DART & EPIC Labs

Co-authors: Alexander Lu, Kevin Choi, Rajan Tayal, Aaron Young, Anirban Mazumdar

## About
Code in support of ACM THRI Submission: Improving Human Situational Awareness and Planning using a Human Centric Velocity Obstacle Algorithm

YouTube Video: https://www.youtube.com/watch?v=9ITD1GBBz24

The structure of this repo is the default structure of Unity games. The Assets folder notably contains Images, Materials, Scenes, and SteamVR plugin support.

##System requirements:
Unity Engine 2019 and up
HTC VIVE Pro with 4 Lighthouse base stations (4 corners of a 4.5m^2)
HTC VIVE Wireless kit (available PCI slot on VR computer needed)
Recommended VIVE Pro Computer Specs: https://www.vive.com/us/support/vive-pro/category_howto/what-are-the-system-requirements.html

##### Experimental Computer Used:
Dell Precision 3630, Windows
Nvidia Quadro P400
Intel i7-8700 CPU @ 3.2GHz
32GB RAM

## Running Experiments
Once downloading and installing the appropriate Unity version, you will need to install the appropriate plugins from the Unity Store. We note that all plugins/assets used are free for this experimental setup. Follow a tutorial such as this [one]( https://youtu.be/iJ0oNYIUFJo) to get set up.

Trials are mostly autonomous. Using the Unity Play button you may track both what an individual sees in the game view and a 3d person view of the field. This is how videos were made of the game (by screen recording with OBS studio).

Open the Game.Unity file under scenes. Solve any errors that may pop up on your system. The MainMenu game object is a blank object that contains the MainMenu.cs script which houses an array of public variables which other scripts call.

There are also other useful variables being tracked in MainMenu such as the current failure rate, trial time, time till next obstacle set activation.

- Runing Control: Ensure all condition Booleans are off (no checkmark)
- Running Drone View: Check on Drone_View
- Running HCVO: Check on VO_view - ensure use_tuning bool in VO.cs is true
- Running VO: Check on VO_view - ensure that use_tuning bool in VO.cs is false
- Changing the number of threats: NumberOfThreats is defaulted to 1 but can be set up to 4.
Data is logged to the CSV folder.

## Key Scripts
Under Assets -> Scripts there are an array of .cs files of C++ code to run the game engine. Notable ones are listed below with their uses:

- MainMenu.cs - Control station that enables minimal code changes when running experiments. Uses public variables that can be called by other scripts.
- VO.cs - Contains code to process obstacle, goal, and player state into an HCVO and/or a VO output.
- VO_I_visualization.cs - A visualizer that is displayed to participants. It is a white ring with 60 increments (6 degrees each) which appear white when that directional action is safe and disappear when it is not safe.
- DroneView.cs - A set of masks that enable obstacles, goal, and user to be seen from a top down camera in the sky (representing a simulated drone).
- PlayerMovement.cs - Switch between different tracking methods to move the player.
- CreateObstacle - Logic to create obstacles and turn them on to move on paths of potential collision.
- TargetRing.cs - Creation of the goal object.
- TargetAreaBehavior.cs - Logic that enables the goal to move around in a pseudo random fashion with the goal of keeping people constantly moving.
- Health.cs - Notes the current health of the individual with a health bar and value.
- HitDetectedTextBehavior - Gives the hit detected warning.
- DistanceTextBehavoir - Controls the color and number of dashes to represent how far an individual is from the goal.
- HUDBehavior - Transforms the canvas that has all the notifications, health, and perceptual augmentations to the head of the user to simulate a heads up display.
- Logging.cs - Creating of rows of data for every time step and logging to file for post processing.
