using UnityEngine;

namespace ALIyerEdon
{
    public class Bootstrapper : MonoBehaviour
    {
        void Awake()
        {
            SetupDependencies();
        }

        private void SetupDependencies()
        {
            // Register as Singleton (default is Singleton if not specified)
            DIContainer.Instance.Register<ITypeEventManager, TypeEventManager>(Lifetime.Singleton);
            DIContainer.Instance.Register<IOfflinePracticeAPI, OfflinePracticeAPIJson>(Lifetime.Singleton);
            DIContainer.Instance.Register<IDataManager, DataManager>(Lifetime.Singleton);
        }

    }
}
