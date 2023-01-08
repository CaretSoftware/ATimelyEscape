using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    [SerializeField] private Dialogue dialogue;
    DialogueManager dialogueManager;

    private void Start() {
        dialogueManager = FindObjectOfType<DialogueManager>();

        NewRatCharacterController.NewCharacterInput.dPadRightPressed += WhatHappensOnDpadRightPressed;
        NewRatCharacterController.NewCharacterInput.dPadLeftPressed += WhatHappensOnDpadLeftPressed;
    }


    private void OnDestroy() {
        NewRatCharacterController.NewCharacterInput.dPadRightPressed -= WhatHappensOnDpadRightPressed;
        NewRatCharacterController.NewCharacterInput.dPadLeftPressed -= WhatHappensOnDpadLeftPressed;
    }

    private void WhatHappensOnDpadRightPressed() {
        OnNextDialogue();
    }

    private void WhatHappensOnDpadLeftPressed() {
        GoBackToGame();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            OnNextDialogue();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoBackToGame();
        }
    }

    private void OnNextDialogue() {
        if (dialogueManager.dialogueStarted) {
            dialogueManager.NextPressed();
        } else {
            dialogueManager.dialogueStarted = true;
            dialogueManager.StartDialogue(dialogue);
        }
    }

    private void GoBackToGame() {
        FindObjectOfType<RuntimeSceneManager>().UnloadOnboardingRoom();
    }


    /*public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }*/
}
