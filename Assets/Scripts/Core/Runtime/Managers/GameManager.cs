using System.Linq;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using Core.Config;
using Core.Runtime.Events.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Runtime.Managers
{
    public class GameManager : MonoBehaviour,
        IEventHandler<SceneLoaderReadyEvent>,
        IEventHandler<MainSceneLoadedEvent>
    {
        private GameConfig m_config;
        public GameConfig Config => m_config ??= Resources.Load<GameConfig>("Config/GameConfig");
        
        private EventManager m_eventManager;

        private IManager[] m_managers;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            
            DI.Bind(this);
            m_eventManager = new EventManager(64);
            DI.Bind<EventManager>(m_eventManager);

            m_eventManager.AddListener<SceneLoaderReadyEvent>(this, Priority.Critical);
            m_eventManager.AddListener<MainSceneLoadedEvent>(this, Priority.Critical);
        }

        private void Start()
        {
            SceneManager.LoadScene(Config.LoadingScene);
        }

        public void OnEventReceived(ref SceneLoaderReadyEvent evt)
        {
            var loadSceneEvent = new LoadSceneEvent(Config.MainScene);
            m_eventManager.SendEvent(ref loadSceneEvent);
        }

        public void OnEventReceived(ref MainSceneLoadedEvent evt)
        {
            if (evt.SceneIndex == Config.MainScene)
            {
                LoadMainSceneObjects();
                InitializeManagers();
            }
        }

        private void LoadMainSceneObjects()
        {
            Instantiate(Config.UIPrefab);
        }

        private void InitializeManagers()
        {
            m_managers = FindObjectsOfType<MonoBehaviour>().OfType<IManager>().ToArray();
            
            foreach (var manager in m_managers)
            {
                manager.BindDependencies();
            }
            foreach (var manager in m_managers)
            {
                manager.ResolveDependencies();
            }
            foreach (var manager in m_managers)
            {
                manager.SubscribeToEvents();
            }
            foreach (var manager in m_managers)
            {
                manager.PreInitialize();
            }
            foreach (var manager in m_managers)
            {
                manager.Initialize();
            }
            foreach (var manager in m_managers)
            {
                manager.LateInitialize();
            }

            var evt = new ManagersInitializedEvent();
            m_eventManager.SendEvent(ref evt);
        }

        private void DisposeManagers()
        {
            foreach (var manager in m_managers)
            {
                manager.Dispose();
            }
        }
    }

}