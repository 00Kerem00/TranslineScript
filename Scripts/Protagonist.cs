using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Protagonist : MonoBehaviour
{
    bool firstPass = false;
    public int health = 0;
    public int score = 0;
    public int bestScore = 0;
    public int[] currentCorner = { 0, 0 };

    public float speed;

    /// <summary>
    /// 0: Up, 1: Right, 2: Down, 3: Left
    /// </summary>
    public int direction = 0, rawDirection = 0;

    public Messenger messenger;
    public Text txtScore;
    public Text txtHealth;
    public Animation foreground;
    public Transform transformNextCornerMarker;
    public Animation animationNextCornerMarker;
    public Controlller controller;

    private void Start()
    {
        bestScore = DatabaseManager.GetBestScore();
        txtHealth = GameObject.Find("TxtHealth").GetComponent<Text>();
        LeaveTrail();
    }

    #region Movement
    private void Update()
    {
        Vector3 translateUnit;
        switch (direction)
        {
            case 0: translateUnit = new Vector3(0, speed * Time.deltaTime); break;
            case 1: translateUnit = new Vector3(speed * Time.deltaTime, 0); break;
            case 2: translateUnit = new Vector3(0, -speed * Time.deltaTime); break;
            case 3: translateUnit = new Vector3(-speed * Time.deltaTime, 0); break;
            default: translateUnit = new Vector3(0, 0); Debug.LogError("Invalid Direction!"); break;
        }
        transform.Translate(translateUnit);

        //Controller();
    }
    #endregion

    #region LeaveTrail
    private void LeaveTrail()
    {
        InvokeRepeating("LeaveTrailPiece", 0.5f, 0.1f);
    }
    private void LeaveTrailPiece()
    {
        Spawner.SpawnTrailPiece(transform.position);
    }
    #endregion

    #region OnContact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Line": OnContactLine(int.Parse(collision.gameObject.name)); break;
            case "Trap": OnContactTrap(); break;
            case "Diamond": OnContactDiamond(); break;
            case "ExtraHealth": OnContactExtraHealth(collision.gameObject); break;
            case "Wall": OnContactWall(); break;
        }
    }
    private void OnContactLine(int linePos)
    {
        switch (direction)
        {
            case 0: currentCorner[1] = linePos; break;
            case 1: currentCorner[0] = linePos; break;
            case 2: currentCorner[1] = linePos; break;
            case 3: currentCorner[0] = linePos; break;
        }
        transform.position = new Vector2(currentCorner[0], currentCorner[1]);

        if (direction != rawDirection)
        {
            Spawner.SpawnTurnEffect(transform.position);
            direction = rawDirection;
        }

        if (controllerHolding)
            MarkNextCorner();
        else if (firstPass)
            HideNextCornerMarker();
    }
    private void OnContactTrap()
    {
        if (health == 0)
        {
            GetComponent<Protagonist>().enabled = false;
            PPRManager.GameOver(score);
            AdvertManager.DestroyBanner();
            GameObject.Find("PS").GetComponent<OnStartedPS>().CancelInvoke("UpdateBanner");
        }
        else
        {
            health--; txtHealth.text = "X" + health.ToString(); foreground.Stop(); foreground.Play("Foreground4");
        }
    }
    private void OnContactDiamond()
    {
        score++;
        txtScore.text = score.ToString();
        txtScore.gameObject.GetComponent<Animation>().Play("TxtScore");
        if (score == bestScore + 1)
            messenger.AddToMessageList("New Best Score!");

        Spawner.SpawnObjects(score, true);

        GameObject.Find("LineLights").GetComponent<Animation>().Play("LineLight");
    }
    private void OnContactExtraHealth(GameObject extraHealth)
    {
        health++;
        txtHealth.text = "X" + health.ToString();
        Spawner.RemoveExtraHealth(extraHealth);
        messenger.AddToMessageList("Extra Health!");
    }
    private void OnContactWall()
    {
        switch (direction)
        {
            case 0: transform.Translate(new Vector2(0, -10)); break;
            case 1: transform.Translate(new Vector2(-10, 0)); break;
            case 2: transform.Translate(new Vector2(0, 10)); break;
            case 3: transform.Translate(new Vector2(10, 0)); break;
        }
        Spawner.SpawnTurnEffect(transform.position);
    }
    #endregion

    #region NextCorner
    public void MarkNextCorner()
    {
        Vector2 nextCornerMarkerLocation;

        switch (direction)
        {
            case 0: nextCornerMarkerLocation = new Vector2(currentCorner[0], currentCorner[1] + 1); break;
            case 1: nextCornerMarkerLocation = new Vector2(currentCorner[0] + 1, currentCorner[1]); break;
            case 2: nextCornerMarkerLocation = new Vector2(currentCorner[0], currentCorner[1] - 1); break;
            case 3: nextCornerMarkerLocation = new Vector2(currentCorner[0] - 1, currentCorner[1]); break;
            default: nextCornerMarkerLocation = new Vector2(0, 0); break;
        }

        transformNextCornerMarker.position = nextCornerMarkerLocation;
        animationNextCornerMarker.Stop();
        animationNextCornerMarker.Play("NextCornerMarker");

        firstPass = true;
    }
    private void HideNextCornerMarker()
    {
        animationNextCornerMarker.Play("NextCornerMarkerRewind");
        firstPass = false;
    }
    private bool controllerHolding { get { return controller.holding; } }
    #endregion
}

