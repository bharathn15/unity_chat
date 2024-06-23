using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{

    [SerializeField] private string userId;
    [SerializeField] private string userName;
    [SerializeField] private bool isConnected;
    string privateReceiver = "";
    string currentChat;
    [SerializeField] private TMP_InputField chatField;
    [SerializeField] private TMP_Text chatDisplay;
    [SerializeField] private UnityEngine.UI.Button joinChatButton;


    ChatClient chatClient;
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private GameObject joinChatInputPanel;


    #region Monobehaviour callbacks
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected)
        {
            chatClient.Service();
        }
        if(!string.IsNullOrEmpty(chatField.text) && Input.GetKey(KeyCode.Return))
        {
            SubmitPublicChatOnClick();
            SubmitPrivateChatOnClick();
        }
        
    }
    #endregion

    public void UserNameOnValueChanged(string value)
    {
        userName = value;
    }

    public void ChatConnectOnClick()
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userName));
        Debug.Log($"<color=green>{nameof(ChatConnectOnClick)} \t Connecting...........</color>");

    }

    public void TypeChatOnvalueChanged(string value)
    {
        currentChat = value;    
    }

    public void SubmitPublicChatOnClick()
    {
        if(privateReceiver == string.Empty)
        {
            chatClient.PublishMessage("RegionChannel", currentChat);
            chatField.text = "";
            currentChat = "";
        }
    }

    

    public void SubmitPrivateChatOnClick()
    {
        if(privateReceiver != string.Empty)
        {
            chatClient.SendPrivateMessage(privateReceiver, currentChat);
            chatField.text = "";
            currentChat = "";
        }
    }

    public void ReceiverOnValueChanged(string value)
    {
        privateReceiver = value;
    }



    #region IChatClientListener callbacks
    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnConnected()
    {
        isConnected = true;
        joinChatButton.gameObject.SetActive(false);
        chatClient.Subscribe(new string[] { "RegionChannel" });
        Debug.Log($"<color=green>{nameof(OnConnected)}</color>");
    }

    public void OnDisconnected()
    {
        Debug.Log($"<color=red>{nameof(OnDisconnected)}</color>");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string message = string.Empty;
        for(int i = 0; i < senders.Length; i++)
        {
            message = $"{senders[i]}: {messages[i]}";
            chatDisplay.text += "\n " + message;
            Debug.Log($"message : {message}");
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string msgs = "";
        msgs = $"Private {sender}: {message}";
        chatDisplay.text += "\n " + msgs;
        Debug.Log($"{nameof(OnPrivateMessage)} \t msgs {msgs}");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"{nameof(OnStatusUpdate)} \t user : {user} added to your list \t status : {status} \t gotMessage {gotMessage} \t message {message}");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        joinChatInputPanel.SetActive(false);
        chatPanel.SetActive(true);
        for(int i = 0;i < channels.Length;i++)
        {
            Debug.Log($"<color=orange>{nameof(OnSubscribed)} \t channel {i} : {channels[i]} with result {i}: {results[i]}</color>");
        }
        
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"<color=orange>{nameof(OnUserSubscribed)} \t user : {user} has subscribed to channel : {channel}</color>");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"<color=red>{nameof(OnUserUnsubscribed)} \t user : {user} has unsubscribed from channel : {channel}</color>");
    }
    #endregion
}
