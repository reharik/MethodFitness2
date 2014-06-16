namespace CC.DataValidation
{
    public class ErrorInfo
    {
        public ErrorInfo(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        public object Instance { get; set; }
        public string ObjectType { get; set; }

    }
}