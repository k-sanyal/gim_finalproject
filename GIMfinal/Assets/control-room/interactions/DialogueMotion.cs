using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueMotion : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text dialogueText;

    [Header("Typing")]
    public float typingSpeed = 0.015f;

    [Header("Auto Play")]
    public bool playOnEnable = true;

    private Coroutine typingCoroutine;
    private string originalText;

    private void Awake()
    {
        if (dialogueText == null)
            dialogueText = GetComponent<TMP_Text>();

        if (dialogueText == null)
            dialogueText = GetComponentInChildren<TMP_Text>(true);
    }

    private void OnEnable()
    {
        if (!playOnEnable)
            return;

        if (dialogueText == null)
            return;

        originalText = dialogueText.text;

        if (string.IsNullOrEmpty(originalText))
            return;

        PlayCurrentText();
    }

    public void PlayCurrentText()
    {
        if (dialogueText == null)
            return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeRoutine());
    }

    private IEnumerator TypeRoutine()
    {
        dialogueText.maxVisibleCharacters = 0;

        yield return null;

        dialogueText.ForceMeshUpdate();

        int totalCharacters = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;
        typingCoroutine = null;
    }

    private void OnDisable()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueText != null)
            dialogueText.maxVisibleCharacters = int.MaxValue;
    }
}