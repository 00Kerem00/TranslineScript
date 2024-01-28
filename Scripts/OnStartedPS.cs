using UnityEngine;
using UnityEngine.UI;

public class OnStartedPS : MonoBehaviour
{
    public SpriteRenderer line, arrowMark;
    public Image imgUp, imgDown;
    public Image imgDragUpArrow;
    public Image imgDragDownArrow;
    public Image lastScoreFrame;
    public Text txtRetry, txtBackToMM;
    public Text txtLastScore, txtLastScoreString;
    public GameObject txtHealth;
    public GameObject ImgHealth;

    Color color;

    private void Awake()
    {
        SetPanelWithInternetReachability();

        color = GeneralVariables.GetRandomColor();
        arrowMark.color = new Color(color.r, color.g, color.b, 0);
        line.color = color;
        imgUp.color = color;
        imgDown.color = color;

        imgDragUpArrow.color = color;
        Image[] upArrows = imgDragUpArrow.gameObject.GetComponentsInChildren<Image>();
        foreach (Image arrow in upArrows)
            arrow.color = color;

        imgDragDownArrow.color = color;
        Image[] downArrows = imgDragDownArrow.gameObject.GetComponentsInChildren<Image>();
        foreach (Image arrow in downArrows)
            arrow.color = color;

        lastScoreFrame.color = color;
        Image[] lastScoreFrameObjects = lastScoreFrame.gameObject.GetComponentsInChildren<Image>();
        foreach (Image Object in lastScoreFrameObjects)
            Object.color = color;

        txtLastScore.color = color;
        txtLastScoreString.color = color;
        txtRetry.color = color;
        txtBackToMM.color = color;

        if (!GameObject.Find("Bizzie").GetComponent<AudioSource>().enabled)
        {
            GameObject.Find("SoundOn").GetComponent<Image>().enabled = false;
            GameObject.Find("SoundOff").GetComponent<Image>().enabled = true;
        }
    }

    private void SetPanelWithInternetReachability()
    {
        if(Application.internetReachability != NetworkReachability.NotReachable)
        {
            GameObject.Find("ImgDownPanel").SetActive(false);
            AdvertManager.ShowBanner();
            InvokeRepeating("UpdateBanner", 45, 45);
        }
        else
        {
            ImgHealth.SetActive(false);
            txtHealth.SetActive(false);
        }
    }

    private void UpdateBanner()
    {
        AdvertManager.DestroyBanner();
        AdvertManager.ShowBanner();
    }
}
