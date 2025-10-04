using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectLetterbox : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    Camera cam;
    int lastW, lastH;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;
        ApplyLetterbox();
    }

    void Update()
    {
        if (Screen.width != lastW || Screen.height != lastH)
            ApplyLetterbox();
    }

    void ApplyLetterbox()
    {
        lastW = Screen.width;
        lastH = Screen.height;

        float windowAspect = (float)lastW / lastH;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            cam.rect = new Rect(0f, (1f - scaleHeight) * 0.5f, 1f, scaleHeight);
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;
            cam.rect = new Rect((1f - scaleWidth) * 0.5f, 0f, scaleWidth, 1f);
        }
    }
}
