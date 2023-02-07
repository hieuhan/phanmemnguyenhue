namespace phanmemnguyenhue.Services.ToastrNotifications
{
    public class Toastr
    {
        public ToastrTypes Severity { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string FunctionName { get; private set; }
        public Toastr(ToastrTypes severity, string title, string message)
        {
            Title = title;
            Message = message;
            Severity = severity;
        }
        public Toastr(string functionName, string scripts)
        {
            FunctionName = functionName;
            Message = scripts;
        }

        public Toastr(ToastrTypes severity, string message)
            : this(severity, null, message)
        { }
    }
}