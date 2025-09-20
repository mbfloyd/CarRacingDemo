# takehome-unity

Take-home test for Unity engineering candidates

Feature: Offline Practice Mode with Ghost Racer
 
Overview
Add a new Offline Practice Mode to the existing Off-Road Racing Game template. Players should be able to race against their own best previous performance (ghost racer).
 
🛠️ Requirements

Mode Accessibility
- Add an "Offline Practice Mode" entry to the main menu.

Ghost Racer
- Record and save the player’s best race performance.
- Display a visual "ghost" of the player's fastest race so they can race against it in future sessions.

UI Integration
- Show the current lap/race time.
- Clearly indicate when a ghost racer is active.

Data Storage
- Ghost race data should persist across sessions.


======================

Completed Assignment was contained to the OfflinePracticeMode folder for easier Reference.

I implemented a Dependency Injection approach, to add the new features without creating new direct dependencies.

The practice data is stored in an instance of PracticeLevelData.

This is loaded and saved through the DataManager via the resolution of the IOfflinePracticeAPI.

Currently, the Bootstrapper sets up the IOfflinePracticeAPI to resolve with OfflinePracticeAPIJson. 

This can be updated to use a backend API by creating the new backend API class with implementing the IOfflinePracticeAPI interface and updating the Register in the Bootstrapper.

I maintained the existing design and implementation by adding the PracticeRacer and GhostRacer components to the existing Player Prefabs. They each have their own setup methods that configure the prefab instance for their own needs. This allowed me to add the new features and still used all the existing tracks and cars without additional setup.

The Race Start and End are handled using an EventManager. The PracticeRacer and GhostRacer register to listen for these events, which in turn is triggered by the Race_Manager when the race starts and ends.



