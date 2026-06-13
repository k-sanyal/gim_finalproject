using UnityEngine;
using UnityEngine.Playables;

public class OfficeCutsceneManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline1;
    [SerializeField] private PlayableDirector timeline2;

    private void Start()
    {
        switch (StoryState.Chapter)
        {
            case 0:
                timeline1.Play();
                break;

            case 2:
                timeline2.Play();
                break;
        }
    }
}