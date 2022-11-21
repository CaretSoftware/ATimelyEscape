using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = System.Object;

namespace CallbackSystem {
    // public fields are supposed to start with capital letters
    public abstract class Event {
        public GameObject gameObject;
        public void Invoke() { EventSystem.Current.FireEvent(this); }

        // would really like to do this without the generic parameter, but I'm not smart enough to figure out how reflection works
        public static void AddListener<T>(System.Action<T> listener) where T : Event {
            EventSystem.Current.RegisterListener<T>(listener);
        }

        public static void RemoveListener<T>(System.Action<T> listener) where T : Event {
            EventSystem.Current.UnregisterListener<T>(listener);
        }
    }

    public class DebugEvent : Event {
        public string DebugText;
    }

    public class DieEvent : DebugEvent {
        public AudioClip deathSound;
        public float timeToDestroy;
        public List<ParticleSystem> particleSystems;
        public Renderer renderer;
    }

    public class DestinyChanged : DebugEvent {
        public TimeTravelObject changedObject;
    }

    public class TimePeriodChanged : DebugEvent {
        public TimeTravelPeriod from;
        public TimeTravelPeriod to;
    }


    // this looks feels very redundant, looking into alternative solutions.
    public class PhysicsSimulationComplete : DebugEvent
    {
        public TimeTravelPeriod from;
        public TimeTravelPeriod to;
    }

    public class CheckpointEvent : Event
    {
        public Transform respawnPoint;

        public CheckpointEvent(Transform respawnPoint)
        {
            this.respawnPoint = respawnPoint;
        }
    }

    public class FailStateEvent : Event
    {

    }

    public class ChargeChangedEvent : Event
    {
        public TimeTravelObject changedObject;

        public ChargeChangedEvent(TimeTravelObject changedObject)
        {
            this.changedObject = changedObject;
        }
    }

    public class SetVignetteModifierEvent : Event
    {
        public Vignette vignette;

        public SetVignetteModifierEvent(Vignette vignette)
        {
            this.vignette = vignette;
            Debug.Log(vignette);
        }
    }
}