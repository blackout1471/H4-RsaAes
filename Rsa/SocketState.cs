using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class SocketState
    {
        public int BufferSize { get; private set; }
        public byte[] Buffer { get; private set; }
        public Socket WorkerSocket { get; private set; }
        public StringBuilder Builder { get; private set; }

        public SocketState(int bufferSize, Socket workerSocket)
        {
            BufferSize = bufferSize;
            WorkerSocket = workerSocket;
            Buffer = new byte[BufferSize];
            Builder = new StringBuilder();
        }
    }
}
