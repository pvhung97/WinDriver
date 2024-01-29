using System.Diagnostics;

namespace WindowsDriver
{
    public class SessionInfo
    {
        public string url;
        public Process p;
        public int commandTimeout;

        public SessionInfo(string url, Process p, int commandTimeout)
        {
            this.url = url;
            this.p = p;
            this.commandTimeout = commandTimeout;
        }
    }
}
