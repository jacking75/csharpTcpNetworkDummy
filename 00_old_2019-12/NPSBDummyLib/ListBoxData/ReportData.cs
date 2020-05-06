namespace NPSBDummyLib
{
    public class ReportData
    {
        public string Message { get; private set; }
        public string Detail { get; private set; }

        public ReportData(string message, string detail = null)
        {
            Message = message;
            Detail = detail;
        }

        public ReportData()
        {
        }
    }
}
