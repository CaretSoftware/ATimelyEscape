using UnityEngine;

namespace StateMachines.Testing {
    public class StateMachineTester : MonoBehaviour {
        private StateMachineScriptableObject sm;
        [SerializeField] private StateScriptableObject[] states;

        // Start is called before the first frame update
        private void Start() { sm = new StateMachineScriptableObject(this, states); }

        // Update is called once per frame
        private void Update() { sm.Run(); }
    }
}