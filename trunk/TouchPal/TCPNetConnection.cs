/*
 * This file is part of the TouchPal project hosted on Google Code
 * (http://code.google.com/p/touchpal). See the accompanying license.txt file for 
 * applicable licenses.
 */

/*
 * (c) Copyright Craig Courtney 2009 All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TouchPal
{
    class TCPNetConnection : INetConnection
    {
        private ControlManager manager = null;
        private Socket mainSocket = null;
        private Socket clientSocket = null;
        public AsyncCallback socketDataCallback = null;
        public AsyncCallback socketConnectCallback = null;
        private byte[] dataBuffer = new byte[2048];

        public TCPNetConnection()
        {
            socketDataCallback = new AsyncCallback(OnDataReceived);
            socketConnectCallback = new AsyncCallback(OnClientConnect);

            mainSocket = new Socket(AddressFamily.InterNetwork,
                                      SocketType.Stream,
                                      ProtocolType.Tcp);
            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, 9089);
            mainSocket.Bind(ipLocal);
            mainSocket.Listen(0);

            mainSocket.BeginAccept(socketConnectCallback, null);
            TouchPal.Debug("Waiting on client connect.");
        }

        public ControlManager Manager
        {
            get
            {
                return manager;
            }
            set
            {
                manager = value;
            }
        }

        public void SendData(string data)
        {
            try
            {
                if (clientSocket != null)
                {
                    byte[] sendData = System.Text.Encoding.ASCII.GetBytes(data + "\n");
                    clientSocket.Send(sendData);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                TouchPal.Debug("Client connected.");
                clientSocket = mainSocket.EndAccept(asyn);
                WaitForData();
                if (manager != null)
                    manager.ClientConnected();
            }
            catch (ObjectDisposedException)
            {
                TouchPal.Debug("Client disconnected.  Waiting on client connect.");
                clientSocket = null;
                mainSocket.BeginAccept(socketConnectCallback, null);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }

        }

        private void WaitForData()
        {
            try
            {
                if (clientSocket != null)
                {
                    // Start receiving any data written by the connected client
                    // asynchronously
                    clientSocket.BeginReceive(dataBuffer, 0,
                                       dataBuffer.Length,
                                       SocketFlags.None,
                                       socketDataCallback,
                                       null);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }

        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                int iRx = 0;
                iRx = clientSocket.EndReceive(asyn);

                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(dataBuffer,
                                         0, iRx, chars, 0);
                String szData = new System.String(chars);

                if (szData[0].Equals('*'))
                {
                    if (szData[1].Equals('Q'))
                    {
                        TouchPal.Debug("Client disconnected.  Waiting on client connect.");
                        clientSocket.Close();
                        clientSocket = null;
                        mainSocket.BeginAccept(socketConnectCallback, null);
                    }
                    else if (szData[1].Equals('R') && manager != null)
                    {
                        manager.ResetControls();
                    }
                }
                else if (manager != null)
                {
                    string[] controlUpdates = szData.Split(new Char[] { ':' });
                    foreach (string controlUpdate in controlUpdates)
                    {

                        string[] values = controlUpdate.Split(new Char[] { '=' });
                        if (values.Count() == 2)
                        {
                            try
                            {
                                int networkID = Convert.ToInt32(values[0]);
                                manager.UpdateControl(networkID,values[1]);
                            }
                            catch (FormatException fe)
                            {
                                TouchPal.Error("Bad network id value (" + values[0] + ") received from client.");
                            }
                        }
                    }
                }
                WaitForData();
            }
            catch (ObjectDisposedException)
            {
                TouchPal.Debug("Client disconnected.  Waiting on client connect.");
                mainSocket.BeginAccept(socketConnectCallback, null);
                clientSocket = null;
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
    }
}
