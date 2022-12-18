using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private float typingSpeed = 1f;
    [SerializeField] private float timeBetweenSentences = 10f;

    private Queue<string> sentences;
    private Queue<AudioClip> audioClips;
    private AudioSource audioSource;

    void Awake()
    {
        sentences = new Queue<string>();
        audioClips = new Queue<AudioClip>();
        audioSource = FindObjectOfType<AudioSource>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        foreach (AudioClip audioClip in dialogue.audioClips)
        {
            audioClips.Enqueue(audioClip);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        audioSource.clip = audioClips.Dequeue();
        if (audioSource.clip)
        {
            audioSource.Play();
        }
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        yield return new WaitForSecondsRealtime(timeBetweenSentences);
        DisplayNextSentence();
    }

    public void GoalReached(AudioClip clip)
    {
        animator.SetBool("IsOpen", true);
        if (clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        sentences.Clear();
        StopAllCoroutines();
        StartCoroutine(TypeSentence("Congratulations, you have finished the tutorial!"));
    }

    private void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        audioSource.Stop();
    }
}
