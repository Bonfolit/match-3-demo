using System;
using System.Collections.Generic;

namespace BonLib.Events
{

    public class EventManager
    {
        private Dictionary<Type, List<(object handler, Priority priority)>> m_listeners;

        public EventManager(int capacity)
        {
            m_listeners = new Dictionary<Type, List<(object handler, Priority priority)>>(capacity);
        }

        public void AddListener<T>(IEventHandler<T> handler, Priority priority = Priority.Normal) where T : IEvent 
        {
            if (!m_listeners.ContainsKey(typeof(T)))
            {
                var set = new List<(object handler, Priority priority)>();
                m_listeners.Add(typeof(T), set);
            }
            
            AddListenerInternal<T>(handler, priority);
        }

        private void AddListenerInternal<T>(object handler, Priority priority)
        {
            var listenerSet = m_listeners[typeof(T)];
            listenerSet.Add((handler, priority));
            
            listenerSet.Sort((x, y) => x.priority.CompareTo(y.priority));
        }
        
        public void RemoveListener<T>(IEventHandler<T> handler) where T : IEvent
        {
            if (!m_listeners.ContainsKey(typeof(T)))
            {
                return;
            }

            RemoveListenerInternal<T>(handler);
        }
        
        private void RemoveListenerInternal<T>(object handler)
        {
            var listenerSet = m_listeners[typeof(T)];
            listenerSet.RemoveAll(x => x.handler == handler);
        }
        
        public void SendEvent<T>(ref T evt) where T : IEvent
        {
            if (!m_listeners.ContainsKey(typeof(T)))
            {
                return;
            }

            var listenerSet = m_listeners[typeof(T)];
            // listenerSet.Sort((x, y) => x.priority.CompareTo(y.priority));

            for (var i = 0; i < listenerSet.Count; i++)
            {
                if(evt.IsConsumed)
                    break;
                
                var listener = listenerSet[i];
                var handler = listener.handler;
                
                if (handler is IEventHandler<T> typedHandler)
                {
                    typedHandler.OnEventReceived(ref evt);
                }
                else
                {
                    throw new Exception($"Event handler {handler} could not be cast into generic IEventHandler.");
                }
            }
        }
    }

}