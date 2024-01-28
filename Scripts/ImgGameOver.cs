using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImgGameOver : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool holding = false;
    public bool up = false;
    bool Switch = true;
    public float dragDistance;
    public Transform skeleton;
    Vector2 pointDown, pointUp, pointHold;
    public RectTransform txtRetry, txtBackToMM;
    public Transform canvas;
    public Image foreground;

    public string dragDownFunction, dragUpFunction;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!up)
        {
            pointDown = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
            holding = true;
            Switch = true;
        }
    }

    private void Update()
    {
        if (holding)
        {
            if (!up)
                pointHold = Camera.main.ScreenPointToRay(Input.mousePosition).origin;

            if ((dragDistance >= 0 && pointHold.y - pointDown.y < 0) || (dragDistance <= 0 && pointHold.y - pointDown.y > 0))
                Switch = true;

            if (!up)
                dragDistance = pointHold.y - pointDown.y;

            if (dragDistance >= 0 && Switch)
            {
                txtRetry.SetParent(canvas);
                txtBackToMM.SetParent(skeleton);
                Switch = false;
            }
            else if (dragDistance >= 0 && !Switch)           // Yukarı Sürüklenirken
            {
                txtRetry.anchoredPosition = new Vector2(0, (-dragDistance * 100) - 100);
                if (txtRetry.position.y < 0)
                {
                    txtRetry.position = new Vector3(0, 0, 0);
                    Retry();
                }
            }
            else if (Switch)
            {

                txtRetry.SetParent(skeleton);
                txtBackToMM.SetParent(canvas);
                Switch = false;
            }
            else                                            // Aşağı Sürüklenirken
            {
                txtBackToMM.anchoredPosition = new Vector2(0, (-dragDistance * 100) + 100);
                if (txtBackToMM.position.y > 0)
                {
                    txtBackToMM.position = new Vector3(0, 0, 0);
                    LoadMM();
                }
            }
            skeleton.position = new Vector3(0, dragDistance);
            foreground.color = new Color(foreground.color.r, foreground.color.g, foreground.color.b, Mathf.Abs(dragDistance) / 5);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!up)
        {
            up = true;
            if (dragDistance < 0)
                InvokeRepeating("DecreaseDragDistance", 0.01f, 0.005f);
            else
                InvokeRepeating("IncreaseDragDistance", 0.01f, 0.005f);
        }
    }

    void IncreaseDragDistance()
    {
        dragDistance += 0.04f;
    }
    void DecreaseDragDistance()
    {
        dragDistance -= 0.04f;
    }

    void LoadMM()
    {
        GetComponent<ImgGameOver>().enabled = false;
        GameObject.Find("Foreground").GetComponent<Animation>().Play("Foregorund");

        if (AdvertManager.interstitialAvailable)
            StartCoroutine(AdvertManager.ShowInterstitial(0.5f, PPRManager.LoadMMLater(0.5f)));
        else
            StartCoroutine(PPRManager.LoadMMLater(0.5f));
    }
    void Retry()
    {
        GetComponent<ImgGameOver>().enabled = false;
        GameObject.Find("Foreground").GetComponent<Animation>().Play("Foregorund");

        if (AdvertManager.interstitialAvailable)
            StartCoroutine(AdvertManager.ShowInterstitial(0.5f, PPRManager.LoadPSLater(0.5f)));
        else
            StartCoroutine(PPRManager.LoadPSLater(0.5f));
    }
}
