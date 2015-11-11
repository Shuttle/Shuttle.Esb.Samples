using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ProcessManagement
{
    public class ProductQuery : IProductQuery
    {
        public IEnumerable<DataRow> All()
        {
            var table = new DataTable();

            table.Columns.Add("Id", typeof (Guid));
            table.Columns.Add("Description", typeof (string));
            table.Columns.Add("Price", typeof (decimal));

            table.Rows.Add(new Guid("{BEE9B8F5-FE30-4C3E-BC3C-AACEB3D3B02C}"), "Patterns of Enterprise Application Architecture - Martin Fowler", 50);
            table.Rows.Add(new Guid("{9516EB77-5113-4D82-B638-692F10018B43}"), "Applying Domain-Driven Design Patterns - Jimmy Nilsson", 50);
            table.Rows.Add(new Guid("{BABE9376-ACBE-4706-87E3-C7BAC02FF940}"), "Implementing Domain-Driven Design - Vaughn Vernon", 45);
            table.Rows.Add(new Guid("{68CAD2E3-C858-4FD1-8FE6-C2F9ED3E6AF9}"), "Domain-Driven Design - Eric Evans", 55);
            table.Rows.Add(new Guid("{B6DDD092-6EC6-4310-8595-058F3C66ECF1}"), "Refactoring - Martin Fowler, et. al.", 43);
            table.Rows.Add(new Guid("{0F98EBDF-369F-4E95-83FE-3EFDE9358657}"), "Test Driven Development - Kent Beck", 37);

            return table.Rows.OfType<DataRow>();
        }

        public DataRow Get(Guid id)
        {
            var result = All().FirstOrDefault(dataRow => ProductColumns.Id.MapFrom(dataRow).Equals(id));

            Guard.Against<ApplicationException>(result == null, string.Format("Could not locate a product with id '{0}'.", id));

            return result;
        }
    }
}