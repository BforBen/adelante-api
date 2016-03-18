using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Adelante.Payments.Api.Models;
using Adelante.Payments.Api.Models.v3;

namespace Adelante.Payments.Api.Controllers
{
    /// <summary>
    /// Payments lookup service
    /// </summary>
    [RoutePrefix("adelante/v3")]
    public class LookupV3Controller : ApiController
    {
        /// <summary>
        /// Returns a filtered list of transactions.
        /// </summary>
        /// <param name="id">Transcation ID</param>
        /// <param name="reference">List of references</param>
        /// <param name="fund">Fund code</param>
        /// <param name="ledgerCode">Ledger code</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <param name="methods">Methods of payment to include</param>
        /// <param name="xmethods">Methods of payment to exclude</param>
        /// <returns>Returns a list of transactions.</returns>
        [HttpGet]
        [Route("transactions")]
        [ResponseType(typeof(IEnumerable<Transaction>))]
        public async Task<IHttpActionResult> Get(string id = null, [FromUri] IEnumerable<string> reference = null, string fund = null, string ledgerCode = null, DateTime? from = null, DateTime? to = null, [FromUri] IEnumerable<string> methods = null, [FromUri] IEnumerable<string> xmethods = null)
        {
            if (reference == null)
            {
                reference = new List<string>();
            }

            if (methods == null)
            {
                methods = new List<string>();
            }

            if (xmethods == null)
            {
                xmethods = new List<string>();
            }

            reference = reference.Where(r => !String.IsNullOrWhiteSpace(r));
            methods = methods.Where(m => !String.IsNullOrWhiteSpace(m)).Select(m => m.ToUpper());
            xmethods = xmethods.Where(m => !String.IsNullOrWhiteSpace(m)).Select(m => m.ToUpper());

            if (methods.Count() > 0 && xmethods.Count() > 0)
            {
                var Both = methods.Intersect(xmethods);

                if (Both.Count() > 0)
                {
                    return BadRequest("The same type of payment exists in both the include and exclude lists.");
                }
            }

            if (methods.Where(m => !Utils.Methods.Select(k => k.Item2).Contains(m)).Count() > 0)
            {
                return BadRequest("Unknown method of payment specified in methods.");
            }

            if (xmethods.Where(m => !Utils.Methods.Select(k => k.Item2).Contains(m)).Count() > 0)
            {
                return BadRequest("Unknown method of payment specified in xmethods.");
            }

            methods = Utils.Methods.Where(m => methods.Contains(m.Item2)).Select(m => m.Item1).ToList();
            xmethods = Utils.Methods.Where(m => xmethods.Contains(m.Item2)).Select(m => m.Item1).ToList();
            
            using (var adb = new PaymentsData())
            {
                #region Payments

                var Payments = await adb.Payments
                    .Where(t => t.tStatus == 3 && (t.slQRC.HasValue && t.slQRC == 0))
                    .WhereIf(!String.IsNullOrWhiteSpace(id), t => t.tSessionId == id)
                    .WhereIf(reference.Count() > 0, t => reference.Contains(t.custRef) || reference.Contains(t.custRef1) || reference.Contains(t.custRef2) || reference.Contains(t.custRef3))
                    .WhereIf(!String.IsNullOrWhiteSpace(fund), t => t.fundCode == fund)
                    .WhereIf(!String.IsNullOrWhiteSpace(ledgerCode), t => t.ledgerCode == ledgerCode)
                    .WhereIf(from.HasValue, t => t.tStartDate >= from)
                    .WhereIf(to.HasValue, t => t.tStartDate < to)
                    .WhereIf(methods.Count() > 0, t => methods.Contains(t.paymethod))
                    .WhereIf(xmethods.Count() > 0, t => !xmethods.Contains(t.paymethod))
                    .Select(t => new Transaction
                    {
                        Account = (new List<string>() { t.custRef.Trim(), t.custRef1.Trim(), t.custRef2.Trim(), t.custRef3.Trim() }).Where(s => !String.IsNullOrEmpty(s)),
                        Amount = (decimal)t.tAmount / 100m,
                        Channel = t.paymethod,
                        Date = t.tStartDate.Value,
                        Description = t.description,
                        LedgerCode = t.ledgerCode,
                        Method = t.paymethod + "|" + t.cardType,
                        Id = t.tSessionId,
                        User = (t.custRef3.StartsWith(@"GUILDFORD\")) ? t.custRef3 : t.cashierCode,
                        Vat = (decimal)t.tVAT / 100m
                    }).ToListAsync();

                #endregion

                var TransactionIds = Payments.Select(t => t.Id).Distinct();

                #region Refunds

                var Refunds = await adb.Refunds
                    .WhereIf(!String.IsNullOrWhiteSpace(id), t => t.tSessionId == id)
                    .WhereIf(reference.Count() > 0, t => reference.Contains(t.custRef) || reference.Contains(t.custRef1) || reference.Contains(t.custRef2) || reference.Contains(t.custRef3))
                    .WhereIf(!String.IsNullOrWhiteSpace(fund), t => t.fundCode == fund)
                    .WhereIf(!String.IsNullOrWhiteSpace(ledgerCode), t => t.ledgerCode == ledgerCode)
                    .WhereIf(from.HasValue, t => t.tStartDate >= from)
                    .WhereIf(to.HasValue, t => t.tStartDate < to)
                    .WhereIf(methods.Count() > 0, t => methods.Contains(t.paymethod))
                    .WhereIf(xmethods.Count() > 0, t => !xmethods.Contains(t.paymethod))
                    .Select(t => new Transaction
                    {
                        Account = (new List<string>() { t.custRef.Trim(), t.custRef1.Trim(), t.custRef2.Trim(), t.custRef3.Trim() }).Where(s => !String.IsNullOrEmpty(s)),
                        Amount = ((decimal)t.tAmount / 100m),
                        Channel = "Staff",
                        Date = t.tStartDate,
                        Description = t.description,
                        LedgerCode = t.ledgerCode,
                        Method = t.paymethod + "|" + t.cardType,
                        Id = t.tSessionId,
                        User = (t.custRef3.StartsWith(@"GUILDFORD\")) ? t.custRef3 : t.cashiercode,
                        Vat = (decimal)t.tVAT / 100m
                    }).ToListAsync();

                #endregion

                var Transactions = Payments.Concat(Refunds).Select(t =>
                    {
                        t.Channel = Utils.GetChannel(t.Description, t.Channel, t.User);
                        t.Method = Utils.GetMethod(t.Method);
                        return t;
                    });

                return Ok(Transactions.OrderBy(t => t.Date));
            }
        }

        /// <summary>
        /// Returns details of a specific transaction.
        /// </summary>
        /// <param name="id">The transaction reference number</param>
        /// <returns>Returns the details of the transaction with the specified reference number.</returns>
        [HttpGet]
        [Route("transactions/{id}")]
        [ResponseType(typeof(IEnumerable<Transaction>))]
        public async Task<IHttpActionResult> GetPayment(string id = null)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("No transaction reference specified.");
            }

            return await Get(id);
        }
    }
}
