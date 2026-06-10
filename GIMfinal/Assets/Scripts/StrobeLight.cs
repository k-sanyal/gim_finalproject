using UnityEngine;

public class StrobeLight : MonoBehaviour
{
    public float frequency = 10f;
    public float onDuration = 0.05f;
    private Light pointLight;
    private float timer;
    private bool isOn;

    void Start()
    {
        pointLight = GetComponent<Light>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        float interval = 1f / frequency;

        if (!isOn && timer >= interval)
        {
            pointLight.enabled = true;
            isOn = true;
            timer = 0f;
        }
        else if (isOn && timer >= onDuration)
        {
            pointLight.enabled = false;
            isOn = false;
            timer = 0f;
        }
    }
}