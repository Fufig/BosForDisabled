using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class code : MonoBehaviour
{
    Thread mThread;
    public GameObject[] points = null;
    int[,] CoordinatesForPoint = new int[21, 3];
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    Vector3 receivedPos = Vector3.zero;
    bool running;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        points = GameObject.FindGameObjectsWithTag("points");

        for (int i = 0; i < 21; i++)
        {
            points[i].transform.position = new Vector3(CoordinatesForPoint[i, 0], CoordinatesForPoint[i, 1], CoordinatesForPoint[i, 2]);
        }
    }
    void GetInfo()
    {
        Debug.Log("GetInfo");
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            SendAndReceiveData();
        }
        listener.Stop();
    }
    void SendAndReceiveData()
    {
        Debug.Log("SendAndRecieveData");
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];


        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Debug.Log(dataReceived);
        if (dataReceived != null)
        {
            string[] coords = dataReceived.Split(' ');


            int PointCounter = 0;

            for (int i = 0; i < coords.Length; i += 3)
            {
                CoordinatesForPoint[PointCounter, 0] = int.Parse(coords[i]);
                CoordinatesForPoint[PointCounter, 1] = int.Parse(coords[i + 1]);
                CoordinatesForPoint[PointCounter, 2] = int.Parse(coords[i + 2]);

                PointCounter += 1;

            }
            Debug.Log("kjdshi");

            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("мб пригодится");
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        }
    }
}


