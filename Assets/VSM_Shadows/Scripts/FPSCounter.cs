using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // For UI Text component

public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsText; // UI Text component to display the FPS

    private float deltaTime = 0.0f;
    private const float FPS_REFRESH_RATE = 0.5f; // Refresh rate in seconds

    private float timer = 0.0f;
    private int frameCount = 0;
    private float fps = 0.0f;

    private StringBuilder sb = new StringBuilder(16); // For avoiding garbage

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        timer += Time.deltaTime;
        frameCount++;

        // Calculate FPS every second
        if (timer >= FPS_REFRESH_RATE)
        {
            fps = frameCount / timer;
            frameCount = 0;
            timer = 0.0f;

            // Format the FPS value
            sb.Clear();
            sb.Append("FPS: ").Append(Mathf.CeilToInt(fps));

            // Update the UI Text if assigned
           // if (fpsText != null)
         //   {
             //   fpsText.text = sb.ToString();
                TrySetText(fpsText,sb);
          //  }
        }

        // Optionally log FPS to console for debugging purposes
        // Debug.Log("FPS: " + Mathf.Ceil(fps));
    }

    public void TrySetText(TMP_Text textComponent, StringBuilder stringBuilder)
    {
        if(textComponent == null)
            return;

        if(stringBuilder == null)
            return;

        // TextMeshPro has built in support for SetText(StringBuilder).
        if(!stringBuilder.Equals(textComponent.text))
            textComponent.SetText(stringBuilder);
    }
}
