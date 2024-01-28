using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PPRManager : MonoBehaviour
{
    public static void LoadMM()
    {
        SceneManager.LoadScene(1);
    }
    public static IEnumerator LoadMMLater(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        LoadMM();
    }

    public static void LoadPS()
    {
        SceneManager.LoadScene(2);
    }
    public static IEnumerator LoadPSLater(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        LoadPS();
    }

    public static void GameOver(int score)
    {
        GameObject.Find("Protagonist").GetComponent<BoxCollider2D>().enabled = false;

        DatabaseManager.SetLastScore(score);
        Spawner.locations = new bool[9, 9];
        Spawner.era = 0;

        GameObject.Find("LastScoreFrame").GetComponent<ImgGameOver>().enabled = true;
        StaticCoroutine.DoCoroutine(EnableGameOverAnimators());
        GameObject.Find("ImgGameOver").GetComponent<Animation>().Play("ImgGameOver");
        GameObject.Find("TxtLastScore").GetComponent<Text>().text = score.ToString();
        Camera.main.GetComponent<Animation>().enabled = false;
        Camera.main.GetComponent<Transform>().position = new Vector3(0, 0, -10);
    }
    private static IEnumerator EnableGameOverAnimators()
    {
        yield return new WaitForSeconds(0.25f);
        GameObject.Find("ImgDragUpArrow").GetComponent<Animator>().enabled = true;
        GameObject.Find("ImgDragDownArrow").GetComponent<Animator>().enabled = true;
        GameObject.Find("LastScoreFrame").GetComponent<Animator>().enabled = true;
    }
    public static IEnumerator GameOverLater(float delayTime, int score)
    {
        yield return new WaitForSeconds(delayTime);
        GameOver(score);
    }

    public static void InterruptForLoadingInterstitial()
    {
        GameObject.Find("ImgAdLoading").GetComponent<Image>().enabled = true;
        GameObject.Find("TxtAdLoading").GetComponent<Text>().enabled = true;
    }
    public static void ContinueAfterInterstitial()
    {
        GameObject.Find("ImgAdLoading").GetComponent<Image>().enabled = false;
        GameObject.Find("TxtAdLoading").GetComponent<Text>().enabled = false;
    }

    public static void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
    public static IEnumerator SetTimeScaleLater(float delayTime, float timeScale)
    {
        yield return new WaitForSeconds(delayTime);
        SetTimeScale(timeScale);
    }

    public static void Pause()
    {
        GameObject.Find("Protagonist").GetComponent<Protagonist>().enabled = false;
        GameObject.Find("Protagonist").GetComponent<BoxCollider2D>().enabled = false;

        GameObject.Find("ImgPause").GetComponent<Animation>().Play("ImgPause");
        GameObject.Find("ImgPause").GetComponent<Transform>().position = new Vector3(0, 0, 30);
        StaticCoroutine.DoCoroutine(SetPauseAnimators(0.25f, true));
    }
    public static void Resume()
    {
        GameObject.Find("Protagonist").GetComponent<Protagonist>().enabled = true;
        GameObject.Find("Protagonist").GetComponent<BoxCollider2D>().enabled = true;

        GameObject.Find("ImgPause").GetComponent<Animation>().Play("ImgPauseRewind");
        StaticCoroutine.DoCoroutine(MoveImgPause(0.25f));
        StaticCoroutine.DoCoroutine(SetPauseAnimators(0.01f, false));
    }
    private static IEnumerator MoveImgPause(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        GameObject.Find("ImgPause").GetComponent<Transform>().position = new Vector3(20, 0);
    }
    private static IEnumerator SetPauseAnimators(float delayTime, bool state)
    {
        yield return new WaitForSeconds(delayTime);
        GameObject.Find("BtnResume").GetComponent<Animation>().enabled = state;
        GameObject.Find("BtnReplay").GetComponent<Animation>().enabled = state;
        GameObject.Find("BtnBackToMM").GetComponent<Animation>().enabled = state;
    }
}
