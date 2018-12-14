using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoCaRo
{
    class SocketManager
    {
        #region Client
        Socket client;

        public bool ConnectServer()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), PORT);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(iep);
                return true;

            }
            catch
            {
                return false;

            }
        }

        #endregion
        #region Server
        Socket server;
        public void CreateServer()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), PORT);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(10);
            Thread acceptClient = new Thread(() =>
             {
               client = server.Accept();
             });
            acceptClient.IsBackground = true;
            acceptClient.Start();
        }
        #endregion
        #region Both
        public string IP = "127.0.0.1";
        public int PORT = 9999;
        bool isServer;
        public const int Buffer = 1024;

        public bool IsServer { get => isServer; set => isServer = value; }

        public bool Send(object data)
        {
            byte[] sendData = SerializeData(data);
          
            return SendData(client, sendData);
        }
        public object Receive()
        {
            byte[] receiveData = new byte[Buffer];
            bool isOk = ReceiveData(client, receiveData);
            return DeserializeData(receiveData);
        }
        private bool SendData(Socket target,byte[] data)
        {
            return target.Send(data) == 1 ? true : false;
        }
        private bool ReceiveData(Socket target, byte[] data)
        {
            return target.Receive(data) == 1 ? true : false;
        }
        //Nen 1 doi tuong thanh mang byte
        public byte[] SerializeData(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }
        //Giai nen 1 mang byte duoc gui toi thanh 1 doi tuong (object)
        public object DeserializeData(byte[] theByteArr)
        {
            MemoryStream ms = new MemoryStream(theByteArr);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;
            return bf1.Deserialize(ms);

        }

        //Lay ra IP v4 cua card mang dang dung
        public string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach(NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(item.NetworkInterfaceType==_type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach(UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }
        #endregion
    }
}
