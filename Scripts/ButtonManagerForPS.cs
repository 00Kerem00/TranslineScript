using UnityEngine;
using UnityEngine.UI;

public class ButtonManagerForPS : MonoBehaviour
{
    public Animator imgDragUpArrow, imgDragDownArrow, frame;
    public GameObject frameOb;

    public void BtnPause()
    {
        PPRManager.Pause();
    }
    public void BtnResume()
    {
        PPRManager.Resume();
    }
    public void BtnBackToMM()
    {
        GameObject.Find("Foreground").GetComponent<Animation>().Play("Foregorund");
        StartCoroutine(PPRManager.LoadMMLater(0.5f));
    }
    public void BtnReplay()
    {
        GameObject.Find("Foreground").GetComponent<Animation>().Play("Foregorund");
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
}
