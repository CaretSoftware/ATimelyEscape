using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private float typingSpeed = 1f;
    [SerializeField] private float timeBetweenSentences = 10f;
    [SerializeField] private AudioClip startClip;
    [SerializeField] private bool canTimeTravel;

    private Queue<string> sentences;
    private Queue<AudioClip> audioClips;
    private AudioSource audioSource;
    private RuntimeSceneManager sceneManager;

    [HideInInspector]
    public bool dialogueStarted;

    void Awake() {
        sentences = new Queue<string>();
        audioClips = new Queue<AudioClip>();
        audioSource = FindObjectOfType<AudioSource>();
        sceneManager = FindObjectOfType<RuntimeSceneManager>();
    }

    private void Start() {
        if (startClip) {
            audioSource.clip = startClip;
            audioSource.Play();
        }

        if (canTimeTravel) {
            GameObject player = GameObject.FindWithTag("Player");
            if (player) {
                player.GetComponent<NewRatCharacterController.NewCharacterInput>().CanTimeTravel = true;
            }
        }
    }

    public void StartDialogue(Dialogue dialogue) {
        animator.SetBool("IsOpen", true);
        sentences.Clear();
        audioClips.Clear();

        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }

        foreach (AudioClip audioClip in dialogue.audioClips) {
            audioClips.Enqueue(audioClip);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0 || audioClips.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        audioSource.clip = audioClips.Dequeue();
        if (audioSource.clip) {
            audioSource.Play();
        }
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence) {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        //yield return new WaitForSecondsRealtime(timeBetweenSentences);
        //DisplayNextSentence();
        if (sentence.Contains("Congratulations")) {
            Invoke(nameof(EndDialogue), 3f);
            Invoke(nameof(UnloadRoom), 3.1f);
        }
    }

    private void UnloadRoom() { sceneManager.UnloadOnboardingRoom(); }

    public void GoalReached(AudioClip clip) {
        animator.SetBool("IsOpen", true);
        if (clip) {
            audioSource.clip = clip;
            audioSource.Play();
        }

        sentences.Clear();
        audioClips.Clear();
        StopAllCoroutines();
        StartCoroutine(TypeSentence("Congratulations, you have finished the tutorial!"));
    }

    private void EndDialogue() {
        animator.SetBool("IsOpen", false);
        audioSource.Stop();
        Invoke(nameof(SetDialogueStarted), 1f);
    }

    private void SetDialogueStarted() {
        dialogueStarted = false;
    }

    public void NextPressed() {
        StopAllCoroutines();
        audioSource.Stop();
        DisplayNextSentence();
    }
}
