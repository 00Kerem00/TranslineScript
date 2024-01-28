using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public static bool[,] locations = new bool[9, 9];

    private static int newEraFrequency = 15;
    public static int era = 0;
    private static int maxEra = 5;

    public static int deadlyCircleCountOffset = 4;
    public static int deadlyCircleIncreaseFrequency = 4;
    private static int maxCircleCount = 22; 

    public static int lineGuardSpawnFrequency = 5;
    public static int lineGuardIncreaseFrequency = 15;
    private static int maxLineGuardCount = 4;

    public static int extraHealthSpawnFrequency = 10;

    public static void SpawnObjects(int score, bool respawn)
    {
        if (score % newEraFrequency == 0)
            PassToNewEra();

        if (respawn)
        {
            RemoveAllTraps();
            RemoveAllOthers();
        }

        SpawnOthers(score);
        SpawnTraps(score);
    }

    private static void SpawnOthers(int score)
    {
        SpawnDiamond();

        if (score % extraHealthSpawnFrequency == 0)
            SpawnExtraHealth();
    }
    private static void RemoveAllOthers()
    {
        StaticCoroutine.DoCoroutine(DestroyDiamond(0.25f));
    }

    private static void SpawnTraps(int score)
    {
        int deadlyCircleCount = Mathf.FloorToInt(score / deadlyCircleIncreaseFrequency) + deadlyCircleCountOffset;

        if (score % lineGuardSpawnFrequency == 0)
            deadlyCircleCount -= 2 * SpawnLineGuardsWithScore(score); // Oluşturulan Çizgi Gardiyanı Sayısının İki Katı Kadar Oluşturalacak Ölümcül Daire Sayısını Azalt.

        SpawnDeadlyCirclesWithScore(deadlyCircleCount);
    }
    private static void RemoveAllTraps()
    {
        RemoveAllLineGuards();
        RemoveAllDeadlyCircles();
    }

    #region Spawn Location
    /// <summary>
    /// Geriye Rastgele True Olmayan Bir locations Değerinin İndexlerinin Konumlaşmış Halini Döndürür. Yani Boş Olan Bir Konum Döndürür.
    /// </summary>
    /// <param name="toBeUsed">Eğer Geri Döndürülen Konum Kullanılacaksa Geri Döndürülen Konumun locaitons'daki Değerini True Yapar</param>
    /// <returns></returns
    public static Vector3 GetEmptySpawnLocation(bool toBeUsed)
    {
        int x, y;
        int xForArray, yForArray;
        Vector3 location;

        x = UnityEngine.Random.Range(-4, 4);
        y = UnityEngine.Random.Range(-4, 4);

        xForArray = x + 4;
        yForArray = y + 4;

        if (locations[xForArray, yForArray] == true)
        {
            for (int i = xForArray; i <= 8; i++)
            {
                for (int j = yForArray; j <= 8; j++)
                    if (locations[i, j] != true) { x = i - 4; y = j - 4; xForArray = i; yForArray = j; goto cont; }
                for (int j = yForArray; j >= 0; j--)
                    if (locations[i, j] != true) { x = i - 4; y = j - 4; xForArray = i; yForArray = j; goto cont; }
            }

            for (int i = xForArray; i >= 0; i--)
            {
                for (int j = yForArray; j <= 8; j++)
                    if (locations[i, j] != true) { x = i - 4; y = j - 4; xForArray = i; yForArray = j; goto cont; }
                for (int j = yForArray; j >= 0; j--)
                    if (locations[i, j] != true) { x = i - 4; y = j - 4; xForArray = i; yForArray = j; goto cont; }
            }
        }
    cont:
        location = new Vector3(x, y, -1);
        if (toBeUsed)
            ProtectLocation(location);
        return location;
    }
    public static void UnprotectLocation(Vector2 location)
    {
        locations[(int)location.x + 4, (int)location.y + 4] = false;
    }
    public static void ProtectLocation(Vector2 location)
    {
        locations[(int)location.x + 4, (int)location.y + 4] = true;
    }
    #endregion

    #region Deadly Circle
    private static void SpawnDeadlyCirclesWithScore(int deadlyCircleCount)
    {
        Transform lines = GameObject.Find("Lines").GetComponent<Transform>();

        if (deadlyCircleCount != 0)
        {
            if (deadlyCircleCount > maxCircleCount)
                deadlyCircleCount = maxCircleCount;
            GameObject[] deadlyCircles;
            deadlyCircles = new GameObject[deadlyCircleCount];

            for (int i = 0; i < deadlyCircleCount; i++)
                deadlyCircles[i] = NewDeadlyCircle(lines);

            EnableAllDeadlyCircles(deadlyCircles);
        }
    }
    private static void RemoveAllDeadlyCircles()
    {
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");

        foreach (GameObject go in traps)
            if (go.name == "DeadlyCircle")
            {
                UnprotectLocation(go.GetComponent<Transform>().position);
                StaticCoroutine.DoCoroutine(DestroyDeadlyCircle(0.25f, go));
            }
    }
    private static void EnableAllDeadlyCircles(GameObject[] deadlyCircles)
    {
        foreach (GameObject go in deadlyCircles)
            StaticCoroutine.DoCoroutine(EnableDeadlyCircle(1, go));
    }

    private static GameObject NewDeadlyCircle(Transform parent)
    {
        GameObject deadlyCircle = Instantiate(GameObject.Find("DeadlyCircle0_"), GetEmptySpawnLocation(true), Quaternion.Euler(0, 0, 0));
        deadlyCircle.GetComponent<Transform>().SetParent(parent);
        deadlyCircle.name = "DeadlyCircle";
        return deadlyCircle;
    }

    private static IEnumerator EnableDeadlyCircle(float delayTime, GameObject deadlyCircle)
    {
        yield return new WaitForSeconds(delayTime);
        if (deadlyCircle != null)
            deadlyCircle.GetComponent<BoxCollider2D>().enabled = true;
    }
    private static IEnumerator DestroyDeadlyCircle(float delayTime, GameObject deadlyCircle)
    {
        deadlyCircle.name = "DeadlyCircle (Will Be Destroyed)";
        deadlyCircle.GetComponent<Animation>().Play("OnDestroyDeadlyCircle");
        deadlyCircle.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(delayTime);
        if (deadlyCircle != null)
        {
            Destroy(deadlyCircle);            
        }
    }
    #endregion

    #region Diamond
    public static void SpawnDiamond()
    {
        GameObject diamond = Instantiate(GameObject.Find("Diamond_"), GetEmptySpawnLocation(true), Quaternion.Euler(0, 0, 0));
        diamond.GetComponent<Transform>().SetParent(GameObject.Find("Lines").GetComponent<Transform>());
        diamond.name = "Diamond";
    }
    private static IEnumerator DestroyDiamond(float delayTime)
    {
        GameObject diamond = GameObject.Find("Diamond");
        diamond.name = "Diamond(Will Be Destroyed)";
        diamond.GetComponent<Animation>().Play("OnDestroyDiamond");
        yield return new WaitForSeconds(delayTime);
        if (diamond != null)
        {
            Spawner.UnprotectLocation(diamond.GetComponent<Transform>().position);
            Destroy(diamond);
        }
    }
    #endregion

    #region Extra Health
    public static void SpawnExtraHealth()
    {
        GameObject extraHealth = Instantiate(GameObject.Find("ExtraHealth_"), GetEmptySpawnLocation(true), Quaternion.Euler(0, 0, 0));
        extraHealth.name = "ExtraHealth";
    }
    private static IEnumerator DestroyExtraHealth(float delayTime, GameObject extraHealth)
    {
        extraHealth.name = "ExtreHealth(Will Be Destroyed)";
        extraHealth.GetComponent<Animation>().Play("OnDestroyDiamond");
        yield return new WaitForSeconds(delayTime);

        if (extraHealth != null)
        {
            Spawner.UnprotectLocation(extraHealth.GetComponent<Transform>().position);
            Destroy(extraHealth);
        }
    }
    public static void RemoveExtraHealth(GameObject extraHealth)
    {
        StaticCoroutine.DoCoroutine(DestroyExtraHealth(0.25f, extraHealth));
    }
    #endregion

    #region Turn Effect
    public static void SpawnTurnEffect(Vector2 position)
    {
        GameObject CloneTurnEffect = Instantiate(GameObject.Find("TurnEffect"), position , Quaternion.Euler(0, 0, 0));
        StaticCoroutine.DoCoroutine(DestroyTurnEffect(0.5f, CloneTurnEffect));
    }
    private static IEnumerator DestroyTurnEffect(float delayTime, GameObject turnEffect)
    {
        turnEffect.name = "TurnEffect (Will Be Destroyed)";
        yield return new WaitForSeconds(delayTime);
        Destroy(turnEffect);
    }
    #endregion

    #region Trail Piece
    public static void SpawnTrailPiece(Vector3 location)
    {
        GameObject trailPiece = Instantiate(GameObject.Find("TrailPiece_"), location, Quaternion.Euler(0, 0, 0)); ;
        StaticCoroutine.DoCoroutine(DestroyTrailPiece(3, trailPiece));
    }
    private static IEnumerator DestroyTrailPiece(float delayTime, GameObject trailPiece)
    {
        yield return new WaitForSeconds(delayTime);
        Destroy(trailPiece);
    }
    #endregion

    #region Line Guard
    /// <summary>
    /// Geriye Oluşturulan Çizgi Gardiyanı Sayısını Döndürür.
    /// </summary>
    /// <param name="score">Skora Göre Çizgi Gardiyanı Ekler</param>
    /// <returns></returns>
    private static int SpawnLineGuardsWithScore(int score)
    {
        int lineGuardCount = Mathf.FloorToInt(score / lineGuardIncreaseFrequency);
        if (lineGuardCount == 0)
            lineGuardCount++;
        else if (lineGuardCount > maxLineGuardCount)
            lineGuardCount = maxLineGuardCount;

        GameObject[] lineGuards = new GameObject[lineGuardCount];

        for (int i = 0; i < lineGuardCount; i++)
            lineGuards[i] = NewLineGuard();

        EnableAllLineGuards(lineGuards);
        return lineGuardCount;
    }

    private static GameObject NewLineGuard()
    {
        Vector3 spawnLocation;
        int horizontalOrVertical = Random.Range(0, 2);  // Horizontal: 1, Vertical: 0
        if (horizontalOrVertical == 1)
        {
            int yLocation = (int)GetEmptySpawnLocation(false).y;

            spawnLocation = new Vector2(0, yLocation);
            for (int i = -4; i <= 4; i++)
                ProtectLocation(new Vector2(i, yLocation));
        }
        else
        {
            int xLocation = (int)GetEmptySpawnLocation(false).x;

            spawnLocation = new Vector2(xLocation, 0);
            for (int i = -4; i <= 4; i++)
                ProtectLocation(new Vector2(xLocation, i));
        }

        GameObject lineGuard = Instantiate(GameObject.Find("LineGuardField_"), spawnLocation + new Vector3(0, 0, 0.1f), Quaternion.Euler(0, 0, 90 * horizontalOrVertical));
        return lineGuard;
    }

    private static void RemoveAllLineGuards()
    {
        GameObject[] lineGuardFields = GameObject.FindGameObjectsWithTag("LineGuardField");
        if (lineGuardFields.Length != 0)
        {
            foreach (GameObject go in lineGuardFields)
                if (go.name == "LineGuardField")
                    StaticCoroutine.DoCoroutine(DestroyLineGuard(0.5f, go));
        }

    }
    private static void EnableAllLineGuards(GameObject[] lineGuards)
    {
        foreach (GameObject go in lineGuards)
            StaticCoroutine.DoCoroutine(EnableLineGuard(1.5f, go));
    }

    private static IEnumerator EnableLineGuard(float delayTime, GameObject lineGuard)
    {
        lineGuard.name = "LineGuardField";
        yield return new WaitForSeconds(delayTime);
        if (lineGuard != null)
        {
            lineGuard.GetComponentInChildren<BoxCollider2D>().enabled = true;
            lineGuard.GetComponent<Animator>().enabled = true;
        }
    }
    private static IEnumerator DestroyLineGuard(float delayTime, GameObject lineGuard)
    {
        int x = (int)lineGuard.GetComponent<Transform>().position.x;
        int y = (int)lineGuard.GetComponent<Transform>().position.y;

        lineGuard.GetComponent<Animator>().enabled = false;
        lineGuard.GetComponent<Animation>().Play("OnDestroyLineGuardField");
        lineGuard.GetComponentInChildren<BoxCollider2D>().enabled = false;

        if (lineGuard.GetComponent<Transform>().rotation.z == 0)
            for (int i = -4; i <= 4; i++)
                UnprotectLocation(new Vector2(x, i));
        else
            for (int i = -4; i <= 4; i++)
                UnprotectLocation(new Vector2(i, y));


        yield return new WaitForSeconds(delayTime);
        if (lineGuard != null)
            Destroy(lineGuard);
    }
    #endregion

    #region New Era
    public static void PassToNewEra()
    {
        if (era != maxEra)
        {
            era++;
            RenderSettings.skybox = Resources.Load(@"Skyboxes\Background" + era, typeof(Material)) as Material;
            Color UIColor = GeneralVariables.GetEraUIColor(era);
            Color color = GeneralVariables.GetEraColor(era);
            ColorImgPause(color, UIColor);
            ColorImgGameOver(color, UIColor);
            GameObject.Find("LineLights").GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0);
            GameObject.Find("ArrowMark").GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0);
            GameObject.Find("HorizontalMarker").GetComponent<SpriteRenderer>().color = color;
            GameObject.Find("VerticalMarker").GetComponent<SpriteRenderer>().color = color;
            GameObject.Find("Messenger").GetComponent<Messenger>().AddToMessageList("New Era");
            Quake();
            AnimateObjects();
        }
    }

    private static void ColorImgGameOver(Color color, Color UIColor)
    {
        GameObject.Find("ImgGameOver").GetComponent<Image>().color = UIColor;
        GameObject.Find("TxtRetry").GetComponent<Text>().color = color;
        GameObject.Find("TxtBackToMM").GetComponent<Text>().color = color;
        GameObject.Find("TxtLastScore").GetComponent<Text>().color = color;
        GameObject.Find("TxtLastScoreString").GetComponent<Text>().color = color;

        Image[] upArrows = GameObject.Find("ImgDragUpArrow").GetComponentsInChildren<Image>();
        foreach (Image arrow in upArrows)
            arrow.color = color;

        Image[] downArrows = GameObject.Find("ImgDragDownArrow").GetComponentsInChildren<Image>();
        foreach (Image arrow in downArrows)
            arrow.color = color;

        Image[] scoreFrames = GameObject.Find("LastScoreFrame").GetComponentsInChildren<Image>();
        foreach (Image frame in scoreFrames)
            frame.color = color;
    }
    private static void ColorImgPause(Color color, Color UIColor)
    {
        GameObject.Find("ImgPause").GetComponent<Image>().color = UIColor;
        GameObject.Find("TxtPaused").GetComponent<Text>().color = color;
        GameObject.Find("TxtBackToMMP").GetComponent<Text>().color = color;
        GameObject.Find("TxtReplay").GetComponent<Text>().color = color;
        GameObject.Find("TxtResume").GetComponent<Text>().color = color;

        Image[] btnBackToMM = GameObject.Find("BtnBackToMM").GetComponentsInChildren<Image>();
        foreach (Image i in btnBackToMM)
            i.color = color;

        Image[] btnReplay = GameObject.Find("BtnReplay").GetComponentsInChildren<Image>();
        foreach (Image i in btnReplay)
            i.color = color;

        Image[] btnResume = GameObject.Find("BtnResume").GetComponentsInChildren<Image>();
        foreach (Image i in btnResume)
            i.color = color;
    }
    private static void AnimateObjects()
    {
        GameObject.Find("Lines").GetComponent<Animation>().Play("NewEra_" + era);
        GameObject.Find("ImgUpPanel").GetComponent<Animation>().Play("ImgNewEra_" + era);
        if (GameObject.Find("ImgDownPanel") != null)
            GameObject.Find("ImgDownPanel").GetComponent<Animation>().Play("ImgNewEra_" + era);
    }
    #endregion

    #region CamShake
    private static IEnumerator UnableCamShake(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Camera.main.GetComponent<Animation>().enabled = false;
        Camera.main.GetComponent<Transform>().position = new Vector3(0, 0, -10);
    }

    private static void Quake()
    {
        Camera.main.GetComponent<Animation>().Play("Cam");
        Camera.main.GetComponent<Animation>().enabled = true;
        StaticCoroutine.DoCoroutine(UnableCamShake(10));
    }
    #endregion
}
