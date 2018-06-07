/* Copyright (C) YU & CO. (LAB) LIMITED - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Eric Koo <eric.koo@yucolab.com>, May 2013
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Yucolab
{
    /// <summary>
    /// Exception class for this assembly.
    /// </summary>
    public class TcpLibException : ApplicationException
    {
        public TcpLibException(string msg)
            : base(msg)
        {
        }
    }
}
