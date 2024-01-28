using UnityEngine;
using UnityEngine.EventSystems;

public class Controlller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool holding;

    public GameObject ArrowMark;
    public Protagonist protagonist;
    private float defaultProtagonistSpeed;
    public float downProtagonistSpeed;
    public float upProtagonistSpeed;
    public float outrunTime;
    Vector2 pointDown, pointUp;

    private void Start()
    {
        defaultProtagonistSpeed = protagonist.speed;
    }

    public void OnPointerDown(PointerEventData EventData)
    {
        holding = true;
        protagonist.MarkNextCorner();

        pointDown = Camera.main.ScreenPointToRay(Input.mousePosition).origin;

        CancelInvoke("SetDefaultProtagonistSpeed");
        protagonist.speed = downProtagonistSpeed;
    }
    
    public void OnPointerUp(PointerEventData EventData)
    {
        holding = false;

        int direction;
        float x, y;

        pointUp = Camera.main.ScreenPointToRay(Input.mousePosition).origin;

        x = Mathf.Abs(pointDown.x - pointUp.x);
        y = Mathf.Abs(pointDown.y - pointUp.y);
        if (x > y)
            if (pointDown.x < pointUp.x)
                direction = 1;
            else direction = 3;
        else
            if (pointDown.y < pointUp.y)
            direction = 0;
        else direction = 2;

        AnimateArrowMark(direction);
        protagonist.rawDirection = direction;

        protagonist.speed = defaultProtagonistSpeed;
        //Invoke("SetDefaultProtagonistSpeed", outrunTime);
    }

    private void SetDefaultProtagonistSpeed()
    {
        protagonist.speed = defaultProtagonistSpeed;
    }

    private void AnimateArrowMark(int direction)
    {
        ArrowMark.GetComponent<Animation>().Stop();

        switch (direction)
        {
            case 0: ArrowMark.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0); break;
            case 1: ArrowMark.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, -90); break;
            case 2: ArrowMark.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 180); break;
            case 3: ArrowMark.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 90); break;
        }

        ArrowMark.GetComponent<Animation>().Play("ArrowMark");
    }
}
