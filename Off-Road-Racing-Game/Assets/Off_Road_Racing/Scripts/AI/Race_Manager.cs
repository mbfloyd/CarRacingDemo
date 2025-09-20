//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ALIyerEdon;
using System;
using UnityEngine.SceneManagement;

namespace ALIyerEdon
{
    public class Race_Manager : MonoBehaviour
    {
        private class Racer_Position
        {
            public int ID;
            public string Name;
            public float Position;
        }

        [Header("Options ____________________________________________________" +
            "____________________________________________________")]
        [Space(5)]
        public int levelID = 0;
        public bool showLocalPosition = false;

        [Header("Race Start ____________________________________________________" +
            "____________________________________________________")]
        [Space(5)]
        public float timeScale = 1f;
        int counterNumbers = 3;
        public int totalLaps = 3;
        [HideInInspector] public GameObject startCounter;

        [Header("User Interface ____________________________________________________" +
            "____________________________________________________")]
        [Space(5)]
        public GameObject startUI;
        public GameObject raceUI;
        public GameObject raceFinishUI;
        public GameObject positionUI;
        public GameObject mobileControls;
        public KeyCode startKey = KeyCode.F;

        [Header("Player Info ____________________________________________________" +
            "____________________________________________________")]
        [Space(5)]
        public UnityEngine.UI.Text playerInfo;
        public UnityEngine.UI.Text lapInfo;
        public UnityEngine.UI.Text[] racerInfo;


        // Racers info class    
        List<Racer_Position> positions = new List<Racer_Position>();
        List<Racer_Position> sortedPositions = new List<Racer_Position>();

        [Header("Racing Elements ____________________________________________________" +
            "____________________________________________________")]
        [Space(5)]
        // Name of the each racer in order
        [HideInInspector] public string[] racerNames;

        // Player cars to spawn at the spawn points
        public GameObject[] playerPrefabs;
        GameObject playerPrefab;

        // Racer cars to spawn at the spawn points
        public GameObject[] racerPrefabs;

        // Spawn point for each racer in order
        public Transform[] spawnPositions;

        Car_Position[] carPositions;

        Car_Position playerPosition;

        [HideInInspector] public bool raceStarted;

        bool dontGetKey = false;
        string playerName = "Player";
        bool canStart;

        private IDataManager dataManager;
        private ITypeEventManager eventManager;

        private void Awake()
        {
            eventManager = DIContainer.Instance.Resolve<ITypeEventManager>();
            dataManager = DIContainer.Instance.Resolve<IDataManager>();
        }

        IEnumerator Start()
        {
            Time.timeScale = timeScale;

            if (PlayerPrefs.GetInt("Target FPS") > 25)
            {
                Application.targetFrameRate =
                    PlayerPrefs.GetInt("Target FPS");
            }

            if (startUI)
                startUI.SetActive(false);
            if (raceUI)
                raceUI.SetActive(false);
            if (mobileControls)
                mobileControls.SetActive(false);


            FindFirstObjectByType<Start_Counter>().timeScale = timeScale;

            // Initial info
            carPositions = new Car_Position[racerPrefabs.Length];
            racerNames = new string[racerPrefabs.Length];

            if (dataManager.GetRaceMode() == RaceMode.OfflinePractice)
            {
                SetupOfflinePracticeRacers();
            }
            else
            {
                SetupRacers();
            }

            playerName = GameObject.FindGameObjectWithTag("Player").GetComponent
                <Car_Position>().RacerName;
            //_________________________________

            // Find car position component of the player car to update UI text info (position + lap)
            playerPosition = GameObject.FindGameObjectWithTag("Player").
                GetComponent<Car_Position>();

            startCounter = FindFirstObjectByType<Start_Counter>().gameObject;

            GameObject.FindGameObjectWithTag("Player")
                .GetComponent<EasyCarController>().Clutch = true;

            FindFirstObjectByType<InputSystem>().canControl = false;

            yield return new WaitForSeconds(3 * timeScale);
            
            startUI.SetActive(true);
            canStart = true;

            Update_Positions_Display();
        }

