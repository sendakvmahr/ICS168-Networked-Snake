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
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        // Tells threads when signals have occurred.
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        // List of clients (for future, must send updates to all clients)
        public static List<Socket> clients = new List<Socket>();
        // Connection to sqlite database
        static DatabaseConnection database = new DatabaseConnection();

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];
            // Figure out how to limit properly later
            // IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            // IPAddress ipAddress = ipHostInfo.AddressList[0];
            // IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // For now, can accept from any ip address
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);

            // listener socket is an IPV4, Stream, TCP socket
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint); 
                listener.Listen(20);  // 20 pending connections max

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();
                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            clients.Add(handler);
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n\tData : {1}",
                        content.Length, content);
                    if (content == "ICS168 Snake Project Start <EOF>")
                    {
                        Send(handler, "Ready<EOF>");
                        Console.WriteLine("Ready");
                    }
                    else if (content.StartsWith("login"))
                    {
                        string[] information = content.Split(' ');
                        string user = information[1];
                        string password = information[2];
                        if (database.UserIsInDatabase(user))
                        {
                            if (database.IsCorrectPassword(user, password))
                            {
                                Send(handler, "Allowed<EOF>");
                                Console.WriteLine("Allowed");
                            }
                            else
                            {
                                Send(handler, "Incorrect<EOF>");
                                Console.WriteLine("Incorrect");
                            }
                        }
                        else
                        {
                            database.AddNewUser(user, password);
                            Send(handler, "Created<EOF>");
                            Console.WriteLine("Incorrect");
                        }
                    }
                    else // Incorrect protocol
                    {
                        Console.WriteLine("Incorrect protocol.");
                    }

                    // Setup a new state object
                    StateObject newstate = new StateObject();
                    newstate.workSocket = handler;

                    // Call BeginReceive with a new state object
                    handler.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), newstate);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII or Unicode encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent, "\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}