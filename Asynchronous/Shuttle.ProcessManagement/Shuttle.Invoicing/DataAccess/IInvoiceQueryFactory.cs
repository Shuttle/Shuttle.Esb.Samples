using System;
using Shuttle.Core.Data;
using Shuttle.Invoicing.Domain;

namespace Shuttle.Invoicing.DataAccess
{
    public interface IInvoiceQueryFactory
    {
        IQuery Add(Invoice invoice);
        IQuery AddItem(InvoiceItem invoiceItem, Guid invoiceId);
    }
}