using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueTyper : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;

    private void Start()
    {
        Debug.Log(gameObject.name + " Start");
    }

    private void OnEnable()
    {
        Debug.Log("타이핑 시작");

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        string fullText = dialogueText.text;

        dialogueText.text = "";

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}