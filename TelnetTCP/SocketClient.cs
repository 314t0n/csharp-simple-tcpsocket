using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TelnetTCP
{
    public class SocketClient : IDisposable
    {
        private Socket _socket;
        private StreamWriter _writer;

        public SocketClient(Socket socket)
        {
            _socket = socket;
            try
            {
                _writer = new StreamWriter(new NetworkStream(socket));
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }
        }

        public void Send(string message)
        {
            _writer.Write(message);
            _writer.Flush();
        }

        #region :: IDisposable ::

        public void Dispose()
        {
            try
            {
                if (_writer != null)
                {
                    _writer.Close();
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception ex)
            {

            }

            try
            {
                if (_socket != null)
                {
                    _socket.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
