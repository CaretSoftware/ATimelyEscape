using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (dialogueManager.dialogueStarted)
            {
                dialogueManager.NextPressed();
            }
            else
            {
                dialogueManager.dialogueStarted = true;
                dialogueManager.StartDialogue(dialogue);
            }
        }
    }

    /*public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }*/

}
