using UnityEngine;
using System.Collections;

public class Scene5Director : MonoBehaviour
{
    public PlanetTextDisplay planetText;
    //ublic FinalTextSequence finalText;

    // call this when Scene 5 timeline starts
    public void StartScene5()
    {
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        yield return new WaitForSeconds(2f);
        planetText.ShowText("Somewhere above this golf course,");

        yield return new WaitForSeconds(4f);
        planetText.ShowText("a dish still listens.");

        yield return new WaitForSeconds(7f);
        planetText.ShowText("72 seconds. The universe spoke once.");

        yield return new WaitForSeconds(10f);
        planetText.ShowText("The signal was too precise to be noise.");

        yield return new WaitForSeconds(5f);
        planetText.ShowText("But what if this signal actually meant something?");

        yield return new WaitForSeconds(15f);
        planetText.ShowText("We never responded. But what if someone is trying to reach us?");

        //yield return new WaitForSeconds(13f);
        //finalText.StartSequence();
    }
}