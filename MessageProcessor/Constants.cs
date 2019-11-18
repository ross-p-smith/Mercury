// ---------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
// </copyright>
// ---------------------------------------------------------------------------

namespace MessageProcessor
{
    using System;

    /// <summary>
    /// Constants for MessageProcessor
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Sleep time between queue poll
        /// </summary>
        public const int SleepTime = 5000;

        /// <summary>
        /// Filename the processor will write to
        /// </summary>
        public const string NotifyFilename = "NotifyReceived.txt";
    }
}
