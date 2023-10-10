namespace BonLib.Managers
{

    public interface IManager
    {
        void BindDependencies();
        void ResolveDependencies();
        void SubscribeToEvents();
        void PreInitialize();
        void Initialize();
        void LateInitialize();
        void Dispose();
    }

}