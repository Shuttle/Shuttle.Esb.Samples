namespace Shuttle.Invoicing.Domain
{
    public class InvoiceAccountContact
    {
        public string Name { get; private set; }
        public string EMail { get; private set; }

        public InvoiceAccountContact(string name, string eMail)
        {
            Name = name;
            EMail = eMail;
        }
    }
}