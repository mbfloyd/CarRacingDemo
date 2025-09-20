using UnityEngine;

namespace ALIyerEdon
{
    public class OfflinePracticeAPIJson : IOfflinePracticeAPI
    {
        private string fileName = "BestTimes/{0}.json";

        public PracticeLevelData GetLevelData(string levelName)
        {
            PracticeLevelData levelData = JsonFileHelper.Load<PracticeLevelData>(GetLevelFileName(levelName));
            return levelData;
        }

        public void SaveLevelData(string levelName, PracticeLevelData levelData)
        {
            JsonFileHelper.Save(levelData, GetLevelFileName(levelName));
        }

        private string GetLevelFileName(string levelName)
        {
            return string.Format(fileName, levelName); ;
        }

    }
}
