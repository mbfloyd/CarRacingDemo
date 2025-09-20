using System.Collections.Generic;
using UnityEngine;

namespace ALIyerEdon
{

    [System.Serializable]
    public class PracticeLevelData
    {
        public int carId;
        public float raceTime;
        public List<PracticeTransformData> transformData = new List<PracticeTransformData>();
    }

    [System.Serializable]
    public class PracticeTransformData
    {
        public float time;
        public PracticePositionData position;
        public PracticeRotationData rotation;
        public PracticeRotationData wheelRotation;
    }

    [System.Serializable]
    public class PracticePositionData
    {
        public float x;
        public float y;
        public float z;

        public PracticePositionData(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        public Vector3 GetVector3()
        {
            return new Vector3(x, y, z);
        }
    }


    [System.Serializable]
    public class PracticeRotationData
    {
        public float w;
        public float x;
        public float y;
        public float z;

        public PracticeRotationData(Quaternion rotation)
        {
            w = rotation.w;
            x = rotation.x;
            y = rotation.y;
            z = rotation.z;
        }

        public Quaternion GetQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }
}
