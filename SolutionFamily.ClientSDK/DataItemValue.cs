namespace SolutionFamily
{
    public class DataItemValue
    {
        internal DataItemValue(string timestamp, string value)
        {
            TimeStamp = timestamp;
            Value = value;
        }

        public string Value { get; set; }
        public string TimeStamp { get; set; }

    }
}
