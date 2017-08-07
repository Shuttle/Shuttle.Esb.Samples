namespace Shuttle.Ordering.Domain
{
    public class OrderCustomer
    {
        public OrderCustomer(string name, string eMail)
        {
            Name = name;
            EMail = eMail;
        }

        public string Name { get; }
        public string EMail { get; }
    }
}