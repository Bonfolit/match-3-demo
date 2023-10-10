namespace BonLib.DependencyInjection
{

    public static class DI
    {
        private static DependencyContainer m_container;

        private static DependencyContainer Container => m_container ??= new DependencyContainer(64);

        public static void Bind<T>(T dependency)
        {
            Container.Bind(dependency);
        }
        
        public static void Unbind<T>()
        {
            Container.Unbind<T>();
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static bool CanResolve<T>()
        {
            return Container.CanResolve<T>();
        }
    }

}