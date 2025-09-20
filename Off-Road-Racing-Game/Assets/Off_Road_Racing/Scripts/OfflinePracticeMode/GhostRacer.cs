using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ALIyerEdon
{
    public class GhostRacer : MonoBehaviour
    {
        private ITypeEventManager eventManager;
        private IDataManager dataManager;
        private PracticeLevelData levelData;
        private float raceStartTime;
        private Transform[] frontWheels = new Transform[2];
        private Transform[] backWheels =new Transform[2];
        private bool isRaceStarted = false;
        private int currentFrame = 0;

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

            Scene currentScene = SceneManager.GetActiveScene();
            string levelName = currentScene.name;
            levelData = dataManager.GetPracticeLevelData(levelName);

            EasyCarController easyCarController = GetComponent<EasyCarController>();
            frontWheels[0] = easyCarController.Wheel_Transforms[0];
            frontWheels[1] = easyCarController.Wheel_Transforms[1];
            backWheels[0] = easyCarController.Wheel_Transforms[2];
            backWheels[1] = easyCarController.Wheel_Transforms[3];

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;

            //turn off components not going to be used 
            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            boxCollider.isTrigger = true; //so it won't collide with the car, but still trigger the checkpoints

            //hide until the race starts
            HideGhostCar();
        }

        private void HideGhostCar()
        {
            gameObject.SetActive(false);
        }

        private void ShowGhostCar()
        {
            gameObject.SetActive(true);
        }

        private void RaceStart( RaceStartEvent gameEvent)
        {
            isRaceStarted = true;
            currentFrame = 0;
            raceStartTime = Time.realtimeSinceStartup;
            ShowGhostCar();
        }

        private void RaceEnd(RaceEndEvent gameEvent)
        {
            isRaceStarted = false;
        }

        private void Update()
        {
            if (isRaceStarted)
            {
                UpdateTransform();
            }
        }

        private void UpdateTransform()
        {
            float time = dataManager.GetRaceTime();

            // Stop at the end
            if (currentFrame >= levelData.transformData.Count - 1) return;

            // Advance until next timestamp
            while (currentFrame < levelData.transformData.Count - 1 &&
                   time >= levelData.transformData[currentFrame + 1].time)
            {
               currentFrame++;
            }

            // Interpolate between currentFrame and nextFrame
            PracticeTransformData a = levelData.transformData[currentFrame];
            PracticeTransformData b = levelData.transformData[Mathf.Min(currentFrame + 1, levelData.transformData.Count - 1)];

            float t = Mathf.InverseLerp(a.time, b.time, time);

            transform.position = Vector3.Lerp(a.position.GetVector3(), b.position.GetVector3(), t);
            transform.rotation = Quaternion.Slerp(a.rotation.GetQuaternion(), b.rotation.GetQuaternion(), t);

            // Wheels
            ApplyWheelRotations(a, b, t);
        }

        private void ApplyWheelRotations(PracticeTransformData a, PracticeTransformData b, float t)
        {
            Quaternion wheelRot = Quaternion.Slerp(a.wheelRotation.GetQuaternion(), b.wheelRotation.GetQuaternion(), t);

            frontWheels[0].localRotation = wheelRot;
            frontWheels[1].localRotation = wheelRot;

            float spin = wheelRot.eulerAngles.x; // assuming X axis = spin

            Vector3 eulerB0 = backWheels[0].localRotation.eulerAngles;
            Vector3 eulerB1 = backWheels[1].localRotation.eulerAngles;
            backWheels[0].localRotation = Quaternion.Euler(spin, eulerB0.y, eulerB0.z);
            backWheels[1].localRotation = Quaternion.Euler(spin, eulerB1.y, eulerB1.z);
        }
        
    }
}
