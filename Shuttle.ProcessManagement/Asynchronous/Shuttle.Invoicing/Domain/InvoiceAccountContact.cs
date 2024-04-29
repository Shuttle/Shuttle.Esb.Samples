namespace Shuttle.Invoicing.Domain
{
    public class InvoiceAccountContact
    {
        public InvoiceAccountContact(string name, string eMail)
        {
            Name = name;
            EMail = eMail;
        }

        public string Name { get; }
        public string EMail { get; }
    }
}