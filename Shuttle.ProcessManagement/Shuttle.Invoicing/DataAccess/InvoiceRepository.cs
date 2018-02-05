using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Invoicing.Domain;

namespace Shuttle.Invoicing.DataAccess
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IInvoiceQueryFactory _queryFactory;

        public InvoiceRepository(IDatabaseGateway databaseGateway, IInvoiceQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));

            _databaseGateway = databaseGateway;
            _queryFactory = queryFactory;
        }

        public void Add(Invoice invoice)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Add(invoice));

            foreach (var item in invoice.Items)
            {
                _databaseGateway.ExecuteUsing(_queryFactory.AddItem(item, invoice.Id));
            }
        }
    }
}