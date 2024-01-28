using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidFunctions : MonoBehaviour
{
    private static AndroidJavaObject context;
    private static AndroidJavaObject othersInstance;

    private static void SetAJOs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (context == null)
            using (AndroidJavaClass currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                context = currentActivity.GetStatic<AndroidJavaObject>("currentActivity");

        if (othersInstance == null)
            using (AndroidJavaClass others = new AndroidJavaClass("com.example.translinelib.Others"))
                othersInstance = others.CallStatic<AndroidJavaObject>("getInstance", context);
#endif
    }

    public static void ShowToastMessage(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        SetAJOs();

        context.Call("runOnUiThread", new AndroidJavaRunnable(() => 
        {
            othersInstance.Call("showToastMessage", message);
        }));
#endif
    }

    public static void CheckStoragePermisson()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!IsStoragePermissionAllowed())
            RequestStoragePermission();
#endif
    }
    private static bool IsStoragePermissionAllowed()
    {
        SetAJOs();

        bool result = othersInstance.Call<bool>("isStoragePermissionAllowed");
        return result;
    }
    private static void RequestStoragePermission()
    {
        SetAJOs();

        context.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            othersInstance.Call("requestStoragePermission");
        }));
    }
}
