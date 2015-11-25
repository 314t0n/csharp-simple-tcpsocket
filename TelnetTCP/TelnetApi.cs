using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TelnetTCP
{
    public class TelnetApi : IDisposable
    {
        #region :: Properties ::

        private const int MaxConnections = 10;

        private readonly List<SocketClient> _clients = new List<SocketClient>();
        private Socket _serverSocket;

        public string Address { get; set; }
        public int Port { get; set; }

        #endregion

        #region :: Constructor ::

        public TelnetApi(int port)
        {
            Port = port;
            Init();
        }

        public TelnetApi(string address, int port)
        {
            Address = address;
            Port = port;
            Init();
        }

        #endregion

        protected void Init()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(new AsyncCallback(OnConnect), null);
        }

        private void OnConnect(IAsyncResult asyncResult)
        {
            try
            {
                // Block until a client connects
                Socket socket = _serverSocket.EndAccept(asyncResult);

                SocketClient client = new SocketClient(socket);

                int currentActiveConnectionsCount = _clients.Count;
                if (currentActiveConnectionsCount < MaxConnections)
                {
                    try
                    {
                        //client.Send(""); here we could handle some handshake
                        AddClient(client);
                    }
                    catch
                    {
                        client.Dispose();
                    }
                }
                else
                {
                    //client.Send("Sorry - Too many connections.\r\n");
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (_serverSocket != null)
                {
                    _serverSocket.BeginAccept(new AsyncCallback(OnConnect), null);
                }
            }
        }

        public void BroadCastData(Object o)
        {
            foreach (var client in _clients)
            {
                SendClientMessage(client, o.ToString());
            }
        }

        private void SendClientMessage(SocketClient client, string message)
        {
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                client.Dispose();
            }
        }

        private void AddClient(SocketClient client)
        {
            lock (this)
            {
                _clients.Add(client);
            }
        }

        private void RemoveClient(SocketClient client)
        {
            lock (this)
            {
                _clients.Remove(client);
            }
        }

        #region :: IDisposable ::

        public void Dispose()
        {
            foreach (SocketClient socketClient in _clients)
            {
                socketClient.Dispose();
            }
            _clients.Clear();

            try
            {
                _serverSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {

            }

            try
            {
                _serverSocket.Close();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
