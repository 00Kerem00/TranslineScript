using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnStartedFS : MonoBehaviour
{
    public Animator logo;
    private void Start()
    {
        EnableAudioSource();
        logo.enabled = true;
        Invoke("LoadMM", 2);
    }

    private void EnableAudioSource()
    {
        if (!DatabaseManager.GetSound())
            GameObject.Find("Bizzie").GetComponent<AudioSource>().enabled = true;
        DontDestroyOnLoad(GameObject.Find("Bizzie"));
    }

    private void LoadMM()
    {
        PPRManager.LoadMM();
    }
}
