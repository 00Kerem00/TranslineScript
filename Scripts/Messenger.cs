using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour
{
    public Text messengerText, effectText;

    ArrayList messageList = new ArrayList();

    public void AddToMessageList(string message)
    {
        messageList.Add(message);
        if (messageList.Count == 1)
            ShowMessage();
    }

    public void ShowMessage()
    {
        SetText(messageList[0].ToString());
        GetComponent<Animation>().Stop();
        GetComponent<Animation>().Play("Messenger");
    }

    public void ShowNextMessage()
    {
        messageList.RemoveAt(0);
        if(messageList.Count == 0)
            return;

        ShowMessage();
    }

    private void SetText(string text)
    {
        messengerText.text = text;
        effectText.text = text;
    }
}
