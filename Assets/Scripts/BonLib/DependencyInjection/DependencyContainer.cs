using System;
using System.Collections.Generic;

namespace BonLib.DependencyInjection
{

    public class DependencyContainer
    {
        private Dictionary<Type, object> m_map;

        public DependencyContainer(int capacity)
        {
            m_map = new Dictionary<Type, object>(capacity);
        }

        public void Bind<T>(T dependency)
        {
            m_map[typeof(T)] = dependency;
        }
        
        public void Unbind<T>()
        {
            m_map.Remove(typeof(T));
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            if (m_map.TryGetValue(type, out var dependency))
            {
                return (T)dependency;
            }

            throw new Exception($"Dependency of type {typeof(T).FullName} does not exist!");
        }

        public bool CanResolve<T>()
        {
            return m_map.ContainsKey(typeof(T));
        }
    }

}
