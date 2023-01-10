using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class IncubatorManager : MonoBehaviour {


    // Start is called before the first frame update
    /*     void Start() {
            PuzzleCompleted e = new PuzzleCompleted();
            e.Invoke();
        }

        // Update is called once per frame
        void Update() {
        }

        private void OnStageOneComplete(PuzzleCompleted e) {
            if (e.stage == 1) print(1);
        }
        private void OnStageTwoComplete(PuzzleCompleted e) {
            if (e.stage == 2) print(1);
        }
        private void OnStageThreeComplete(PuzzleCompleted e) {
            if (e.stage == 3) print(1);
        } */
}

public class PuzzleStage {
    private UnityEngine.Events.UnityAction stageStartDelegate;
    private UnityEngine.Events.UnityAction stageRun;
    private UnityEngine.Events.UnityAction stageCompleteDelegate;
    private int stageIndex;

    private void StartStage() { stageStartDelegate.Invoke(); }

    public void RunStage() { stageRun.Invoke(); }

    private void CompleteStage() { stageCompleteDelegate.Invoke(); }
}
