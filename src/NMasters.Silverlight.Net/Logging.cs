using System;
using System.Diagnostics;
using NMasters.Silverlight.Net.Http;

namespace NMasters.Silverlight.Net
{
    using System.Threading;
    using System.Security;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;    
    using System.Linq;
    using System.Collections.ObjectModel;

    internal class Logging
    {
        private static volatile bool s_LoggingEnabled = true;
        private static volatile bool s_LoggingInitialized;
        private static volatile bool s_AppDomainShutdown;

        private const int DefaultMaxDumpSize = 1024;
        private const bool DefaultUseProtocolTextOnly = false;

        private const string AttributeNameMaxSize = "maxdatasize";
        private const string AttributeNameTraceMode = "tracemode";
        private static readonly string[] SupportedAttributes = new string[] { AttributeNameMaxSize, AttributeNameTraceMode };

        private const string AttributeValueProtocolOnly = "protocolonly";
        //private const string AttributeValueIncludeHex = "includehex";

        private const string TraceSourceWebName = "System.Net";
        private const string TraceSourceHttpListenerName = "System.Net.HttpListener";
        private const string TraceSourceSocketsName = "System.Net.Sockets";
        private const string TraceSourceWebSocketsName = "System.Net.WebSockets";
        private const string TraceSourceCacheName = "System.Net.Cache";
        private const string TraceSourceHttpName = "System.Net.Http";

        private static TraceSource s_WebTraceSource;
        private static TraceSource s_HttpListenerTraceSource;
        private static TraceSource s_SocketsTraceSource;
        private static TraceSource s_WebSocketsTraceSource;
        private static TraceSource s_CacheTraceSource;
        private static TraceSource s_TraceSourceHttpName;

        private Logging()
        {
        }

