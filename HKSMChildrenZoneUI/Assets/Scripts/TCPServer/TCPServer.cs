/* Copyright (C) YU & CO. (LAB) LIMITED - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Eric Koo <eric.koo@yucolab.com>, May 2013
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace Yucolab
{

    /// <summary>
    /// Implements the core of a TcpServer socket listener.
    /// </summary>
    public class TcpServer
    {
        public delegate void TcpServerEventDlgt(object sender, TcpServerEventArgs e);
        public delegate void ApplicationExceptionDlgt(object sender, TcpLibApplicationExceptionEventArgs e);

        /// Event fires when a connection is accepted. Being multicast, this allows you to attach not only your application's event handler, but also other handlers, 
        /// such as diagnostics/monitoring, to the event.
        public event TcpServerEventDlgt Connected;
        public event TcpServerEventDlgt DataReceived;
        public event TcpServerEventDlgt Disconnected;

        /// This event fires when *your* application throws an exception that *you* do not handle in the interaction with the client. 
        /// You can hook this event to log unhandled exceptions, more as a tool to aid development rather than a suggested approach for handling your application errors.
        public event ApplicationExceptionDlgt HandleApplicationException;

        protected IPEndPoint endPoint;
        protected Socket listener;
        protected int pendingConnectionQueueSize;
        protected string delimiter;

        // ConnectionState array
        public List<ConnectionState> csList = new List<ConnectionState>();

        public YucoDebugger debugger;

        // === Get/Set functions ========================================================================================================================================

        /// Gets/sets pendingConnectionQueueSize. The default is 100.
        public int PendingConnectionQueueSize
        {
            get { return pendingConnectionQueueSize; }
            set
            {
                if (listener != null)
                {
                    YucoDebugger.instance.LogError("Listener has already started. Changing the pending queue size is not allowed.", "set PendingConnectionQueueSize", "TCPServer");
                    throw new TcpLibException("Listener has already started. Changing the pending queue size is not allowed.");
                }
                pendingConnectionQueueSize = value;
            }
        }

        /// Gets listener socket.
        public Socket Listener
        {
            get { return listener; }
        }

        /// Gets/sets endPoint
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
            set
            {
                if (listener != null)
                {
                    YucoDebugger.instance.LogError("Listener has already started. Changing the endpoint is not allowed.", "set EndPoint", "TCPServer");
                    throw new TcpLibException("Listener has already started. Changing the endpoint is not allowed.");
                }
                endPoint = value;
            }
        }

        // === Constructor functions ========================================================================================================================================

        /// Default constructor.
        public TcpServer()
        {
            pendingConnectionQueueSize = 100;
            delimiter = "";
            debugger = YucoDebugger.instance;
        }

        /// Initializes the server with an endpoint.
        public TcpServer(IPEndPoint endpoint)
        {
            this.endPoint = endpoint;
            pendingConnectionQueueSize = 100;
            delimiter = "";
            debugger = YucoDebugger.instance;
        }

        /// Initializes the server with a port, the endpoint is initialized with IPAddress.Any.
        public TcpServer(int port)
        {
            endPoint = new IPEndPoint(IPAddress.Any, port);
            pendingConnectionQueueSize = 100;
            delimiter = "";
            debugger = YucoDebugger.instance;
        }

        /// Initializes the server with a 4 digit IP address and port.
        public TcpServer(string address, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            pendingConnectionQueueSize = 100;
            debugger = YucoDebugger.instance;
        }

        // === TCP functions =================================================================================================================================================

        /// Begins listening for incoming connections. This method returns immediately. Incoming connections are reported using the Connected event.
        public void StartListening()
        {
            if (endPoint == null)
            {
                YucoDebugger.instance.LogError("EndPoint not initialized.", "StartListening", "TCPServer");
                throw new TcpLibException("EndPoint not initialized.");
            }

            if (listener != null)
            {
                YucoDebugger.instance.LogError("Already listening.", "StartListening", "TCPServer");
                throw new TcpLibException("Already listening.");
            }

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(pendingConnectionQueueSize);
            listener.BeginAccept(AcceptConnection, null);


            YucoDebugger.instance.Log("TCP server bound to " + endPoint.ToString() + ", startListening", "StartListening", "TCPServer");
        }

        /// Shuts down the listener.
        public void StopListening()
        {
            // Make sure we're not accepting a connection.
            lock (this)
            {
                listener.Close();
                listener = null;
            }
        }

        // Kill all client connections
        public void KillAllConnections()
        {
            foreach (ConnectionState cs in csList)
            {
                if (cs != null)
                {
                    try
                    {
                        cs.Close();
                    }
                    catch (Exception ex)
                    {
                        YucoDebugger.instance.LogError("TCPServer.KillAllConnections ::::: Exception: " + ex.ToString());
                    }
                }
            }
            csList.Clear();
            YucoDebugger.instance.Log("TCPServer.KillAllConnections ::::: Server killed all client connections.");
        }

        /// Restart the server.
        public void Restart(string _ip, int _port)
        {
            YucoDebugger.instance.LogWarning("Server Restarting to " + _ip + ":" + _port + "......", "Restart", "TCPServer");
            StopListening();

            // Close socket connection
            KillAllConnections();

            // Restart with new EndPoint
            endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            pendingConnectionQueueSize = 100;
            StartListening();

            YucoDebugger.instance.Log("Server Restarted. (" + _ip + ":" + _port + ")", "Restart", "TCPServer");
        }

        /// Accepts the connection and invokes any Connected event handlers.
        protected void AcceptConnection(IAsyncResult res)
        {
            Socket connection;

            // Make sure listener doesn't go null on us.
            lock (this)
            {
                connection = listener.EndAccept(res);
                listener.BeginAccept(AcceptConnection, null);
                YucoDebugger.instance.LogDebug("AcceptConnection: success", "AcceptConnection", "TCPServer");
            }

            // Close the connection if there are no handlers to accept it
            if (Connected == null)
            {
                connection.Close();
                YucoDebugger.instance.LogError("AcceptConnection: Connected == null --> connection.Close()", "AcceptConnection", "TCPServer");
            }
            else
            {
                ConnectionState cs = new ConnectionState(connection, this);
                // Fire Connected event
                OnConnected(new TcpServerEventArgs(cs));
            }
        }

        /// Receive string data from connection and invokes any Received event handlers.
        protected void ReceiveCallback(IAsyncResult res)
        {
            ConnectionState cs = (ConnectionState)res.AsyncState;
            try
            {
                int bytesRead = 0;
                lock (this)
                {
                    bytesRead = cs.Connection.EndReceive(res);
                }

                if (bytesRead != 0)
                {
                    //YucoDebugger.instance.LogDebug("bytesRead = " + bytesRead.ToString(), "ReceiveCallback", "TCPServer");
                    // Assign no. of byte read
                    cs.BytesRead = bytesRead;
                    // Get string from data
                    cs.DataString = Encoding.UTF8.GetString(cs.Buffer, 0, bytesRead);
                    // Fire Connected event
                    OnDataReceived(new TcpServerEventArgs(cs));
                    // Begin receiving data
                    lock (this)
                    {
                        cs.Connection.BeginReceive(cs.Buffer, 0, cs.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), cs);
                    }
                }
                else
                {
                    YucoDebugger.instance.LogDebug("bytesRead = 0", "ReceiveCallback", "TCPServer");
                    cs.Close();
                    OnDisconnected(new TcpServerEventArgs(cs));
                }
            }
            catch (Exception ex)
            {
                // Close the connection if the application threw an exception that is caught here by the server.
                cs.Close();
                csList.Remove(cs);
                TcpLibApplicationExceptionEventArgs appErr = new TcpLibApplicationExceptionEventArgs(ex);
                YucoDebugger.instance.LogError("ReceiveCallback: cs.Close()");

                try
                {
                    OnHandleApplicationException(appErr);
                    YucoDebugger.instance.LogWarning("ReceiveCallback: OnHandleApplicationException(appErr)", "ReceiveCallback", "TCPServer");
                }
                catch (Exception ex2)
                {
                    // the exception handler threw an exception...
                    YucoDebugger.instance.LogError("ReceiveCallback: ex2" + ex2, "ReceiveCallback", "TCPServer");
                }
            }
        }

        public bool hasClientConnecting()
        {
            return csList.Count > 0;
        }

        public void SetDelimiter(String delim)
        {
            delimiter = delim;
        }

        // === TCP events firing =================================================================================================================================================

        /// Fire the Connected event if it exists.
        protected virtual void OnConnected(TcpServerEventArgs e)
        {
            if (Connected != null)
            {
                try
                {
                    Connected(this, e);
                    // Start Receiving message
                    e.ConnectionState.Connection.BeginReceive(e.ConnectionState.Buffer, 0, e.ConnectionState.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), e.ConnectionState);
                    // Push to array
                    csList.Add(e.ConnectionState);
                }
                catch (Exception ex)
                {
                    // Close the connection if the application threw an exception that is caught here by the server.
                    e.ConnectionState.Close();
                    TcpLibApplicationExceptionEventArgs appErr = new TcpLibApplicationExceptionEventArgs(ex);
                    YucoDebugger.instance.LogError("OnDataReceived: Connected(this, e) encountered exception", "OnConnected", "TCPServer");

                    try
                    {
                        OnHandleApplicationException(appErr);
                        YucoDebugger.instance.LogWarning("OnConnected: OnHandleApplicationException(appErr)", "OnConnected", "TCPServer");
                    }
                    catch (Exception ex2)
                    {
                        // the exception handler threw an exception...
                        YucoDebugger.instance.LogError("OnConnected: ex2" + ex2, "OnConnected", "TCPServer");
                    }
                }
            }
        }

        /// Fire the DataReceived event if it exists.
        protected virtual void OnDataReceived(TcpServerEventArgs e)
        {
            if (DataReceived != null)
            {
                try
                {
                    DataReceived(this, e);
                }
                catch (Exception ex)
                {
                    // Close the connection if the application threw an exception that is caught here by the server.
                    e.ConnectionState.Close();
                    csList.Remove(e.ConnectionState);
                    YucoDebugger.instance.LogError("OnDataReceived: DataReceived(this, e) encountered exception", "OnDataReceived", "TCPServer");
                    TcpLibApplicationExceptionEventArgs appErr = new TcpLibApplicationExceptionEventArgs(ex);

                    try
                    {
                        OnHandleApplicationException(appErr);
                        YucoDebugger.instance.Log("OnDataReceived: OnHandleApplicationException(appErr)", "OnDataReceived", "TCPServer");
                    }
                    catch (Exception ex2)
                    {
                        // the exception handler threw an exception...
                        YucoDebugger.instance.LogError("OnDataReceived: ex2" + ex2, "OnDataReceived", "TCPServer");
                    }
                }
            }
        }

        /// Fire the Disonncted event if it exists.
        protected virtual void OnDisconnected(TcpServerEventArgs e)
        {
            if (Disconnected != null)
            {
                try
                {
                    Disconnected(this, e);
                    csList.Remove(e.ConnectionState);
                }
                catch (Exception ex)
                {
                    YucoDebugger.instance.LogError("OnDisconnected: ex" + ex, "OnDisconnected", "TCPServer");
                }
            }
        }

        // === TCP send message handlers =================================================================================================================================================

        // Send string data to client.
        public void SendDataString(string dataStr)
        {
            foreach (ConnectionState cs in csList)
            {
                BeginSend(cs, dataStr + delimiter);
            }
        }

        // Begin async. socket send operation.
        private void BeginSend(ConnectionState cs, string dataStr)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(dataStr); // gives you data for the buffer

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.SetBuffer(buffer, 0, buffer.Length);
            e.Completed += new EventHandler<SocketAsyncEventArgs>(SendCallback);

            bool completedAsync = false;

            try
            {
                completedAsync = cs.Connection.SendAsync(e);
            }
            catch (SocketException se)
            {
                YucoDebugger.instance.Log("Socket Exception: " + se.ErrorCode + " Message: " + se.Message, "BeginSend", "TCPServer");
            }

            if (!completedAsync)
            {
                // The call completed synchronously so invoke the callback ourselves
                SendCallback(this, e);
            }
        }

        // Send result callback.
        private void SendCallback(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //YucoDebugger.instance.Log("Socket data sent successfully.");
            }
            else
            {
                YucoDebugger.instance.LogError("Socket Error: " + e.SocketError + " when sending to client.", "SendCallback", "TCPServer");
            }
        }

        // === TCP special exception handlers =================================================================================================================================================

        /// Invokes the HandleApplicationException handler, if exists.
        protected virtual void OnHandleApplicationException(TcpLibApplicationExceptionEventArgs e)
        {
            if (HandleApplicationException != null)
            {
                YucoDebugger.instance.LogError("OnHandleApplicationException: " + e, "OnHandleApplicationException", "TCPServer");
                HandleApplicationException(this, e);
            }
        }
    }
}