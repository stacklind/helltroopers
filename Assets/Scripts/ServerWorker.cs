using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerWorker
{
    private readonly Socket socket;
    Queue<Action> workQueue = new Queue<Action>();

    public ServerWorker(Socket socket)
    {
        this.socket = socket;
        
    }

    public void Start()
    {
        new Thread(Work).Start();
    }

    private void Work()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int recievedBytes = socket.Receive(buffer, buffer.Length, SocketFlags.None);

            if (recievedBytes < 0) break;

            string message = Encoding.ASCII.GetString(buffer, 0, recievedBytes);

            Debug.Log(message);
        }
    }
}
