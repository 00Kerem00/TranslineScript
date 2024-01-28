using UnityEngine;
using UnityEngine.UI;

public class OnStartedMM : MonoBehaviour
{
    public Text lastScore, bestScore, txtTitle;

    private void Start()
    {
        Color color = GeneralVariables.GetRandomColor();
        lastScore.color = color;
        bestScore.color = color;
        txtTitle.color = color;

        lastScore.text = "Last Score: " + DatabaseManager.GetLastScore();
        bestScore.text = "Best Score: " + DatabaseManager.GetBestScore();

        if (!GameObject.Find("Bizzie").GetComponent<AudioSource>().enabled)
        {
            GameObject.Find("SoundOn").GetComponent<Image>().enabled = false;
            GameObject.Find("SoundOff").GetComponent<Image>().enabled = true;
        }
    }
}