        private static object s_InternalSyncObject;

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object o = new Object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, o, null);
                }
                return s_InternalSyncObject;
            }
        }

        internal static bool On
        {
            get
            {
                if (!s_LoggingInitialized)
                {
                    InitializeLogging();
                }
                return s_LoggingEnabled;
            }
        }

        internal static bool IsVerbose(TraceSource traceSource)
        {
            return ValidateSettings(traceSource, TraceEventType.Verbose);
        }

        internal static TraceSource Web
        {
            get
            {
                if (!s_LoggingInitialized)
                {
                    InitializeLogging();
                }
                if (!s_LoggingEnabled)
                {
                    return null;
                }
                return s_WebTraceSource;
            }
        }

        internal static TraceSource Http
        {
            get
            {
                if (!s_LoggingInitialized)
                {
                    InitializeLogging();
                }
                if (!s_LoggingEnabled)
                {
                    return null;
                }
                return s_TraceSourceHttpName;
            }
        }

        internal static TraceSource HttpListener
        {
            get
            {
                if (!s_LoggingInitialized)
                {
                    InitializeLogging();
                }
                if (!s_LoggingEnabled)
                {
                    return null;
                }
                return s_HttpListenerTraceSource;
            }
        }

        internal static TraceSource Sockets
        {

            get
            {
                if (!s_LoggingInitialized)
                {
                    InitializeLogging();
                }
                if (!s_LoggingEnabled)
                {
                    return null;
                }
                return s_SocketsTraceSource;
            }
        }

        internal static TraceSource RequestCache
        {
            get
            {
                if (!s_LoggingInitialized)
                {
                    InitializeLogging();
                }
                if (!s_LoggingEnabled)
                {
                    return null;
                }
                return s_CacheTraceSource;
            }
        }

        internal static TraceSource WebSockets
        {
            get
            {
                if (!s_LoggingInitialized)
                {
                    InitializeLogging();
                }
                if (!s_LoggingEnabled)
                {
                    return null;
                }
                return s_WebSocketsTraceSource;
            }
        }
       
        /// <devdoc>
        ///    <para>Sets up internal config settings for logging. (MUST be called under critsec) </para>
        /// </devdoc>
        private static void InitializeLogging()
        {
            lock (InternalSyncObject)
            {
                if (!s_LoggingInitialized)
                {
                    bool loggingEnabled = false;
                    s_WebTraceSource = new NclTraceSource(TraceSourceWebName);
                    s_HttpListenerTraceSource = new NclTraceSource(TraceSourceHttpListenerName);
                    s_SocketsTraceSource = new NclTraceSource(TraceSourceSocketsName);
                    s_WebSocketsTraceSource = new NclTraceSource(TraceSourceWebSocketsName);
                    s_CacheTraceSource = new NclTraceSource(TraceSourceCacheName);
                    s_TraceSourceHttpName = new NclTraceSource(TraceSourceHttpName);

                    try
                    {
                        loggingEnabled = true;
                    }
                    catch (SecurityException)
                    {
                        // These may throw if the caller does not have permission to hook up trace listeners.
                        // We treat this case as though logging were disabled.
                        Close();
                        loggingEnabled = false;
                    }
                    if (loggingEnabled)
                    {
                        AppDomain currentDomain = AppDomain.CurrentDomain;                        
                        // SL fixes
                        //currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
                        //currentDomain.DomainUnload += new EventHandler(AppDomainUnloadEvent);
                        //currentDomain.ProcessExit += new EventHandler(ProcessExitEvent);
                    }
                    s_LoggingEnabled = loggingEnabled;
                    s_LoggingInitialized = true;
                }
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Logging functions must work in partial trust mode")]
        private static void Close()
        {
            //if (s_WebTraceSource != null) s_WebTraceSource.Close();
            //if (s_HttpListenerTraceSource != null) s_HttpListenerTraceSource.Close();
            //if (s_SocketsTraceSource != null) s_SocketsTraceSource.Close();
            //if (s_WebSocketsTraceSource != null) s_WebSocketsTraceSource.Close();
            //if (s_CacheTraceSource != null) s_CacheTraceSource.Close();
            //if (s_TraceSourceHttpName != null) s_TraceSourceHttpName.Close();
        }

        /// <devdoc>
        ///    <para>Logs any unhandled exception through this event handler</para>
        /// </devdoc>
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Exception(Web, sender, "UnhandledExceptionHandler", e);
        }

        private static void ProcessExitEvent(object sender, EventArgs e)
        {
            Close();
            s_AppDomainShutdown = true;
        }
        /// <devdoc>
        ///    <para>Called when the system is shutting down, used to prevent additional logging post-shutdown</para>
        /// </devdoc>
        private static void AppDomainUnloadEvent(object sender, EventArgs e)
        {
            Close();
            s_AppDomainShutdown = true;
        }


        /// <devdoc>
        ///    <para>Confirms logging is enabled, given current logging settings</para>
        /// </devdoc>
        private static bool ValidateSettings(TraceSource traceSource, TraceEventType traceLevel)
        {
            if (!s_LoggingEnabled)
            {
                return false;
            }
            if (!s_LoggingInitialized)
            {
                InitializeLogging();
            }
            if (traceSource == null )//|| !traceSource.Switch.ShouldTrace(traceLevel))
            {
                return false;
            }
            if (s_AppDomainShutdown)
            {
                return false;
            }
            return true;
        }

        /// <devdoc>
        ///    <para>Converts an object to a normalized string that can be printed
        ///         takes System.Net.ObjectNamedFoo and coverts to ObjectNamedFoo, 
        ///         except IPAddress, IPEndPoint, and Uri, which return ToString()
        ///         </para>
        /// </devdoc>
        private static string GetObjectName(object obj)
        {
            if (obj is Uri || obj is System.Net.IPAddress || obj is System.Net.IPEndPoint)
            {
                return obj.ToString();
            }
            else
            {
                return obj.GetType().Name;
            }
        }

        internal static uint GetThreadId()
        {            
            return (uint)Thread.CurrentThread.GetHashCode();         
        }

        internal static void PrintLine(TraceSource traceSource, TraceEventType eventType, int id, string msg)
        {
            string logHeader = "[" + GetThreadId().ToString("d4", CultureInfo.InvariantCulture) + "] ";
            // SL fixes
            Debug.WriteLine(logHeader + msg);
            //traceSource.TraceEvent(eventType, id, logHeader + msg);
            
        }

        /// <devdoc>
        ///    <para>Indicates that two objects are getting used with one another</para>
        /// </devdoc>

        internal static void Associate(TraceSource traceSource, object objA, object objB)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }

            string lineA = GetObjectName(objA) + "#" + ValidationHelper.HashString(objA);
            string lineB = GetObjectName(objB) + "#" + ValidationHelper.HashString(objB);

            PrintLine(traceSource, TraceEventType.Information, 0, "Associating " + lineA + " with " + lineB);
        }

        /// <devdoc>
        ///    <para>Logs entrance to a function</para>
        /// </devdoc>

        internal static void Enter(TraceSource traceSource, object obj, string method, string param)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            Enter(traceSource, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj), method, param);
        }

        /// <devdoc>
        ///    <para>Logs entrance to a function</para>
        /// </devdoc>

        internal static void Enter(TraceSource traceSource, object obj, string method, object paramObject)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            Enter(traceSource, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj), method, paramObject);
        }

        /// <devdoc>
        ///    <para>Logs entrance to a function</para>
        /// </devdoc>
        internal static void Enter(TraceSource traceSource, string obj, string method, string param)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            Enter(traceSource, obj + "::" + method + "(" + param + ")");
        }

        /// <devdoc>
        ///    <para>Logs entrance to a function</para>
        /// </devdoc>
        internal static void Enter(TraceSource traceSource, string obj, string method, object paramObject)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            string paramObjectValue = "";
            if (paramObject != null)
            {
                paramObjectValue = GetObjectName(paramObject) + "#" + ValidationHelper.HashString(paramObject);
            }
            Enter(traceSource, obj + "::" + method + "(" + paramObjectValue + ")");
        }

        /// <devdoc>
        ///    <para>Logs entrance to a function, indents and points that out</para>
        /// </devdoc>
        internal static void Enter(TraceSource traceSource, string method, string parameters)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            Enter(traceSource, method + "(" + parameters + ")");
        }

        /// <devdoc>
        ///    <para>Logs entrance to a function, indents and points that out</para>
        /// </devdoc>
        internal static void Enter(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            // Trace.CorrelationManager.StartLogicalOperation();
            PrintLine(traceSource, TraceEventType.Verbose, 0, msg);
        }

        /// <devdoc>
        ///    <para>Logs exit from a function</para>
        /// </devdoc>

        internal static void Exit(TraceSource traceSource, object obj, string method, object retObject)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            string retValue = "";
            if (retObject != null)
            {
                retValue = GetObjectName(retObject) + "#" + ValidationHelper.HashString(retObject);
            }
            Exit(traceSource, obj, method, retValue);
        }

        /// <devdoc>
        ///    <para>Logs exit from a function</para>
        /// </devdoc>
        internal static void Exit(TraceSource traceSource, string obj, string method, object retObject)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            string retValue = "";
            if (retObject != null)
            {
                retValue = GetObjectName(retObject) + "#" + ValidationHelper.HashString(retObject);
            }
            Exit(traceSource, obj, method, retValue);
        }


        /// <devdoc>
        ///    <para>Logs exit from a function</para>
        /// </devdoc>

        internal static void Exit(TraceSource traceSource, object obj, string method, string retValue)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            Exit(traceSource, GetObjectName(obj) + "#" + ValidationHelper.HashString(obj), method, retValue);
        }

        /// <devdoc>
        ///    <para>Logs exit from a function</para>
        /// </devdoc>
        internal static void Exit(TraceSource traceSource, string obj, string method, string retValue)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            if (!ValidationHelper.IsBlankString(retValue))
            {
                retValue = "\t-> " + retValue;
            }
            Exit(traceSource, obj + "::" + method + "() " + retValue);
        }

        /// <devdoc>
        ///    <para>Logs exit from a function</para>
        /// </devdoc>
        internal static void Exit(TraceSource traceSource, string method, string parameters)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            Exit(traceSource, method + "() " + parameters);
        }

        /// <devdoc>
        ///    <para>Logs exit from a function</para>
        /// </devdoc>
        internal static void Exit(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            PrintLine(traceSource, TraceEventType.Verbose, 0, "Exiting " + msg);
            // Trace.CorrelationManager.StopLogicalOperation();
        }

        /// <devdoc>
        ///    <para>Logs Exception, restores indenting</para>
        /// </devdoc>

        internal static void Exception(TraceSource traceSource, object obj, string method, Exception e)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Error))
            {
                return;
            }

            StringBuilder exceptionMessageBuilder = new StringBuilder(e.Message);
            if (e is AggregateException)
            {
                AggregateException aggregateException = e as AggregateException;
                ReadOnlyCollection<Exception> innerExceptions = aggregateException.Flatten().InnerExceptions;
                if (innerExceptions.Count > 0)
                {
                    string innerExceptionMessages = string.Join(", ", innerExceptions.Select(innerException => innerException.Message));
                    exceptionMessageBuilder.AppendFormat(CultureInfo.InvariantCulture, " InnerExceptions: {0}", innerExceptionMessages);
                }
            }

            string infoLine = string.Format(CultureInfo.InvariantCulture, "Exception in {0}::{1} - {2}.", GetObjectLogHash(obj), method, exceptionMessageBuilder);

            if (!ValidationHelper.IsBlankString(e.StackTrace))
            {
                infoLine += "\r\n" + e.StackTrace;
            }
            PrintLine(traceSource, TraceEventType.Error, 0, infoLine);
        }

        /// <devdoc>
        ///    <para>Logs an Info line</para>
        /// </devdoc>
        internal static void PrintInfo(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            PrintLine(traceSource, TraceEventType.Information, 0, msg);
        }

        /// <devdoc>
        ///    <para>Logs an Info line</para>
        /// </devdoc>

        internal static void PrintInfo(TraceSource traceSource, object obj, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            PrintLine(traceSource, TraceEventType.Information, 0,
                                   GetObjectName(obj) + "#" + ValidationHelper.HashString(obj)
                                   + " - " + msg);
        }

        /// <devdoc>
        ///    <para>Logs an Info line</para>
        /// </devdoc>
        internal static void PrintInfo(TraceSource traceSource, object obj, string method, string param)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Information))
            {
                return;
            }
            PrintLine(traceSource, TraceEventType.Information, 0,
                                   GetObjectName(obj) + "#" + ValidationHelper.HashString(obj)
                                   + "::" + method + "(" + param + ")");
        }

        /// <devdoc>
        ///    <para>Logs a Warning line</para>
        /// </devdoc>

        internal static void PrintWarning(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Warning))
            {
                return;
            }
            PrintLine(traceSource, TraceEventType.Warning, 0, msg);
        }

        /// <devdoc>
        ///    <para>Logs a Warning line</para>
        /// </devdoc>
        internal static void PrintWarning(TraceSource traceSource, object obj, string method, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Warning))
            {
                return;
            }
            PrintLine(traceSource, TraceEventType.Warning, 0,
                                   GetObjectName(obj) + "#" + ValidationHelper.HashString(obj)
                                   + "::" + method + "() - " + msg);
        }

        /// <devdoc>
        ///    <para>Logs an Error line</para>
        /// </devdoc>

        internal static void PrintError(TraceSource traceSource, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Error))
            {
                return;
            }
            PrintLine(traceSource, TraceEventType.Error, 0, msg);
        }

        /// <devdoc>
        ///    <para>Logs an Error line</para>
        /// </devdoc>

        internal static void PrintError(TraceSource traceSource, object obj, string method, string msg)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Error))
            {
                return;
            }
            PrintLine(traceSource, TraceEventType.Error, 0,
                                   GetObjectName(obj) + "#" + ValidationHelper.HashString(obj)
                                   + "::" + method + "() - " + msg);
        }

        internal static string GetObjectLogHash(object obj)
        {
            return GetObjectName(obj) + "#" + ValidationHelper.HashString(obj);
        }

        /// <devdoc>
        ///    <para>Marhsalls a buffer ptr to an array and then dumps the byte array to the log</para>
        /// </devdoc>
        internal static void Dump(TraceSource traceSource, object obj, string method, IntPtr bufferPtr, int length)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Verbose) || bufferPtr == IntPtr.Zero || length < 0)
            {
                return;
            }
            byte[] buffer = new byte[length];
            Marshal.Copy(bufferPtr, buffer, 0, length);
            Dump(traceSource, obj, method, buffer, 0, length);
        }

        // Latin-1 encoding (ISO-88591-1)
        private static Encoding headerEncoding = Encoding.UTF8;//"28591");

        /// <devdoc>
        ///    <para>Dumps a byte array to the log</para>
        /// </devdoc>
        internal static void Dump(TraceSource traceSource, object obj, string method, byte[] buffer, int offset, int length)
        {
            if (!ValidateSettings(traceSource, TraceEventType.Verbose))
            {
                return;
            }
            if (buffer == null)
            {
                PrintLine(traceSource, TraceEventType.Verbose, 0, "(null)");
                return;
            }
            if (offset > buffer.Length)
            {
                PrintLine(traceSource, TraceEventType.Verbose, 0, "(offset out of range)");
                return;
            }
            PrintLine(traceSource, TraceEventType.Verbose, 0, "Data from " + GetObjectName(obj) + "#" + ValidationHelper.HashString(obj) + "::" + method);
            int maxDumpSize = DefaultMaxDumpSize;
            if (length > maxDumpSize)
            {
                PrintLine(traceSource, TraceEventType.Verbose, 0, "(printing " + maxDumpSize.ToString(NumberFormatInfo.InvariantInfo) + " out of " + length.ToString(NumberFormatInfo.InvariantInfo) + ")");
                length = maxDumpSize;
            }
            if ((length < 0) || (length > buffer.Length - offset))
            {
                length = buffer.Length - offset;
            }
            // SL fixes
            if (true)
            {
                string output = "<<" + headerEncoding.GetString(buffer, offset, length) + ">>";
                PrintLine(traceSource, TraceEventType.Verbose, 0, output);
                return;
            }
            //do
            //{
            //    int n = Math.Min(length, 16);
            //    string disp = String.Format(CultureInfo.CurrentCulture, "{0:X8} : ", offset);
            //    for (int i = 0; i < n; ++i)
            //    {
            //        disp += String.Format(CultureInfo.CurrentCulture, "{0:X2}", buffer[offset + i]) + ((i == 7) ? '-' : ' ');
            //    }
            //    for (int i = n; i < 16; ++i)
            //    {
            //        disp += "   ";
            //    }
            //    disp += ": ";
            //    for (int i = 0; i < n; ++i)
            //    {
            //        disp += ((buffer[offset + i] < 0x20) || (buffer[offset + i] > 0x7e))
            //                    ? '.'
            //                    : (char)(buffer[offset + i]);
            //    }
            //    PrintLine(traceSource, TraceEventType.Verbose, 0, disp);
            //    offset += n;
            //    length -= n;
            //} while (length > 0);
        }

        internal class TraceSource
        {
            public TraceSource(string name)
            {
                Name = name;
            }

            public string Name { get; set; }

            protected virtual string[] GetSupportedAttributes() { return new string[0];}
        }

        internal class NclTraceSource : TraceSource
        {
            internal NclTraceSource(string name) : base(name) { }

            protected override string[] GetSupportedAttributes()
            {
                return Logging.SupportedAttributes;
            }
        }

        internal enum TraceEventType
        {            
            Verbose = 0,
            Critical = 1,
            Error = 2,
            Warning = 3,
            Debug = 4,
            Information = 5
        }
    }
}

