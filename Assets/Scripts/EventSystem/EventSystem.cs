using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author Emil Wessman
/// </summary>
namespace CallbackSystem {
    public class EventSystem : MonoBehaviour {
        private Dictionary<Type, HashSet<EventInfo>> eventListeners;
        private Dictionary<object, List<EventInfo>> listenersByTarget;


        private void OnEnable() {
            current = this;
            eventListeners = new Dictionary<Type, HashSet<EventInfo>>();
            listenersByTarget = new Dictionary<object, List<EventInfo>>();
        }

        //singleton setup
        private static EventSystem current;

        public static EventSystem Current {
            get {
                if (current == null) current = GameObject.FindObjectOfType<EventSystem>();
                return current;
            }
        }

        public delegate void EventListener(Event eventToListenFor);

        public void RegisterListener<T>(System.Action<T> listener) where T : Event {
            var eventType = typeof(T);
            if (!eventListeners.ContainsKey(eventType)) eventListeners[eventType] = new HashSet<EventInfo>();
            if (!listenersByTarget.ContainsKey(listener.Target))
                listenersByTarget.Add(listener.Target, new List<EventInfo>());
            void Wrapper(Event eventToListenFor) { listener((T)eventToListenFor); }
            var info = new EventInfo(Wrapper, listener.Target.GetType(), eventType, listener.Target);
            eventListeners[eventType].Add(info);
            listenersByTarget[listener.Target].Add(info);
        }

        public void UnregisterListener<T>(System.Action<T> listener) where T : Event {
            var eventType = typeof(T);
            if (!eventListeners.ContainsKey(eventType) || !listenersByTarget.ContainsKey(listener.Target)) return;

            if (listenersByTarget.ContainsKey(listener.Target))
                for (int i = listenersByTarget[listener.Target].Count - 1; i >= 0; i--) {
                    if (listenersByTarget[listener.Target][i].targetType == listener.Target.GetType()) {
                        eventListeners[eventType].Remove(listenersByTarget[listener.Target][i]);
                        listenersByTarget[listener.Target].RemoveAt(i);
                    }
                }
        }

        public void FireEvent(Event eventToFire) {
            var eventType = eventToFire.GetType();
            if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null) return;
            /* walks up the event hierarchy and makes sure that listeners to the superclass of the event also get called */
            do {
                // check if superclass has any registered listeners. Only relevant for iteration 2-n of loop thus the "double check"
                if (eventListeners.ContainsKey(eventType)) {
                    foreach (var info in eventListeners[eventType]) info.listener(eventToFire);
                }

                eventType = eventType.BaseType;
            } while (eventType != typeof(Event));
        }

        private class EventInfo {
            public EventInfo(EventListener eventListener, Type targetType, Type eventType, System.Object target) {
                listener = eventListener;
                this.targetType = targetType;
                this.eventType = eventType;
                this.target = target;
            }

            public readonly EventListener listener;
            public readonly Type targetType;
            public readonly Type eventType;
            public readonly System.Object target;
        }
    }
}