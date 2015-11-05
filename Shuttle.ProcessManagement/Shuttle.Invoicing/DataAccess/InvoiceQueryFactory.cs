using System;
using Shuttle.Core.Data;
using Shuttle.Invoicing.Domain;

namespace Shuttle.Invoicing.DataAccess
{
    public class InvoiceQueryFactory : IInvoiceQueryFactory
    {
        public IQuery Add(Invoice invoice)
        {
            return RawQuery.Create(@"
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
                .AddParameterValue(InvoiceColumns.Id, invoice.Id)
                .AddParameterValue(InvoiceColumns.InvoiceNumber, invoice.InvoiceNumber)
                .AddParameterValue(InvoiceColumns.InvoiceDate, invoice.InvoiceDate)
                .AddParameterValue(InvoiceColumns.OrderId, invoice.OrderId)
                .AddParameterValue(InvoiceColumns.AccountContactName, invoice.AccountContact.Name)
                .AddParameterValue(InvoiceColumns.AccountContactEMail, invoice.AccountContact.EMail);
        }

        public IQuery AddItem(InvoiceItem invoiceItem, Guid invoiceId)
        {
            return RawQuery.Create(@"
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
                .AddParameterValue(InvoiceItemColumns.InvoiceId, invoiceId)
                .AddParameterValue(InvoiceItemColumns.Description, invoiceItem.Description)
                .AddParameterValue(InvoiceItemColumns.Price, invoiceItem.Price);
        }
    }
}