using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;

namespace JavaDetect
{
    /// <summary>
    /// Functions for java path detection
    /// </summary>
    internal static class JavaPath
    {
        /// <summary>
        /// Fetches the bin folder of java (the folder where the installed java.exe is).
        /// </summary>
        /// <returns>Java binary folder.</returns>
        /// <exception cref="JavaNotFoundException" />
        public static string GetJavaBinaryPath()
        {
            object cver = GetJavaRegistry().OpenSubKey(GetJavaVersion());
            if (cver == null)
                throw new JavaNotFoundException();

            cver = Path.Combine(GetJavaHome(), "bin");
            if (!Directory.Exists(cver.ToString()))
                throw new JavaNotFoundException();
            else
                return cver.ToString();
        }

        /// <summary>
        /// Fetches the java home directory.
        /// </summary>
        /// <returns>Java home path.</returns>
        /// <exception cref="JavaNotFoundException" />
        public static string GetJavaHome()
        {
            // Search for overriding java path
            object cver = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (cver != null && !string.IsNullOrEmpty(cver.ToString()) && File.Exists(Path.Combine(cver.ToString(), "bin", "java.exe")))
                return cver.ToString();

            // Path expansion (idea by MiningMarsh)
            string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
            foreach (string path in paths)
            {
                if (File.Exists(Path.Combine(path, "..", "bin", "java.exe"))) // we need the java home folder to be valid
                    return Path.Combine(path, "..");
            }

            // Registry (only on windows)
            cver = GetJavaRegistry().OpenSubKey(GetJavaVersion());
            if (cver == null)
                throw new JavaNotFoundException();

            cver = ((RegistryKey)cver).GetValue("JavaHome", null);
            if (cver != null)
                return cver.ToString();

            // No possibilites left.
            throw new JavaNotFoundException();
        }

        /// <summary>
        /// Fetches the java registry node in following order:
        /// 
        /// 64-bit JDK, 64-bit JRE, 32-bit JDK, 32-bit JRE
        /// </summary>
        /// <returns>The registry node of the detected java installation.</returns>
        /// <exception cref="JavaNotFoundException" />
        public static RegistryKey GetJavaRegistry()
        {
            RegistryKey cver = null;

            // JDK
            cver = Registry.LocalMachine
                .OpenSubKey("Software")
                .OpenSubKey("JavaSoft");
            if (cver != null)
                cver = Registry.LocalMachine
                .OpenSubKey("Software")
                .OpenSubKey("JavaSoft")
                .OpenSubKey("Java Development Kit");

            // JRE
            if (cver == null)
                cver = Registry.LocalMachine
                .OpenSubKey("Software")
                .OpenSubKey("JavaSoft");
            if (cver != null)
                cver = Registry.LocalMachine
                .OpenSubKey("Software")
                .OpenSubKey("JavaSoft")
                .OpenSubKey("Java Runtime Environment");

            // 32-bit node on 64-bit systems
            if (Registry.LocalMachine
                .OpenSubKey("Software")
                .OpenSubKey("Wow6432Node") != null)
            {
                // JRE
                if (cver == null)
                    cver = Registry.LocalMachine
                    .OpenSubKey("Software")
                    .OpenSubKey("Wow6432Node")
                    .OpenSubKey("JavaSoft");
                if (cver != null)
                    cver = Registry.LocalMachine
                    .OpenSubKey("Software")
                    .OpenSubKey("Wow6432Node")
                    .OpenSubKey("JavaSoft").
                    OpenSubKey("Java Development Kit");

                // JDK
                if (cver == null)
                    cver = Registry.LocalMachine
                    .OpenSubKey("Software")
                    .OpenSubKey("Wow6432Node")
                    .OpenSubKey("JavaSoft");
                if (cver != null)
                    cver = Registry.LocalMachine
                    .OpenSubKey("Software")
                    .OpenSubKey("Wow6432Node")
                    .OpenSubKey("JavaSoft")
                    .OpenSubKey("Java Runtime Environment");
            }

            // Final stage
            if (cver == null)
                throw new JavaNotFoundException();
            else
                return cver;
        }

        /// <summary>
        /// Fetches the installed java version string.
        /// </summary>
        /// <returns>Java version string.</returns>
        /// <exception cref="JavaNotFoundException" />
        public static string GetJavaVersion()
        {
            object cver = GetJavaRegistry().GetValue("CurrentVersion", null);

            if (cver == null)
                throw new JavaNotFoundException();
            else
                return cver.ToString();
        }
    }

    /// <summary>
    /// Throw when Java is unavailable
    /// </summary>
    [System.Serializable]
    public class JavaNotFoundException : System.Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public JavaNotFoundException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public JavaNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Inner Exception</param>
        public JavaNotFoundException(string message, System.Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected JavaNotFoundException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}