        private void SetupRacers()
        {
            // First racer is the player's prefab
            GameObject playerPrefab = playerPrefabs[PlayerPrefs.GetInt("CarID")];
            racerPrefabs[0] = playerPrefab;

            // Instantiate racers and prefabs
            for (int i = 0; i < racerPrefabs.Length; i++)
            {
                string racerName = racerPrefabs[i].GetComponent<Car_Position>().RacerName;
                SetupRacer(i, racerName, racerPrefabs[i], spawnPositions[i]);
            }
        }

        private void SetupOfflinePracticeRacers()
        {
            // Instantiate player
            GameObject playerPrefab = playerPrefabs[PlayerPrefs.GetInt("CarID")];
            string racerName = playerPrefab.GetComponent<Car_Position>().RacerName;
            GameObject practiceRacerGO = SetupRacer(0, racerName, playerPrefab, spawnPositions[0]);
            PracticeRacer practiceRacer = practiceRacerGO.AddComponent<PracticeRacer>();
            practiceRacer.Setup();

            //if practice data exists for this level
            Scene currentScene = SceneManager.GetActiveScene();
            string levelName = currentScene.name;
            PracticeLevelData levelData = dataManager.GetPracticeLevelData(levelName);
            if (levelData != null) {
                // create Ghost Racer
                int ghostCarId = levelData.carId;
                if (ghostCarId >= playerPrefabs.Length) {
                    ghostCarId = 0;
                }
                GameObject ghostRacerPrefab = playerPrefabs[ghostCarId];
                string ghostRacerName = "Ghost Racer";
                GameObject ghostRacerGO = SetupRacer(1, ghostRacerName, ghostRacerPrefab, spawnPositions[0]);
                ghostRacerGO.tag = "Racer";
                ghostRacerGO.name = ghostRacerName;
                GhostRacer ghostRacer = ghostRacerGO.AddComponent<GhostRacer>();
                ghostRacer.Setup();
            
            }
        }

        private GameObject SetupRacer(int racerId, string racerName, GameObject prefab, Transform spawnPosition)
        {
            GameObject racer = Instantiate(prefab, spawnPosition.position, spawnPosition.rotation) as GameObject;

            // Show or hide car position on the top of the car
            Car_Position carPosition = racer.GetComponent<Car_Position>();
            carPosition.displayPosition = false;
            carPosition.RacerID = racerId;
            carPosition.RacerName = racerName;

            Car_AI carAI = racer.GetComponent<Car_AI>();
            carAI.raceStarted = false;

            carPositions[racerId] = carPosition;
            racerNames[racerId] = racerName;

            // Add the racers position class to the list
            Racer_Position newRacePos = new Racer_Position() { Name = racerNames[racerId], Position = 0 };
            positions.Add(newRacePos);
            sortedPositions.Add(newRacePos);

            return racer;
        }

