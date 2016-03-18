using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;
using Adelante.Payments.Api.Controllers;
using Adelante.Payments.Api.Models.v3;

namespace Adelante.Payments.Api.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public LookupV3Controller Controller = new LookupV3Controller();

        [TestMethod]
        public async Task TestFullRefund()
        {
            var TransactionsRequest = await Controller.Get(reference: new List<string>() { "20215158" }, fund: "03", from: new DateTime(2015, 01, 01), to: new DateTime(2015, 04, 01));

            Assert.IsInstanceOfType(TransactionsRequest, typeof(OkNegotiatedContentResult<IOrderedEnumerable<Transaction>>), "Request failed.");

            var TransactionsContent = TransactionsRequest as OkNegotiatedContentResult<IOrderedEnumerable<Transaction>>;

            var Transactions = TransactionsContent.Content;

            Assert.IsNotNull(Transactions, "No transactions returned.");

            Assert.AreEqual<int>(3, Transactions.Count(), Transactions.Count().ToString() + " transaction(s) were found and 3 were expected.");

            Assert.AreEqual<int>(1, Transactions.Where(t => t.Amount < 0).Count(), "No refunds found.");
        }

        [TestMethod]
        public async Task TestSessionWithFullRefund()
        {
            var TransactionsRequest = await Controller.GetPayment(id: "29RV-6E0L-3NR4");

            Assert.IsInstanceOfType(TransactionsRequest, typeof(OkNegotiatedContentResult<IOrderedEnumerable<Transaction>>), "Request failed.");

            var TransactionsContent = TransactionsRequest as OkNegotiatedContentResult<IOrderedEnumerable<Transaction>>;

            var Transactions = TransactionsContent.Content;

            Assert.IsNotNull(Transactions, "No transactions returned.");

            Assert.AreEqual<int>(2, Transactions.Count(), Transactions.Count().ToString() + " transaction(s) were found and 2 were expected.");

            Assert.AreEqual<int>(1, Transactions.Where(t => t.Amount < 0).Count(), "No refunds found.");
        }
    }
}
