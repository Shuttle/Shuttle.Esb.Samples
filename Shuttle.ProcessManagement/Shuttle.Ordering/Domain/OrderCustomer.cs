namespace Shuttle.Ordering.Domain
{
    public class OrderCustomer
    {
        public string Name { get; private set; }
        public string EMail { get; private set; }

        public OrderCustomer(string name, string eMail)
        {
            Name = name;
            EMail = eMail;
        }
    }
}