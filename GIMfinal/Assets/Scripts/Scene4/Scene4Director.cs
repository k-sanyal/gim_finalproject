using UnityEngine;
using System.Collections;

public class Scene4Director : MonoBehaviour
{
    public PlanetTextDisplay subtitles;
    public UnityEngine.Playables.PlayableDirector scene4Timeline;
    public UnityEngine.Playables.PlayableDirector scene5Timeline;

    public void StartScene4()
    {
        scene4Timeline.Play();
        StartCoroutine(RunDialogue());
    }

    IEnumerator RunDialogue()
    {
        yield return new WaitForSeconds(1f);
        subtitles.ShowText("<color=#aaaaaa>CHIEF SCIENTIST</color>\n\"The university has made its decision.\"");

        yield return new WaitForSeconds(6f);
        subtitles.ShowText("<color=#aaaaaa>CHIEF SCIENTIST</color>\n\"Big Ear will be decommissioned.\"");

        yield return new WaitForSeconds(6f);
        subtitles.ShowText("<color=#aaaaaa>DR. EHMAN</color>\n\"What about the Wow! Signal?\"");

        yield return new WaitForSeconds(6f);
        subtitles.ShowText("<color=#aaaaaa>CHIEF SCIENTIST</color>\n\"It's been sixteen years.\nWe're done listening.\"");

        yield return new WaitForSeconds(6f);
        subtitles.ShowText("<color=#aaaaaa>UNKNOWN</color>\n\"Someone should tell the dish.\"");

        yield return new WaitForSeconds(4f);

        // transition to scene 5
        scene5Timeline.Play();
    }
}