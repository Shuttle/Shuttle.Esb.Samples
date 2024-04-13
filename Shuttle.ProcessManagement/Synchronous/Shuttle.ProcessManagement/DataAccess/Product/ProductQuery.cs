using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.ProcessManagement.DataAccess.Product
{
    public class ProductQuery : IProductQuery
    {
        public IEnumerable<DataRow> All()
        {
            var table = new DataTable();

            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("Url", typeof(string));

            table.Rows.Add(new Guid("{34D0AFA4-B4F5-44BC-8303-48AB233B4DB9}"),
                "Hard Edges, Practical Domain-Driven Design using C# - Eben Roux", 30,
                "https://www.amazon.com/Hard-Edges-Practical-Domain-Driven-Design/dp/1973158671");
            table.Rows.Add(new Guid("{BEE9B8F5-FE30-4C3E-BC3C-AACEB3D3B02C}"),
                "Patterns of Enterprise Application Architecture - Martin Fowler", 50,
                "https://www.amazon.com/Patterns-Enterprise-Application-Architecture-Martin/dp/0321127420");
            table.Rows.Add(new Guid("{9516EB77-5113-4D82-B638-692F10018B43}"),
                "Applying Domain-Driven Design Patterns - Jimmy Nilsson", 50,
                "https://www.amazon.com/Applying-Domain-Driven-Design-Patterns-Examples/dp/0321268202");
            table.Rows.Add(new Guid("{BABE9376-ACBE-4706-87E3-C7BAC02FF940}"),
                "Implementing Domain-Driven Design - Vaughn Vernon", 45,
                "https://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577");
            table.Rows.Add(new Guid("{68CAD2E3-C858-4FD1-8FE6-C2F9ED3E6AF9}"), "Domain-Driven Design - Eric Evans", 55,
                "https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215");
            table.Rows.Add(new Guid("{B6DDD092-6EC6-4310-8595-058F3C66ECF1}"), "Refactoring - Martin Fowler, et. al.",
                43, "https://www.amazon.com/Refactoring-Improving-Design-Existing-Code/dp/0201485672");
            table.Rows.Add(new Guid("{0F98EBDF-369F-4E95-83FE-3EFDE9358657}"), "Test Driven Development - Kent Beck",
                37, "https://www.amazon.com/Test-Driven-Development-Kent-Beck/dp/0321146530");

            return table.Rows.OfType<DataRow>();
        }

        public DataRow Get(Guid id)
        {
            var result = All().FirstOrDefault(dataRow => ProductColumns.Id.Value(dataRow).Equals(id));

            Guard.Against<ApplicationException>(result == null,
                $"Could not locate a product with id '{id}'.");

            return result;
        }
    }
}