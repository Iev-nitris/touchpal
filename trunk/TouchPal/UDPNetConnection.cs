using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TouchPal
{
    class UDPNetConnection : INetConnection
    {
        private ControlManager manager = null;
        private Socket socket = null;
        private EndPoint client = null;
        private string clientID = "";

        public AsyncCallback socketDataCallback = null;
        private byte[] dataBuffer = new byte[2048];

        public UDPNetConnection()
        {
            socketDataCallback = new AsyncCallback(OnDataReceived);

            socket = new Socket(AddressFamily.InterNetwork,
                                      SocketType.Dgram,
                                      ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 9089));

            client = new IPEndPoint(IPAddress.Any, 0);
        }

        public void StartConnection()
        {
            TouchPal.Debug("Starting UDP server.");
            WaitForData();
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
                if (clientID.Length > 0)
                {
                    byte[] sendData = System.Text.Encoding.ASCII.GetBytes(data + "\n");
                    socket.SendTo(sendData, client);
                }
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
                // Start receiving any data written by the connected client
                // asynchronously
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                socket.BeginReceiveFrom(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, ref remote, socketDataCallback, null);
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
                iRx = socket.EndReceiveFrom(asyn, ref client);

                char[] chars = new char[iRx-1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(dataBuffer,
                                         0, iRx-1, chars, 0);
                String szData = new System.String(chars);

                if (szData[0].Equals('*'))
                {
                    if (szData[1].Equals('C'))
                    {
                        string id = szData.Substring(2);
                        if (!id.Equals(clientID))
                        {
                            clientID = id;
                            SendData("A\n");
                            TouchPal.Debug("Client (" + id + ") connected.");
                            manager.ClientConnected();
                        }
                    }
                    if (szData[1].Equals('R') && manager != null)
                    {
                        manager.ResetControls();
                        SendData("A\n");
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
                                SendData("A\n");
                            }
                            catch (FormatException fe)
                            {
                                TouchPal.Error("Bad network id value (" + values[0] + ") received from client - " + fe.Message);
                            }
                        }
                    }
                }
                WaitForData();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
    }
}
