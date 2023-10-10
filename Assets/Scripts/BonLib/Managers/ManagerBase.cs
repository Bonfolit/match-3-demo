using BonLib.DependencyInjection;
using BonLib.Events;
using UnityEngine;

namespace BonLib.Managers
{

    public class ManagerBase : MonoBehaviour, IManager
    {
        protected EventManager EventManager;

        public virtual void BindDependencies() {}
        public virtual void ResolveDependencies() {}
        public virtual void SubscribeToEvents() {}
        public virtual void PreInitialize() {}
        public virtual void Initialize() {}
        public virtual void LateInitialize() {}
        public virtual void Dispose() {}
    }

    public class Manager<T> : ManagerBase where T : Manager<T>
    {
        public override void BindDependencies()
        {
            DI.Bind<T>(this as T);
        }
        
        public override void ResolveDependencies()
        {
            EventManager = DI.Resolve<EventManager>();
        }

        public override void SubscribeToEvents()
        {
        }

        public override void Initialize()
        {
            Debug.Log("Initialized " + typeof(T).Name);
        }
    }

}