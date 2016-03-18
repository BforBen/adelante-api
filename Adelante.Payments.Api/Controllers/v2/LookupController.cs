using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Adelante.Payments.Api.Models;
using Adelante.Payments.Api.Models.v2;
using System.Data.Entity.SqlServer;

namespace Adelante.Payments.Api.Controllers
{
    /// <summary>
    /// Payments lookup service
    /// </summary>
    [RoutePrefix("adelante/v2")]
    public class LookupV2Controller : ApiController
    {
        private List<Tuple<string, string>> Methods = new List<Tuple<string, string>>()
            { 
                new Tuple<string, string>("AM", "CCRD"),
                new Tuple<string, string>("BANKTAP1", "BANK"),
                new Tuple<string, string>("BANKTAP2", "BANK"),
                new Tuple<string, string>("BANKTAP3", "BANK"),
                new Tuple<string, string>("BANKTAP4", "BANK"),
                new Tuple<string, string>("BANKTAP5", "BANK"),
                new Tuple<string, string>("CARD", "CARD"),
                new Tuple<string, string>("CASH", "CASH"),
                new Tuple<string, string>("CASHPOST", "CASH"),
                new Tuple<string, string>("CHQ", "CHEQ"),
                new Tuple<string, string>("CHEQUE", "CHEQ"),
                new Tuple<string, string>("CHQPOST", "CHEQ"),
                new Tuple<string, string>("DD", "DD"),
                new Tuple<string, string>("GIRO", "GIRO"),
                new Tuple<string, string>("GIROIMP", "GIRO"),
                new Tuple<string, string>("INDEMNITY", "INDM"),
                new Tuple<string, string>("INTRACR", "CARD"),
                new Tuple<string, string>("INTRADB", "CARD"),
                new Tuple<string, string>("JRNL", "JRNL"),
                new Tuple<string, string>("MC", "CCRD"),
                new Tuple<string, string>("MD", "DCRD"),
                new Tuple<string, string>("MI", "DCRD"),
                new Tuple<string, string>("MIXED", "MIXD"),
                new Tuple<string, string>("PAYROLL", "PRLL"),
                new Tuple<string, string>("PETTYCASH", "CASH"),
                new Tuple<string, string>("REFDRAW", "DRAW"),
                new Tuple<string, string>("SW", "DCRD"),
                new Tuple<string, string>("TR", "TRFR"),
                new Tuple<string, string>("TRANBAI", "BALF"),
                new Tuple<string, string>("TS", "LODF"),
                new Tuple<string, string>("TU", "DEBT"),
                new Tuple<string, string>("UDD", "UNDD"),
                new Tuple<string, string>("VC", "CCRD"),
                new Tuple<string, string>("VD", "DCRD"),
                new Tuple<string, string>("VE", "DCRD"),
                new Tuple<string, string>("VP", "CCRD")
            };

        /// <summary>
        /// Returns a filtered list of transactions.
        /// </summary>
        /// <param name="id">Transcation ID</param>
        /// <param name="reference">List of references</param>
        /// <param name="fund">Fund code</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <param name="methods">Methods of payment to include</param>
        /// <param name="xmethods">Methods of payment to exclude</param>
        /// <returns>Returns a list of transactions.</returns>
        [HttpGet]
        [Route("transactions")]
        [ResponseType(typeof(IEnumerable<Transaction>))]
        public async Task<IHttpActionResult> Get(string id = null, [FromUri] IEnumerable<string> reference = null, string fund = null, DateTime? from = null, DateTime? to = null, [FromUri] IEnumerable<string> methods = null, [FromUri] IEnumerable<string> xmethods = null)
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

            if (methods.Where(m => !Methods.Select(k => k.Item2).Contains(m)).Count() > 0)
            {
                return BadRequest("Unknown method of payment specified in methods.");
            }

            if (xmethods.Where(m => !Methods.Select(k => k.Item2).Contains(m)).Count() > 0)
            {
                return BadRequest("Unknown method of payment specified in xmethods.");
            }

            methods = Methods.Where(m => methods.Contains(m.Item2)).Select(m => m.Item1).ToList();
            xmethods = Methods.Where(m => xmethods.Contains(m.Item2)).Select(m => m.Item1).ToList();
            
