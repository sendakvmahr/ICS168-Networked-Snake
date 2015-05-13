using System.Collections;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class StateObject
{
	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 256;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();
}

public static class AsynchronousClient
{
	// The port number for the remote device.
	
	public static GameState gameState = new GameState("to be replaced");
	private const int port = 11000;
	public static string playername; 

	
	public static bool gameStarted = false;
	// ManualResetEvent instances signal completion.
	private static ManualResetEvent connectDone =
		new ManualResetEvent(false);
	private static ManualResetEvent sendDone =
		new ManualResetEvent(false);
	private static ManualResetEvent receiveDone =
		new ManualResetEvent(false);

	private static Socket client;
	
	// The response from the remote device.
	private static String response = String.Empty;
	
	public static string StartClient(string username, string password)
	{
		// Connect to a remote device.
		playername = username;
		gameState = new GameState (playername);
		try
		{
			// Establish the remote endpoint for the socket.
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			//IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
			
			// Create a TCP/IP socket.
			client = new Socket(ipAddress.AddressFamily,
			                           SocketType.Stream, ProtocolType.Tcp);
			
			// Connect to the remote endpoint.
			client.BeginConnect(remoteEP,
			                    new AsyncCallback(ConnectCallback), client);
			connectDone.WaitOne();
			
			// Login
			Send(client, "ICS168 Snake Project Start <EOF>");
			sendDone.WaitOne();

			// Receive the response from the remote device.
			Receive(client);
			receiveDone.WaitOne();
			
			// Write the response to the console.

			
			Send(client, "login " + username + " " + password + " <EOF>");
			sendDone.WaitOne();
			
			response = "";
			// Receive the response from the remote device.

			Receive(client);
			receiveDone.WaitOne();
			// Release the socket.
			/*
			try
			{
				client.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException e)
			{
				Debug.Log("Socket closed remotely");
			}
			client.Close();
			return response.Replace("<EOF>", "");
			*/
			
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
			return "Incorrect";
		}
		Debug.Log (response);
		return response.Replace("<EOF>", "");
	}
	
	private static void ConnectCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the socket from the state object.
			Socket client = (Socket)ar.AsyncState;
			
			// Complete the connection.
			client.EndConnect(ar);
			
			Console.WriteLine("Socket connected to {0}",
			                  client.RemoteEndPoint.ToString());
			
			// Signal that the connection has been made.
			connectDone.Set();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}
	
	private static void Receive(Socket client)
	{
		try
		{
			// Create the state object.
			StateObject state = new StateObject();
			state.workSocket = client;
			
			// Begin receiving the data from the remote device.
			client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveCallback), state);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}
	
	private static void ReceiveCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the state object and the client socket 
			// from the asynchronous state object.
			StateObject state = (StateObject)ar.AsyncState;
			Socket client = state.workSocket;
			
			// Read data from the remote device.
			int bytesRead = client.EndReceive(ar);
			if (bytesRead > 0)
			{
				// There might be more data, so store the data received so far.
				state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
				string content = state.sb.ToString();
				
				//Debug.Log("Content");
				//Debug.Log(content);
				if (content.IndexOf("<EOF>") > -1)
				{
					response = state.sb.ToString();
					receiveDone.Set();
					StateObject newstate = new StateObject();
					newstate.workSocket = client;
					if (response[0] == '{')
					{
						//Debug.Log (response);
						// Call BeginReceive with a new state object
						// NOTE THIS IS WHERE YOU TRY TO KEEP ON LISTENING FOR UPDATES
						if (gameStarted) {
							// Time to update the game
							gameState.update(response.Replace ("<EOF>", ""));
							Send (client, "gamestate: " + gameState.ToJSON() + "<EOF>");
							Debug.Log (gameState.ToJSON());
						}
						else {
							Debug.Log ("not a gamestate");
						}
					}
					client.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0,
					                    new AsyncCallback(ReceiveCallback), newstate);
				}
				else
				{
					// Get the rest of the data.
					client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
					                    new AsyncCallback(ReceiveCallback), state);
				}
			}
			else
			{
				// All the data has arrived; put it in response.
				if (state.sb.Length > 1)
				{
					response = state.sb.ToString();
				}
				// Signal that all bytes have been received.
				receiveDone.Set();
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	private static void Send(Socket client, String data)
	{
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
		                 new AsyncCallback(SendCallback), client);
	}
	
	private static void SendCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the socket from the state object.
			Socket client = (Socket)ar.AsyncState;
			
			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);
			Console.WriteLine("Sent {0} bytes to server.", bytesSent);
			
			// Signal that all bytes have been sent.
			sendDone.Set();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}
}
