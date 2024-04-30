using System;
using Shuttle.Core.Data;
using Shuttle.Invoicing.Domain;

namespace Shuttle.Invoicing.DataAccess
{
    public class InvoiceQueryFactory : IInvoiceQueryFactory
    {
        public IQuery Add(Invoice invoice)
        {
            return new Query(@"
insert into dbo.Invoice
(
    Id,
    InvoiceNumber,
    InvoiceDate,
    OrderId,
    AccountContactName,
    AccountContactEMail
)
values
(
    @Id,
    @InvoiceNumber,
    @InvoiceDate,
    @OrderId,
    @AccountContactName,
    @AccountContactEMail
)
")
                .AddParameter(InvoiceColumns.Id, invoice.Id)
                .AddParameter(InvoiceColumns.InvoiceNumber, invoice.InvoiceNumber)
                .AddParameter(InvoiceColumns.InvoiceDate, invoice.InvoiceDate)
                .AddParameter(InvoiceColumns.OrderId, invoice.OrderId)
                .AddParameter(InvoiceColumns.AccountContactName, invoice.AccountContact.Name)
                .AddParameter(InvoiceColumns.AccountContactEMail, invoice.AccountContact.EMail);
        }

        public IQuery AddItem(InvoiceItem invoiceItem, Guid invoiceId)
        {
            return new Query(@"
insert into dbo.InvoiceItem
(
    InvoiceId,
    Description,
    Price
)
values
(
    @InvoiceId,
    @Description,
    @Price
)
")
                .AddParameter(InvoiceItemColumns.InvoiceId, invoiceId)
                .AddParameter(InvoiceItemColumns.Description, invoiceItem.Description)
                .AddParameter(InvoiceItemColumns.Price, invoiceItem.Price);
        }
    }
}