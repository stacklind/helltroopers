using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkClient : MonoBehaviour
{
    private readonly object _lock = new object();
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    private readonly int MAX_ACTIONS_PER_FRAME = 5;

    private Socket socket;
    private Thread clientRecieveThread;

    private void Awake()
    {
        SetupWorker();
    }

    private void Update()
    {
        int completedActions = 0;
        lock(_lock)
        {
            while(mainThreadActions.Count > 0 && completedActions < MAX_ACTIONS_PER_FRAME)
            {
                Action action = mainThreadActions.Dequeue();

                action?.Invoke();

                completedActions++;
            }
        }
    }

    private void SetupWorker()
    {
        try
        {
            Debug.Log("Starting a networking thread");
            clientRecieveThread = new Thread(new ThreadStart(ListenForData));
            clientRecieveThread.IsBackground = true;
            clientRecieveThread.Start();
            Debug.Log("Networking thread started");
        }
        catch (Exception e)
        {
            Debug.LogError("Exception on networking thread setup: " + e.Message);
        }
    }

    private void ListenForData()
    {
        IPAddress[] IPAddresses = Dns.GetHostAddresses("localhost");

        Debug.Log("Establishing connection to " + "localhost");
        socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            Debug.Log("Connection established");

            byte[] recieveBuffer = new byte[512];
            // TO-DO listen for data
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket exception: " + e.Message);
        }
    }
    
    public int Send(string msg)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(msg);

        int bytesSent = 0;
        try
        {
            while (bytesSent < bytes.Length)
            {
                bytesSent += socket.Send(bytes, bytesSent, bytes.Length, SocketFlags.None);
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket exception: " + e.Message);
            return 0;
        }

        return 1;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Closing connections");
        if(socket != null)
        {
            socket.Close();
            Debug.Log("Socket closed");
        }
        if(clientRecieveThread != null)
        {
            clientRecieveThread.Abort();
            Debug.Log("Networking thread aborted");
        }
    }
}
