using System;
using System.Collections;
using UnityEngine;


public class StaticCoroutine : MonoBehaviour
{
    public static StaticCoroutine instance;

    private void Awake()
    {
        instance = this;
    }

    public static Coroutine DoCoroutine(IEnumerator runnable)
    {
        CheckInstance();
        return instance.StartCoroutine(runnable);
    }

    public static void CancelCoroutine(Coroutine coroutine)
    {
        CheckInstance();
        instance.StopCoroutine(coroutine);
    }

    private static void CheckInstance()
    {
        if (instance == null)
        {
            instance = new GameObject().AddComponent<StaticCoroutine>();
            instance.gameObject.name = "StaticCoroutine";
            DontDestroyOnLoad(instance);
        }
    }
}
