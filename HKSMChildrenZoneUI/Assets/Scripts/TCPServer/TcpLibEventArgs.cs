/* Copyright (C) YU & CO. (LAB) LIMITED - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Eric Koo <eric.koo@yucolab.com>, May 2013
 */

using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Implements a custom EventArgs class for passing connection state information.
/// </summary>

namespace Yucolab
{
    public class TcpServerEventArgs : EventArgs
    {
        protected ConnectionState connectionState;

        public ConnectionState ConnectionState
        {
            get { return connectionState; }
        }

        public TcpServerEventArgs(ConnectionState cs)
        {
            connectionState = cs;
        }
    }

    /// <summary>
    /// Implements a custom EventArgs class for passing an application exception back
    /// to the application for processing.
    /// </summary>
    public class TcpLibApplicationExceptionEventArgs : EventArgs
    {
        protected Exception e;

        public Exception Exception
        {
            get { return e; }
        }

        public TcpLibApplicationExceptionEventArgs(Exception e)
        {
            this.e = e;
        }
    }
}