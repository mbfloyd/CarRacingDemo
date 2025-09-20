using System;
using UnityEngine;

namespace ALIyerEdon
{
    public interface IDataManager
    {
        event Action<float> OnRaceTimeUpdate;

        public RaceMode GetRaceMode();
        public void SetRaceMode(RaceMode mode);
        public PracticeLevelData GetPracticeLevelData(string levelName);
        public void SetPracticeLevelData(string levelName, PracticeLevelData data);
        public void UpdateRaceTime(float time);
        public float GetRaceTime();
    }
}
