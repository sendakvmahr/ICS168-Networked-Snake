// Imported by default
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sockets
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleApplication
{
    class ConnectionStateObject
    {
        // Used so multiple clients can connect to the the socket
        public Socket clientSocket;
        public const int receiveBufferSize = 1024;
        public byte[] receiveBuffer = new byte[receiveBufferSize];
        public StringBuilder receivedString = new StringBuilder();
    }
    
    class ServerSocket
    {
        public static ManualResetEvent pause = new ManualResetEvent(false);

        public ServerSocket()
        {
            System.Console.WriteLine("Class successfully imported and initialized");
        }

        public void listen()
        {
            byte[] incomingData = new Byte[ConnectionStateObject.receiveBufferSize];
            
            // localEndPoint = the port on the server the socket binds to. 
            // host and localAddress are used to find localEndPoint
            IPHostEntry host = Dns.GetHostEntry("127.0.0.1");
            IPAddress localAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(localAddress, 32094);
            
            // listener socket is an IPV4, Stream, TCP socket
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Binding listener and assigning events
            listener.Bind(localEndPoint);
            listener.Listen(20); // 20 connections pending max

            // Listening loop
            while (true)
            {
                // Set so that there are no pending events, and the loop will continue 
                pause.Reset();

                // When an attempt is made to connect to listener, startHandShake will be called
                listener.BeginAccept(new AsyncCallback(startHandShake), listener);

                // Wait until a client is done connecting before restarting the loop
                pause.WaitOne();
            }
        }

        public static void startHandShake(IAsyncResult ar)
        {
        }
    }
}
