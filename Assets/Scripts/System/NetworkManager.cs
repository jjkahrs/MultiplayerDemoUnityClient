using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class NetworkManager : MonoBehaviour
{
    private const int PORT = 4201;
    private const string HOST_IP = "127.0.0.1";
    private const int READ_BUFFER_SIZE = 1024;
    
    private GameMaster _GM;
    private TcpClient tcpClient;
	private NetworkStream stream;
	private StreamWriter writer;

    private List<string> messages = new List<string>();

    void Awake()
    {
        _GM = GameObject.FindObjectOfType<GameMaster>();
    }

    public void Connect(string sessionToken )
    {
        StartCoroutine( ConnectToServer( HOST_IP, PORT, sessionToken ) );
    }

    private IEnumerator ConnectToServer(string ipString, int port, string sessionToken )
    {
        yield return null;
        try
        {
            tcpClient = new TcpClient( ipString, port );
            stream = tcpClient.GetStream();
            writer = new StreamWriter( stream );

            Debug.Log($"tcpClient.Connected {tcpClient.Connected}");
            _GM.DispatchEvent( new NetworkConnectionAcceptedEvent() );

            Send($"Token:{sessionToken}");
        }
        catch( Exception ex )
        {
            _GM.DispatchEvent( new NetworkConnectionErrorEvent() );
            Debug.LogError($"ERROR Connecting to server: {ex.Message}");
        }

        yield return null;
    }

    public void Send( string data )
    {
        if( tcpClient != null && tcpClient.Connected )
        {
            writer.Write( data + ';' );
            writer.Flush();
        }
    }

    public bool HasPendingData()
    {
        return messages.Count > 0;
    }

    private string NextMessage()
    {
        if( messages.Count == 0)
            return null;

        StringBuilder sb = new StringBuilder();
        foreach( string s in messages )
            sb.Append(s);

        string data = sb.ToString();
        int endIndex = data.IndexOf(";");

        if( endIndex == -1 )
            return null;

        string msg = data.Substring( 0, endIndex );
        string remainder = data.Substring( endIndex +1 );

        messages.Clear();
        messages.Add( remainder );
        return msg;
    }

    public List<string> ReadMessages()
    {
        List<string> messages = new List<string>();
        string msg = null; 

        do
        {
            msg = NextMessage();
            if( msg != null )
                messages.Add( msg );
        }
        while( msg != null );

        return messages;
    }

    void Update()
    {
        if( tcpClient != null && stream != null )
        {
            ReadAvailableData();
        }
            
    }

    public void ReadAvailableData()
    {
        if( tcpClient == null || stream == null )
            return;

        byte[] buffer = new byte[READ_BUFFER_SIZE];
        int bytesRead = 0;

        if( stream.DataAvailable && stream.CanRead )
        {
            bytesRead = stream.Read( buffer, 0, buffer.Length );
            string data = System.Text.Encoding.UTF8.GetString( buffer.AsSpan( 0, bytesRead ) );
            messages.Add(data);
        }
    }
 
    void OnDestroy()
    {
        writer?.Close();
        stream?.Close();
        tcpClient?.Close();
    }
}
