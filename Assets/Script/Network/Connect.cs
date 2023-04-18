using System.Net;
using System.Net.Sockets;


namespace NetworkCore
{
    public class Connect
    {
        Socket socket;
        SocketAsyncEventArgs AsycEvent;

        public Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            AsycEvent = new SocketAsyncEventArgs();
            AsycEvent.Completed += Set;
        }


        public void Init(EndPoint endpoint, int maxPlay, int backlog = 100)
        {

        }

        void Set(object sender, SocketAsyncEventArgs args)
        {

        }
    }


}
