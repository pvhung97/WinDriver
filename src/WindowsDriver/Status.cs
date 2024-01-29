namespace WindowsDriver
{
    public class Status
    {
        public bool Ready { get; set; }
        public string Message { get; set; }
        public string Version { get; set; }
        public OsInfo Os {  get; set; }

        public Status()
        {
            this.Ready = true;
            this.Message = "WindowsDriver ready for new sessions";
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Version = version != null ? version.ToString() : "0.0.0.1";
            this.Os = new OsInfo();
        }
    }

    public class OsInfo
    {
        public string Arch {  get; set; }
        public string Name { get; set; }

        public OsInfo()
        {
            this.Arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
            this.Name = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        }
    }
}
