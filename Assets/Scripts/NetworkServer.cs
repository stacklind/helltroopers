using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetworkServer : MonoBehaviour
{
    private readonly int PORT = 27515;
    private readonly int BACKLOG = 10;

    [SerializeField] private int numberOfPlayers;

    Socket listener;
    ServerWorker[] workerPool;
    Queue<Action> mainThreadActions = new Queue<Action>();

    private void Awake()
    {
        SetupWorkerPool();
        InitializeListeningSocket();
        ListenForConnections();
    }

    private void SetupWorkerPool()
    {
        Debug.Log("Setting up workerPool");
        workerPool = new ServerWorker[numberOfPlayers];
    }

    private void InitializeListeningSocket()
    {
      
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

        try
        {
            Debug.Log("Opening listening socket");
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(BACKLOG);
            Debug.Log("Socket successfully opened");
        }
        catch(Exception e)
        {
            Debug.LogError("Exception: " + e.Message);
        }
    }

    private void ListenForConnections()
    {
        int connectedPlayers = 0;

        while(connectedPlayers != numberOfPlayers)
        {
            Socket handler = listener.Accept();
            ServerWorker worker = new ServerWorker(handler);
            workerPool[connectedPlayers] = worker;
            worker.Start();
            connectedPlayers++;
        }
    }
}
