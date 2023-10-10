using BonLib.DependencyInjection;
using BonLib.Events;
using Core.Runtime.Events.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Runtime.Managers
{
    public class SceneLoader : MonoBehaviour, 
        IEventHandler<LoadSceneEvent>,
        IEventHandler<ManagersInitializedEvent>
    {
        private EventManager m_eventManager;
        private GameManager m_gameManager;
        
        private void Awake()
        {
            m_eventManager = DI.Resolve<EventManager>();
        }

        private void OnEnable()
        {
            m_eventManager.AddListener<LoadSceneEvent>(this, Priority.Critical);
            m_eventManager.AddListener<ManagersInitializedEvent>(this, Priority.Critical);

            m_gameManager ??= DI.Resolve<GameManager>();

            var evt = new SceneLoaderReadyEvent();
            m_eventManager.SendEvent(ref evt);
        }

        private void OnDisable()
        {
            m_eventManager.RemoveListener<LoadSceneEvent>(this);
        }

        public void OnEventReceived(ref LoadSceneEvent evt)
        {
            LoadScene(evt.SceneIndex);
        }

        private void LoadScene(int index)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            
            operation.completed += (obj) =>
            {
                OnSceneLoaded(index);
            };
        }

        private void OnSceneLoaded(int sceneIndex)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
            
            var evt = new MainSceneLoadedEvent(sceneIndex);
            m_eventManager.SendEvent(ref evt);
        }

        public void OnEventReceived(ref ManagersInitializedEvent evt)
        {
            SceneManager.UnloadSceneAsync(m_gameManager.Config.LoadingScene);
        }
    }

}