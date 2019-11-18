// ---------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace MessageProcessor
{
    /// <summary>
    /// Message processor program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Start function
        /// </summary>
        /// <param name="args">commandline arguments</param>
        public static void Main(string[] args)
        {
            MessageProcessor.Run().Wait();
        }
    }
}
