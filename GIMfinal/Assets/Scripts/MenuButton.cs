using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text label;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    private Color hoverColor = new Color(1f, 1f, 1f, 0.7f);
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        label.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData e)
    {
        label.color = hoverColor;
        if(hoverSound) audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData e)
    {
        label.color = normalColor;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if(clickSound) audioSource.PlayOneShot(clickSound);
    }
}