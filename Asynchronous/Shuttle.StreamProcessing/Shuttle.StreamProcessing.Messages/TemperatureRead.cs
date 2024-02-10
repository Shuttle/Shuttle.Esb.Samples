namespace Shuttle.StreamProcessing.Messages
{
    public class TemperatureRead
    {
        public string Name { get; set; }
        public int Minute { get; set; }
        public decimal Celsius { get; set; }
    }
}