        public void Update_Positions_Display()
        {
            try
            {
                Start_Finish_UI start_Finish_UI = FindFirstObjectByType<Start_Finish_UI>();
                int numOfDrivers = sortedPositions.Count;
                for (int a = 0; a < start_Finish_UI.positions.Length; a++)
                {
                    if (a < numOfDrivers)
                    {
                        start_Finish_UI.driversName[a].text = sortedPositions[a].Name.ToString();
                        start_Finish_UI.positions[a].text = (a+1).ToString();
                    }
                    else
                    {
                        start_Finish_UI.driversName[a].text = "";
                        start_Finish_UI.positions[a].text = "";
                    }
                }
            }
                catch { }
            startUI.GetComponent<Start_Finish_UI>().totalScores.text =
                "Total Scores : " +
                PlayerPrefs.GetInt("TotalScores").ToString();
        }
        public void StartRace_Button()
        {
            if (!dontGetKey)
            {
                StartRace();
                dontGetKey = true;
            }
        }
        public void StartRace()
        {
            StartCoroutine(StartRaceDelay());
        }
        IEnumerator StartRaceDelay()
        {
            FindFirstObjectByType<InputSystem>().canControl = true;

            if (startUI)
                startUI.SetActive(false);
            if (raceUI)
                raceUI.SetActive(true);

            if (GetComponentInChildren<InputSystem>().controlType == InputType.Mobile)
                FindFirstObjectByType<Race_Manager>().mobileControls.SetActive(true);
            else
                FindFirstObjectByType<Race_Manager>().mobileControls.SetActive(false);

            // Enable or disable right side position ui
            if (PlayerPrefs.GetInt("ShowPositionUI") == 3)
                positionUI.SetActive(true);
            else
                positionUI.SetActive(false);

            if (mobileControls)
            {
                if (FindFirstObjectByType<InputSystem>().controlType == InputType.Mobile)
                    mobileControls.SetActive(true);

            }

            yield return new WaitForSeconds(1);

            FindFirstObjectByType<Start_Counter>().StartCounter();

            yield return new WaitForSeconds((counterNumbers) * timeScale);

            foreach (Car_AI carAI in FindObjectsOfType<Car_AI>())
            {
                carAI.raceStarted = true;
                carAI.gameObject.GetComponent<EasyCarController>()
                    .Clutch = false;
            }

            GameObject.FindGameObjectWithTag("Player")
                .GetComponent<EasyCarController>().Clutch = false;

            GameObject.FindGameObjectWithTag("Player")
                                .GetComponent<EasyCarAudio>().stopRandom = true;

            if (GameObject.FindGameObjectWithTag("Player")
                .GetComponent<EasyCarController>().throttleInput > 0.6f)
            {
                GameObject.FindGameObjectWithTag("Player")
                                .GetComponent<EasyCarAudio>().Play_StartSkid_Sound();
            }

            foreach (GameObject racerCars in GameObject.FindGameObjectsWithTag("Racer"))
                racerCars.GetComponent<EasyCarAudio>().Play_StartSkid_Sound();

            // User can display the pause menu after race start
            FindFirstObjectByType<Pause_Menu>().raceIsStarted = true;
            FindFirstObjectByType<Nitro_Feature>().raceIsStarted = true;

            foreach (Racer_Nitro rn in GameObject.FindObjectsOfType<Racer_Nitro>())
                rn.raceIsStarted = true;

            yield return new WaitForSeconds(
                GameObject.FindGameObjectWithTag("Player")
                .GetComponent<EasyCarController>().startDuration);

            GameObject.FindGameObjectWithTag("Player")
                .GetComponent<EasyCarController>().shaking = false;
            yield return new WaitForSeconds(1f);

            // Racers can check reverse mode after 2 seconds from the race start 
            foreach (Car_AI carAI in FindObjectsOfType<Car_AI>())
                carAI.canReverseCheck = true;

            eventManager.TriggerEvent(new RaceStartEvent());
         }
         public void Finish_Race()
         {
            eventManager.TriggerEvent(new RaceEndEvent());

            GameObject.FindGameObjectWithTag("Player").GetComponent<Car_AI>().enabled = true;
            FindFirstObjectByType<InputSystem>().canControl = false;
            FindFirstObjectByType<CameraSwitch>().SelectCamera(0);

             raceFinishUI.SetActive(true);

            FindFirstObjectByType<Start_Finish_UI>().finishRaceMenu.SetActive(true);
            FindFirstObjectByType<Start_Finish_UI>().startButton.SetActive(false);
            FindFirstObjectByType<Start_Finish_UI>().raceUI.SetActive(false);

             mobileControls.SetActive(false);
             Update_Positions_Display();

             // Update award icons (gold , bronze silver) at race finish menu
             if (sortedPositions[0].Name == playerName)
                 FindFirstObjectByType<Start_Finish_UI>().Update_Award(0, levelID);
             else if (sortedPositions[1].Name == playerName)
                 FindFirstObjectByType<Start_Finish_UI>().Update_Award(1, levelID);
             else if (sortedPositions[2].Name == playerName)
                 FindFirstObjectByType<Start_Finish_UI>().Update_Award(2, levelID);
             else
                 FindFirstObjectByType<Start_Finish_UI>().Update_Award(3, levelID);

             startUI.GetComponent<Start_Finish_UI>().totalScores.text =
                 "Total Scores : " +
                 PlayerPrefs.GetInt("TotalScores").ToString();
         }


