# ~~Nintendo Switch~~ Interface for Citizen Science-Base Telerehabilitation (Citizen-Science-Telerehabilitation)
## Purpose
Fostering a better option for stroke patients in their at-home rehabilitation.
The initial vision was for patients to be able to better progress and feel motivated in completing their own rehabilitation without needing a healthcare provider to assist them through it.
Presently patients can still benefit from the direction of a healthcare provider via the difficulty meter and tracking of patient data.
### Present Contributors
Kora Stewart Hughes (Undergraduate Researcher), 
Roni Barak Ventura (Mentor)
## Full Project Abstract
Stroke survivors commonly suffer from hemiparesis- unilateral muscle weakness that limits limb mobility and encumbers the performance of daily activities. Recovery from hemiparesis requires adherence to a rehabilitation regimen, consisting of high-intensity exercises. While these exercises are often perceived as tedious and boring, the integration of citizen science can motivate patients to adhere to their prescribed regimen. Furthermore, by interfacing rehabilitation with a commercial gaming controller, patients can receive remote feedback from therapists, significantly reducing the temporal and fiscal costs associated with outpatient rehabilitation. In this project, we develop a platform to enable hemiparetic patients to participate in a citizen science project while performing bimanual training. We interface the software with the Nintendo Ring Fit Adventure system and Joy-Con controllers. The Joy-Cons’ built-in accelerometer, gyroscope, and infrared depth sensors gather data on the patients’ movement and heart-rate. These data are then used to control the cursor and camera within the interface. While the game’s difficulty is calibrated relative to the patient’s physical ability and motivation, the patient and therapist can manually adjust the difficulty of the game as they see fit. This affordable platform could benefit not only stroke patients but also the scientific community through the recruitment of citizen-scientists.
## For Future Editors of this Project
Before accessing this project, it was implemented for the Novint Falcon and subsequently the Xbox Kinect. Much of the code from these previous two iterations were preserved throughout the commits however some of the basic functionality was rewritten due to incompatibility with the Nintendo switch/keyboard controls.
### Coding Specifications
Assets/Scenes/PanoScene/Scripts for the bulk of user-created/running code,
Assets/_DS_Store for visual changes,
Assets/UserData for data storage
### Contact Me
khughes@nyu.edu, 
https://www.linkedin.com/in/kora-hughes-80ba19188/
## Other Notes
The default game controller we were planning on initially using was the Nintendo Switch Joycons and RingFit accessory, however, the RingFit was sold out, and, after much effort, Nintendo was unable to give us access to their Switch API - as such we are currently transitioning to a separate device.
In the interim, the project's controls were changed to the keyboard to make navigation during beta-testing easy and migration to another device more smooth.
### Project Specifications
Unity Version 2019.2.8f1 (2019.1.6f1 previously), 
Edited primarily through Visual Studio
