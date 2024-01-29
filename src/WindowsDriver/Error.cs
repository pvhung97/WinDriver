namespace WindowsDriver
{
    public class Error
    {
        public string error { get; set; }
        public string message { get; set; }
        public string stacktrace { get; set; }
        public object? data { get; set; }

        public Error(string error, string message) : this(error, message, "") { }

        public Error(string error, string message, string stacktrace) : this(error, message, stacktrace, null) { }

        public Error(string error, string message, string stacktrace, object? data)
        {
            this.error = error;
            this.message = message;
            this.stacktrace = stacktrace;
            this.data = data;
        }
    }
}
