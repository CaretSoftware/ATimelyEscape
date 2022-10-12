using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem {
    public class EventSystem : MonoBehaviour {
        private Dictionary<Type, List<EventInfo>> eventListeners;


        private void OnEnable() {
            current = this;
            eventListeners = new Dictionary<Type, List<EventInfo>>();
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
            Type eventType = typeof(T);
            if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null)
                eventListeners[eventType] = new List<EventInfo>();
            EventListener wrapper = (eventToListenFor) => { listener((T)eventToListenFor); };
            eventListeners[eventType].Add(new EventInfo(wrapper, listener.Target.GetType(), listener.Target));
        }

        public void UnregisterListener<T>(System.Action<T> listener) where T : Event {
            Type eventType = typeof(T);
            if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null) return;

            for (int i = eventListeners[eventType].Count - 1; i >= 0; i--) {
                if (eventListeners[eventType][i].targetType == listener.Target.GetType() &&
                    listener.Target == eventListeners[eventType][i].target)
                    eventListeners[eventType].RemoveAt(i);
            }
        }

        public void FireEvent(Event eventToFire) {
            System.Type eventType = eventToFire.GetType();
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
            public EventInfo(EventListener eventListener, Type targetType, System.Object target) {
                listener = eventListener;
                this.targetType = targetType;
                this.target = target;
            }

            public readonly EventListener listener;
            public readonly Type targetType;
            public readonly System.Object target;
        }
    }
}