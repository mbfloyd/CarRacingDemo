
using System;
using System.Collections.Generic;

namespace ALIyerEdon
{
    public class DataManager : IDataManager
    {
        private RaceMode raceMode = RaceMode.Real;
        private Dictionary<string, PracticeLevelData> practiceLevelData = new Dictionary<string, PracticeLevelData>();
        private IOfflinePracticeAPI offlinePracticeAPI;

        public event Action<float> OnRaceTimeUpdate;
        private float raceTime;

        public DataManager()
        {
            offlinePracticeAPI = DIContainer.Instance.Resolve<IOfflinePracticeAPI>();
        }

        public RaceMode GetRaceMode() => raceMode;

        public void SetRaceMode(RaceMode mode)
        {
            raceMode = mode;
        }

        public PracticeLevelData GetPracticeLevelData(string levelName)
        {
            if (practiceLevelData.ContainsKey(levelName))
            {
                return practiceLevelData[levelName];
            }
            return offlinePracticeAPI.GetLevelData(levelName);
        }

        public void SetPracticeLevelData(string levelName, PracticeLevelData data)
        {
            practiceLevelData[levelName] = data;
            offlinePracticeAPI.SaveLevelData(levelName, data);
        }

        public void UpdateRaceTime(float time)
        {
            raceTime = time;
            OnRaceTimeUpdate?.Invoke(raceTime);
        }

        public float GetRaceTime()
        {
            return raceTime;
        }
    }
}
