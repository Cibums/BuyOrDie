using UnityEngine;
using System.Collections;

public class GlitchController : MonoBehaviour
{
    [Header("Refs")]
    public Material glitchMat;

    [Header("Timing")]
    public float rampUp = 0.35f;
    public float hold   = 0.40f;
    public float speed  = 3f;

    [Header("Look")]
    [Range(0,5)] public float rgbSplit = 1f;
    [Range(1,200)] public float blockiness = 40f;
    [Range(0,1)] public float noise = 0.2f;

    bool busy;

    public static GlitchController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        if (glitchMat != null)
        {
            glitchMat.SetFloat("_Amount", 0f);
            glitchMat.SetFloat("_Speed", speed);
            glitchMat.SetFloat("_RGBSplit", rgbSplit);
            glitchMat.SetFloat("_Blockiness", blockiness);
            glitchMat.SetFloat("_NoiseIntensity", noise);
        }
    }

    public void StartGlitchAndRedirect(string url)
    {
        if (!busy && glitchMat != null)
            StartCoroutine(DoGlitchThenRedirect(url));
    }

    IEnumerator DoGlitchThenRedirect(string url)
    {
        busy = true;

        // ramp up
        float t = 0f;
        while (t < rampUp)
        {
            t += Time.unscaledDeltaTime;
            glitchMat.SetFloat("_Amount", Mathf.SmoothStep(0f, 1f, t / rampUp));
            yield return null;
        }
        glitchMat.SetFloat("_Amount", 1f);

        // hold
        yield return new WaitForSecondsRealtime(hold);

#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLRedirect.RedirectSameTab(url); // JS interop (defined below)
#else
        Application.Quit();
#endif
    }
}