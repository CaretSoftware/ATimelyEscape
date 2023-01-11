using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    [SerializeField] private Dialogue dialogue;
    DialogueManager dialogueManager;

    private void Start() {
        dialogueManager = FindObjectOfType<DialogueManager>();

        NewRatCharacterController.NewCharacterInput.advanceDialogueDelegate += AdvanceDialouge;
        NewRatCharacterController.NewCharacterInput.returnToGameDelegate += GoBackToGame;
    }

    private void OnDestroy() {
        NewRatCharacterController.NewCharacterInput.advanceDialogueDelegate -= AdvanceDialouge;
        NewRatCharacterController.NewCharacterInput.returnToGameDelegate -= GoBackToGame;
    }

    private void AdvanceDialouge() {
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
}