            using (var adb = new PaymentsData())
            {
                #region Payments

                var Payments = await adb.TransactionHistories
                    .Where(t => t.tStatus == 3 && (t.slQRC.HasValue && t.slQRC == 0))
                    .WhereIf(!String.IsNullOrWhiteSpace(id), t => t.tSessionId == id)
                    .WhereIf(reference.Count() > 0, t => reference.Contains(t.custRef) || reference.Contains(t.custRef1) || reference.Contains(t.custRef2) || reference.Contains(t.custRef3))
                    .WhereIf(!String.IsNullOrWhiteSpace(fund), t => t.fundCode == fund)
                    .WhereIf(from.HasValue, t => t.tStartDate >= from)
                    .WhereIf(to.HasValue, t => t.tStartDate < to)
                    .WhereIf(methods.Count() > 0, t => methods.Contains(t.paymethod))
                    .WhereIf(xmethods.Count() > 0, t => !xmethods.Contains(t.paymethod))
                    .Select(t => new Transaction
                    {
                        Account = (new List<string>() { t.custRef.Trim(), t.custRef1.Trim(), t.custRef2.Trim(), t.custRef3.Trim() }).Where(s => !String.IsNullOrEmpty(s)),
                        Amount = (decimal)t.tAmount / 100m,
                        Channel = (t.cashierCode == "TP" ? "ATP" : t.paymethod),
                        Date = t.tStartDate.Value,
                        Description = t.description,
                        Fund = t.fundCode,
                        Method = t.paymethod + "|" + t.cardType,
                        Recorded = t.tStartDate.Value,
                        Id = t.tSessionId,
                        User = (t.custRef3.StartsWith(@"GUILDFORD\")) ? t.custRef3 : t.cashierCode,
                        Vat = (decimal)t.tVAT / 100m
                    }).ToListAsync();

                #endregion

                var TransactionIds = Payments.Select(t => t.Id).Distinct();

                /* 
                 * SQL Server was bombing out trying to filter to list of transactions IDs so have changed to load all as there aren't that many (at the moment)
                 * but will need to review later as it's not a good solution longer term. It doesn't help the database isn't indexed or normalized or even using keys 
                 * that are the same type.
                 * 
                 */

                #region Full refunds

                //var FullyRefundedTransactions = await (from r in adb.ls_payrefunds
                //                                       join t in adb.TransactionHistories on r.slTransNo.Value equals t.refTransNo
                //                                       join a in adb.ls_adminusers on r.opUserId equals SqlFunctions.StringConvert((double)a.uid).Trim() into au
                //                                       from user in au.DefaultIfEmpty() 
                //                                       where t.refTransNo > 0 && TransactionIds.Contains(r.tSessionId) && t.tAmount > 0 
                //                                       select new Transaction
                //                                       {
                //                                           Account = (new List<string>() { t.custRef.Trim(), t.custRef1.Trim(), t.custRef2.Trim(), t.custRef3.Trim() }).Where(s => !String.IsNullOrEmpty(s)),
                //                                           Amount = ((decimal)t.tAmount / 100m) * -1,
                //                                           Channel = "Staff",
                //                                           Date = r.tDate,
                //                                           Description = r.reason,
                //                                           Fund = t.fundCode,
                //                                           Method = r.paymethod,
                //                                           Recorded = r.tDate,
                //                                           Id = t.tSessionId,
                //                                           User = user.username,
                //                                           Vat = ((decimal)t.tVAT / 100m) * -1
                //                                       }).ToListAsync();


                var FullyRefundedTransactions = await (from r in adb.ls_payrefunds
                                                       join t in adb.TransactionHistories on r.slTransNo.Value equals t.refTransNo
                                                       join a in adb.ls_adminusers on r.opUserId equals SqlFunctions.StringConvert((double)a.uid).Trim() into au
                                                       from user in au.DefaultIfEmpty()
                                                       where t.refTransNo > 0 && t.tAmount > 0
                                                       select new Transaction
                                                       {
                                                           Account = (new List<string>() { t.custRef.Trim(), t.custRef1.Trim(), t.custRef2.Trim(), t.custRef3.Trim() }).Where(s => !String.IsNullOrEmpty(s)),
                                                           Amount = ((decimal)t.tAmount / 100m) * -1,
                                                           Channel = "Staff",
                                                           Date = r.tDate,
                                                           Description = r.reason,
                                                           Fund = t.fundCode,
                                                           Method = r.paymethod,
                                                           Recorded = t.tStartDate.Value,
                                                           Id = r.tSessionId,
                                                           User = user.username,
                                                           Vat = ((decimal)t.tVAT / 100m) * -1
                                                       }).ToListAsync();

                #endregion

                #region Partial refunds

                //var PartiallyRefundedTransactions = await (from r in adb.ls_payrefunds
                //                                           join t in adb.TransactionHistories on r.slTransNo.Value equals t.refTransNo
                //                                           join a in adb.ls_adminusers on r.opUserId equals SqlFunctions.StringConvert((double)a.uid).Trim() into au
                //                                           from user in au.DefaultIfEmpty()
                //                                           where t.refTransNo > 0 && TransactionIds.Contains(r.tSessionId) && t.tAmount == 0
                //                                           select new Transaction
                //                                           {
                //                                               Account = (new List<string>() { t.custRef.Trim(), t.custRef1.Trim(), t.custRef2.Trim(), t.custRef3.Trim() }).Where(s => !String.IsNullOrEmpty(s)),
                //                                               Amount = ((decimal)r.tAmount / 100m) * -1,
                //                                               Channel = "Staff",
                //                                               Date = r.tDate,
                //                                               Description = r.reason,
                //                                               Fund = t.fundCode,
                //                                               Method = r.paymethod,
                //                                               Recorded = r.tDate,
                //                                               Id = r.tSessionId,
                //                                               User = user.cashiercode,
                //                                               Vat = ((decimal)r.tVAT / 100m) * -1
                //                                           }).ToListAsync();

                var PartiallyRefundedTransactions = await (from r in adb.ls_payrefunds
                                                           join t in adb.TransactionHistories on r.slTransNo.Value equals t.refTransNo
                                                           join a in adb.ls_adminusers on r.opUserId equals SqlFunctions.StringConvert((double)a.uid).Trim() into au
                                                           from user in au.DefaultIfEmpty()
                                                           where t.refTransNo > 0 && t.tAmount == 0
                                                           select new Transaction
                                                           {
                                                               Account = (new List<string>() { t.custRef.Trim(), t.custRef1.Trim(), t.custRef2.Trim(), t.custRef3.Trim() }).Where(s => !String.IsNullOrEmpty(s)),
                                                               Amount = ((decimal)r.tAmount / 100m) * -1,
                                                               Channel = "Staff",
                                                               Date = r.tDate,
                                                               Description = r.reason,
                                                               Fund = t.fundCode,
                                                               Method = r.paymethod,
                                                               Recorded = r.tDate,
                                                               Id = t.tSessionId,
                                                               User = user.cashiercode,
                                                               Vat = ((decimal)r.tVAT / 100m) * -1
                                                           }).ToListAsync();

                #endregion

                // Now filter to relevant transactions

                var Refunds = FullyRefundedTransactions.Concat(PartiallyRefundedTransactions).Where(r => TransactionIds.Contains(r.Id));

                Refunds = Refunds
                    .WhereIf(reference.Count() > 0, t => reference.Intersect(t.Account, StringComparer.CurrentCultureIgnoreCase).Count() > 0)
                    .WhereIf(!String.IsNullOrWhiteSpace(fund), t => t.Fund == fund)
                    .WhereIf(from.HasValue, t => t.Date >= from)
                    .WhereIf(to.HasValue, t => t.Date < to)
                    .WhereIf(methods.Count() > 0, t => methods.Contains(t.Method))
                    .WhereIf(xmethods.Count() > 0, t => !xmethods.Contains(t.Method));

                var Transactions = Payments.Concat(Refunds).Select(t =>
                    {
                        t.Channel = GetChannel(t.Description, t.Channel);
                        t.Method = GetMethod(t.Method);
                        return t;
                    });

                return Ok(Transactions.OrderBy(t => t.Recorded));
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

        private IEnumerable<string> TrimList(IEnumerable<string> list)
        {
            return list.Where(s => !String.IsNullOrWhiteSpace(s));
        }

        private string GetChannel(string description, string cardType)
        {
            if (cardType == "ATP" || cardType == "Staff")
            {
                return cardType;
            }

            if (description.Contains("CPx"))
            {
                return "Web";
            }

            if (description.Contains("CPi"))
            {
                return "CSC";
            }

            return "Unknown";
        }

        private string GetMethod(string cardType)
        {
            var type = cardType.Split('|');

            var Method = Methods.Where(m => m.Item1 == type[0]).SingleOrDefault();

            if (Method != null && Method.Item2 == "CARD" && type.Count() > 1)
            {
                Method = Methods.Where(m => m.Item1 == type[1]).SingleOrDefault();
            }

            return (Method != null) ? Method.Item2 : "OTHR";
        }
    }
}
