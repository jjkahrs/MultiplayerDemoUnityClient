using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    private const string NETWORK_ERROR_MESSAGE = "ERROR: Unable to connect to server";

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private TMP_Text sessionIdValue;
    [SerializeField] private TMP_Text networkErrorText;
    [SerializeField] private Button connectButton;

    private GameMaster _GM;

    void Awake()
    {
        _GM = GameObject.FindObjectOfType<GameMaster>();
    }

    void Start()
    {
        _GM.Subscribe( typeof( NetworkConnectionAcceptedEvent ), OnConnectionAccepted, this );
        _GM.Subscribe( typeof( SessionConnectEvent ), OnSessionConnect, this );
        _GM.Subscribe( typeof( NetworkConnectionErrorEvent ), OnNetworkError, this );

        menuPanel.SetActive( true );
        hudPanel.SetActive( false );
        connectingPanel.SetActive( false );
        networkErrorText.text = "";
    }

    public void ConnectButtonPressed()
    {
        connectButton.gameObject.SetActive( false );
        connectingPanel.SetActive( true );
        networkErrorText.text = "";
        _GM.DispatchEvent( new RequestNetworkConnectionEvent() );
    }

    void OnConnectionAccepted( GameEvent gameEvent )
    {
        menuPanel.SetActive( false );
        connectButton.gameObject.SetActive( true );
        connectingPanel.SetActive( false );
    }

    void OnSessionConnect( GameEvent gameEvent )
    {
        if( gameEvent is SessionConnectEvent sce )
        {
            sessionIdValue.text = "SessionID: " + sce.sessionId;
            hudPanel.SetActive( true );
            networkErrorText.text = "";
        }
    }

    void OnNetworkError( GameEvent gameEvent )
    {
        connectButton.gameObject.SetActive( true );
        hudPanel.SetActive( false );
        menuPanel.SetActive( true ) ;
        connectingPanel.SetActive( false );
        networkErrorText.text = NETWORK_ERROR_MESSAGE;
    }

    void OnDestroy()
    {
        _GM?.UnsubscribeAll( this );
    }
}
