using UnityEngine;

namespace ALIyerEdon
{
    public interface IOfflinePracticeAPI
    {
        public PracticeLevelData GetLevelData(string levelName);
        public void SaveLevelData(string levelName, PracticeLevelData levelData);
    }
}
