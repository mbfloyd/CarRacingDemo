using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ALIyerEdon
{
    public class PracticeInfoUI : MonoBehaviour
    {

        [SerializeField] private UnityEngine.UI.Text bestTime;
        [SerializeField] private UnityEngine.UI.Text raceTime;

        private IDataManager dataManager;

        void Awake()
        {
            dataManager = DIContainer.Instance.Resolve<IDataManager>();
            dataManager.OnRaceTimeUpdate += UpdateRaceTime;

            if (dataManager.GetRaceMode() == RaceMode.OfflinePractice)
            {
                Setup();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        void OnDestroy()
        {
            dataManager.OnRaceTimeUpdate -= UpdateRaceTime;
        }

        private void Setup()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            string levelName = currentScene.name;
            PracticeLevelData practiceLevelData = dataManager.GetPracticeLevelData(levelName);
            UpdateBestTime(practiceLevelData.raceTime);
        }

        private void UpdateBestTime(float timeInSeconds)
        {
            bestTime.text = FormatTime(timeInSeconds);
        }

        private void UpdateRaceTime(float timeInSeconds)
        {
            raceTime.text = FormatTime(timeInSeconds);
        }

        private string FormatTime(float timeInSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
        }
    }
}
