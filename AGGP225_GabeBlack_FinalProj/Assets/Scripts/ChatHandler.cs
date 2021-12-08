using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatHandler : MonoBehaviour
{
	public static ChatHandler instance { get; set; }

	public RectTransform TextHolder;
	public GameObject textPrefab;
	public int maxMessages = 10;

	[SerializeField]
	public List<Message> messageList = new List<Message>();

	void Awake()
	{
		instance = this;
	}

	public void SendMessageToChat(string text)
	{
		if (messageList.Count >= maxMessages)
		{
			Destroy(messageList[0].textObj.gameObject);
			messageList.Remove(messageList[0]);
		}

		Message newMessage = new Message();

		newMessage.text = text;

		GameObject newTextObj = Instantiate(textPrefab, TextHolder);

		newMessage.textObj = newTextObj.GetComponent<TMP_Text>();

		newMessage.textObj.text = newMessage.text;

		messageList.Add(newMessage);
	}
}


[System.Serializable]
public class Message
{
	public string text;
	public TMP_Text textObj;

	public override string ToString()
	{
		return text;
	}
}
