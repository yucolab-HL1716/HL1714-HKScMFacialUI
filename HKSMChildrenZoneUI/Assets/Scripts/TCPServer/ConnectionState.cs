/* Copyright (C) YU & CO. (LAB) LIMITED - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Eric Koo <eric.koo@yucolab.com>, May 2013
 */

using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;

namespace Yucolab
{
    /// <summary>
    /// Buffers the socket connection and TcpServer instance.
    /// </summary>
    public class ConnectionState
    {
        protected Socket connection;
        protected TcpServer server;
        protected byte[] buffer;
        protected string dataString;
        protected int bytesRead;
        protected bool isClosed;

        // Debugger singleton
        //private Debugger debugger;


        public ConnectionState(Socket connection, TcpServer server)
        {
            this.connection = connection;
            this.server = server;
            buffer = new byte[255];
            bytesRead = 0;
            isClosed = false;
        }


        // === Get/Set functions ========================================================================================================================================

        /// Gets the TcpServer instance. Throws an exception if the connection has been closed.
        public TcpServer Server
        {
            get
            {
                if (server == null)
                {
                    throw new TcpLibException("Connection is closed.");
                }

                return server;
            }
        }

        /// Gets the socket connection. Throws an exception if the connection has been closed.
        public Socket Connection
        {
            get
            {
                if (server == null)
                {
                    throw new TcpLibException("Connection is closed.");
                }

                return connection;
            }
        }

        /// Gets the buffer.
        public byte[] Buffer
        {
            get { return buffer; }
        }

        /// Gets/sets the DataString.
        public string DataString
        {
            get { return dataString; }
            set { dataString = value; }
        }

        /// Gets/sets the DataString.
        public int BytesRead
        {
            get { return bytesRead; }
            set { bytesRead = value; }
        }

        /// Gets isClosed.
        public bool IsClosed
        {
            get { return isClosed; }
        }

        // === TCP functions ========================================================================================================================================

        /// This is the prefered manner for closing a socket connection, as it nulls the internal fields so that subsequently referencing a closed
        /// connection throws an exception. This method also throws an exception if the connection has already been shut down.
        public void Close()
        {
            if (server == null)
            {
                throw new TcpLibException("Connection already is closed.");
            }

            connection.Shutdown(SocketShutdown.Both);
            connection.Close();
            connection = null;
            server = null;

            isClosed = true;
        }
    }
}