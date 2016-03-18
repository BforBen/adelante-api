using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Adelante.Payments.Api.Models;
using System.Data.Entity.SqlServer;

namespace Adelante.Payments.Api.Controllers
{
    /// <summary>
    /// Payments lookup service
    /// </summary>
    [RoutePrefix("adelante/v3")]
    public class PerformanceV3Controller : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ledgerCode"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("summary")]
        [ResponseType(typeof(IEnumerable<Summary>))]
        public async Task<IHttpActionResult> Summary(string ledgerCode = null, DateTime? from = null, DateTime? to = null)
        {
            Serilog.Log.Information("Test");
            var Start = from ?? new DateTime((DateTime.Now.Year - 1), 1, 1);

            using (var adb = new PaymentsData())
            {
                // use to generate an error to test logging , test = System.Text.RegularExpressions.Regex.Match("test", t.description).Value
                
                var Payments = await adb.Payments
                        .Where(t => t.tStatus == 3 && (t.slQRC.HasValue && t.slQRC == 0) && t.tStartDate >= Start)
                        .WhereIf(!String.IsNullOrWhiteSpace(ledgerCode), t => t.ledgerCode == ledgerCode)
                        .WhereIf(to.HasValue, t => t.tStartDate < to)
                        .GroupBy(t => new { Date = DbFunctions.TruncateTime(t.tStartDate.Value), t.ledgerCode, t.paymethod, t.cardType, t.cashierCode, t.source })
                        .ToListAsync();

                var PaymentsSummary = Payments.Select(t => new Summary
                        {
                            Date = t.Key.Date.Value,
                            LedgerCode = t.Key.ledgerCode,
                            Transactions = t.Count(),
                            Total = (t.Sum(p => (decimal)p.tAmount) / 100m),
                            Average = (t.Average(p => (decimal)p.tAmount) / 100m),
                            Method = Utils.GetMethod(t.Key.paymethod + "|" + t.Key.cardType),
                            Channel = Utils.GetChannel(t.Key.source, t.Key.paymethod, t.Key.cashierCode)
                        })
                        // Group by again to handle channels and methods being resolved to the same thing
                        .GroupBy(t => new { t.Date, t.LedgerCode, t.Method, t.Channel })
                        .Select(t => new Summary
                        {
                            Date = t.Key.Date,
                            LedgerCode = t.Key.LedgerCode,
                            Transactions = t.Sum(p => p.Transactions),
                            Total = t.Sum(p => p.Total),
                            Average = t.Average(p => (decimal)p.Average),
                            Method = t.Key.Method,
                            Channel = t.Key.Channel
                        });

                var Refunds = await adb.Refunds
                        .Where(t => t.tStartDate >= Start)
                        .WhereIf(to.HasValue, t => t.tStartDate < to)
                        .WhereIf(!String.IsNullOrWhiteSpace(ledgerCode), t => t.ledgerCode == ledgerCode)
                        .GroupBy(t => new { Date = DbFunctions.TruncateTime(t.tStartDate), t.ledgerCode, t.paymethod, t.cardType })
                        .ToListAsync();

                var RefundSummary = Refunds.Select(t => new Summary
                        {
                            Date = t.Key.Date.Value,
                            LedgerCode = t.Key.ledgerCode,
                            Transactions = t.Count(),
                            Total = (t.Sum(p => (decimal)p.tAmount) / 100m),
                            Average = (t.Average(p => (decimal)p.tAmount) / 100m),
                            Method = Utils.GetMethod(t.Key.paymethod + "|" + t.Key.cardType),
                            Channel = "Staff"
                        })
                    // Group by again to handle channels and methods being resolved to the same thing
                        .GroupBy(t => new { t.Date, t.LedgerCode, t.Method, t.Channel })
                        .Select(t => new Summary
                        {
                            Date = t.Key.Date,
                            LedgerCode = t.Key.LedgerCode,
                            Transactions = t.Sum(p => p.Transactions),
                            Total = t.Sum(p => p.Total),
                            Average = t.Average(p => (decimal)p.Average),
                            Method = t.Key.Method,
                            Channel = t.Key.Channel
                        });

                return Ok(PaymentsSummary.Concat(RefundSummary).OrderBy(t => t.Date).ThenBy(t => t.LedgerCode).ThenBy(t => t.Channel).ThenBy(t => t.Method));
            }
        }
    }
}
