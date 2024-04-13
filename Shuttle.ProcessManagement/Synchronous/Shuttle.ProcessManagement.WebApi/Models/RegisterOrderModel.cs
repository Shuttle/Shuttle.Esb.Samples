namespace Shuttle.ProcessManagement.WebApi
{
    public class RegisterOrderModel
    {
        public List<string>? ProductIds { get; set; }
        public string? TargetSystem { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEMail { get; set; }
    }
}