using System.Runtime.InteropServices;
using UnityEngine;

public static class WebGLRedirect
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void RedirectSameTab_Internal(string url);
#else
    private static void RedirectSameTab_Internal(string url) { Application.OpenURL(url); }
#endif

    public static void RedirectSameTabSafe(string url) { RedirectSameTab_Internal(url); }
    public static void RedirectSameTab(string url) { RedirectSameTab_Internal(url); }
}