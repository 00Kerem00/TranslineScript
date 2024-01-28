using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManagerForMM : MonoBehaviour
{
    public Animation foreground;
    public Transform Main, Settings;

    public void BtnPlay()
    {
        GameObject.Find("BtnPlay").GetComponent<Animation>().Play("BtnPlay");
        Invoke("BtnPlay2", 0.5f);
    }

    void BtnPlay2()
    {
        foreground.Play("Foregorund");
        StartCoroutine(PPRManager.LoadPSLater(0.5f));
    }

    public void BtnSound()
    {
        if (GameObject.Find("Bizzie").GetComponent<AudioSource>().enabled)
        {
            GameObject.Find("Bizzie").GetComponent<AudioSource>().enabled = false;
            GameObject.Find("SoundOn").GetComponent<Image>().enabled = false;
            GameObject.Find("SoundOff").GetComponent<Image>().enabled = true;
            DatabaseManager.SetSound(false);
        }
        else
        {
            GameObject.Find("Bizzie").GetComponent<AudioSource>().enabled = true;
            GameObject.Find("SoundOn").GetComponent<Image>().enabled = true;
            GameObject.Find("SoundOff").GetComponent<Image>().enabled = false;
            DatabaseManager.SetSound(true);
        }
    }

    public void BtnSettings()
    {
        foreground.Play("Foreground3");
        StartCoroutine(SwitchToSettings(0.5f));
    }

    public IEnumerator SwitchToSettings(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Main.localPosition = new Vector3(800, 0);
        Settings.localPosition = new Vector3(0, 0);
    }

    public IEnumerator SwitchToMain(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Main.localPosition = new Vector3(0, 0);
        Settings.localPosition = new Vector3(800, 0);
    }

    public void BtnExit()
    {
        Application.Quit();
    }

    public void BtnBackFromSettings()
    {
        foreground.Play("Foreground3");
        StartCoroutine(SwitchToMain(0.5f));
    }
}
