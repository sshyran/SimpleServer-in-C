using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer
{
    public class VersionInfo
    {
        public static int VersionMajor { get { return 1; } }
        public static int VersionMajorRevision { get { return 0; } }
        public static int VersionMinor { get { return 0; } }
        public static int JenkinsBuild { get { return 0; } } // I haven't configured jenkins yet
        public static string BuildId { get { return "SSSC."+Version+"."+ JenkinsBuild + (!IsDevelopmentBuild ? "F" : "D"); } }
        public static string Version { get { return VersionMajor.ToString() + "." + VersionMajorRevision.ToString() + "." + VersionMinor.ToString(); } }
        public static bool IsDevelopmentBuild { get { return true; } }
    }
}
