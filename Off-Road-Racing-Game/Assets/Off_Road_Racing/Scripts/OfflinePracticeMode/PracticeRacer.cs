using UnityEngine;
using UnityEngine.SceneManagement;

namespace ALIyerEdon
{
    public class PracticeRacer : MonoBehaviour
    {
        private ITypeEventManager eventManager;
        private IDataManager dataManager;
        private PracticeLevelData levelData = new PracticeLevelData();
        private bool isRaceStarted = false;
        private float raceStartTime;
        private float recordInterval = 0.3f; //only record every 1/3 of a sec
        private float lastRecordTime;

        private Transform wheelReference;
        
        void OnDestroy()
        {
            eventManager.UnregisterListener<RaceStartEvent>(RaceStart);
            eventManager.UnregisterListener<RaceEndEvent>(RaceEnd);
        }
        public void Setup()
        {
            eventManager = DIContainer.Instance.Resolve<ITypeEventManager>();
            eventManager.RegisterListener<RaceStartEvent>(RaceStart);
            eventManager.RegisterListener<RaceEndEvent>(RaceEnd);

            dataManager = DIContainer.Instance.Resolve<IDataManager>();

            EasyCarController easyCarController = GetComponent<EasyCarController>();
            wheelReference = easyCarController.Wheel_Transforms[0];

            levelData.carId = PlayerPrefs.GetInt("CarID");
        }

        private void RaceStart( RaceStartEvent gameEvent)
        {
            isRaceStarted = true;
            raceStartTime = Time.realtimeSinceStartup;
            UpdateRactTime();
            RecordTransformData(); //grab race start transform
        }

        private void RaceEnd(RaceEndEvent gameEvent)
        {
            isRaceStarted = false;
            RecordTransformData(); //grab race end transform
            levelData.raceTime = GetTimeInRace();

            CheckRaceTime();
        }

        private void CheckRaceTime()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            string levelName = currentScene.name;
            PracticeLevelData previousLevelData = dataManager.GetPracticeLevelData(levelName);

            bool updateLevelData = true;
            if (previousLevelData != null)
            {
                if (previousLevelData.raceTime <= levelData.raceTime)
                {
                    updateLevelData = false;
                }
            }
            if (updateLevelData)
            {
                dataManager.SetPracticeLevelData(levelName, levelData);
            }
        }

        private void Update()
        {
            if (isRaceStarted) {
                UpdateRactTime();
                if ( Time.realtimeSinceStartup >= lastRecordTime + recordInterval) {
                     RecordTransformData();
                }
            }
        }

        private void UpdateRactTime()
        {
            float raceTime = GetTimeInRace();
            dataManager.UpdateRaceTime(raceTime);
        }

        private void RecordTransformData()
        {
            lastRecordTime = Time.realtimeSinceStartup;
            PracticeTransformData transformData = new PracticeTransformData();
            transformData.time = dataManager.GetRaceTime();
            transformData.position = new PracticePositionData(transform.position);
            transformData.rotation = new PracticeRotationData(transform.rotation);
            transformData.wheelRotation = new PracticeRotationData(wheelReference.localRotation);
            levelData.transformData.Add(transformData);
        }

        private float GetTimeInRace()
        {
            return Time.realtimeSinceStartup - raceStartTime;
        }
        
    }
}
