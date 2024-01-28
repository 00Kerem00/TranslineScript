using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    static AndroidJavaObject context;
    static AndroidJavaObject databaseInstance;

    static void SetAJOs()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if(context == null)
            using(AndroidJavaClass currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                context = currentActivity.GetStatic<AndroidJavaObject>("currentActivity");

        if (databaseInstance == null)
            using (AndroidJavaClass database = new AndroidJavaClass("com.example.translinelib.Database"))
                databaseInstance = database.CallStatic<AndroidJavaObject>("getInstance", context);
#endif
    }

    public static int GetLastScore()
    {
        int result;
#if UNITY_ANDROID && !UNITY_EDITOR
        SetAJOs();

        result = databaseInstance.Call<int>("getLastScore");
#elif UNITY_EDITOR
        result = 99;
#endif
        return result;
    }
    public static void SetLastScore(int lastScore)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        SetAJOs();

        databaseInstance.Call("setLastScore", lastScore);
#endif
    }

    public static int GetBestScore()
    {
        int result;
#if UNITY_ANDROID && !UNITY_EDITOR
        SetAJOs();

        result = databaseInstance.Call<int>("getBestScore");
#elif UNITY_EDITOR
        result = 99;
#endif
        return result;

    }
    public static void SetBestScore(int bestScore)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        SetAJOs();

        databaseInstance.Call("setBestScore", bestScore);
#endif
    }

    public static bool GetSound()
    {
        bool result;
#if UNITY_ANDROID && !UNITY_EDITOR
        SetAJOs();

        result = databaseInstance.Call<bool>("getSound");
    
#elif UNITY_EDITOR
        result = true;
#endif
        return result;
    }
    public static void SetSound(bool sound)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        SetAJOs();

        databaseInstance.Call("setSound", sound);
#endif
    }
}