         void Update()
         {
             //  Start race by keyboard shortcut
             if (!dontGetKey)
             {
                 if (canStart)
                 {
                     if (Input.GetKeyDown(startKey))
                     {
                         StartRace();
                         dontGetKey = true;
                     }
                 }
             }


             // Update ui info (player position + current lap   )
             if (playerInfo)
                 playerInfo.text = "Pos : " + (playerPosition.currentPosition + 1).ToString()
                 + " / " + carPositions.Length.ToString();
             else
                 Debug.Log("Please add -Position Info- text object in the -Race Manager- component");

             if (playerPosition.currentLap > 0)
             {
                 if (lapInfo)
                     lapInfo.text = "Lap : " + playerPosition.currentLap.ToString()
                      + " / " + totalLaps.ToString();
                 else
                     Debug.Log("Please add -Lap Info- text object in the -Race Manager- component");
             }
             else
             {
                 if (lapInfo)
                     lapInfo.text = "Lap : 1" + " / " + totalLaps.ToString();
                 else
                     Debug.Log("Please add -Lap Info- text object in the -Race Manager- component");
             }
            //_________________________________

            // Positions info
            int numOfRacers = sortedPositions.Count;
             for (int pos = 0; pos < racerInfo.Length; pos++)
            {
                try
                {
                    if (racerInfo[pos])
                    {
                        if (pos < numOfRacers)
                        {
                            racerInfo[pos].text = "   " + (pos + 1).ToString() + "   |   " + sortedPositions[pos].Name.ToString();
                        }
                        else
                        {
                            racerInfo[pos].text = "";
                        }
                    }
                }
                catch { }
            }
         }

         // List and sort car positions based on the istance form the checkpoints
         public void Update_Position(int racerID, string totalPoints)
         {
             // List and sort racer positions based on the distance from the checkpoint
            positions[racerID].Position = float.Parse(totalPoints);
             sortedPositions = positions.OrderBy(number => number.Position).ToList();

             sortedPositions.Reverse();
             //_________________________________

             for (int b = 0; b < sortedPositions.Count; b++)
             {
                if (playerPosition.RacerName == sortedPositions[b].Name)
                {
                    playerPosition.currentPosition = b;
                }
             }

            // Enable current position icon (on the top of the car) for each racer
            
             for (int a = 0; a < carPositions.Length; a++)
            {
                if (carPositions[a] == null)
                {
                    continue;
                }
                for (int c = 0; c < carPositions.Length; c++)
                {
                    if (c >= sortedPositions.Count || sortedPositions[c] == null)
                    {
                        continue;
                    }
                    if (carPositions[a].RacerName == sortedPositions[c].Name)
                    {
                        carPositions[a].Update_Position(c);
                    }
                }/*
                 if (carPositions[a].RacerName == sortedPositions[0].Name)
             {
                 carPositions[a].Update_Position(0);
              }

             if (carPositions[a].RacerName == sortedPositions[1].Name)
             { 
                 carPositions[a].Update_Position(1);
              }
             if (carPositions[a].RacerName == sortedPositions[2].Name)
             { 
                 carPositions[a].Update_Position(2);
             }
             if (carPositions[a].RacerName == sortedPositions[3].Name)
             {
                 carPositions[a].Update_Position(3);
             }
             if (carPositions[a].RacerName == sortedPositions[4].Name)
             {
                 carPositions[a].Update_Position(4);
             }*/
            }

        //_________________________________

    }

    }
}