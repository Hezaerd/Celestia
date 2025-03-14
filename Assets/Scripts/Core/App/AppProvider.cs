using UnityEngine;
using MHL.Core.DependencyInjection;

namespace MHL.Core.App
{
    public class AppProvider : MonoProvider
    {
        private static AppProvider _instance;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_instance != null)
            {
                Debug.Log($"[AppProvider] Already initialized.");
                return;
            }

            GameObject go = new GameObject("AppProvider");
            _instance = go.AddComponent<AppProvider>();
            
            DontDestroyOnLoad(go);
            
            Debug.Log("[AppProvider] Successfully initialized.");
        }
        
        protected override void Awake()
        {
            // Check to make sure the class is a valid class instance
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Setup container and inject dependencies
            base.Awake();
        }
        
        public override void RegisterDependencies(IDependencyContainer container)
        {
            // Register app dependencies here
            
            Debug.Log("[AppProvider] Successfully registered dependencies.");
        }
    }
}
