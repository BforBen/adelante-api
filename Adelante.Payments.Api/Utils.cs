using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adelante.Payments.Api
{
    public static class Utils
    {
        public static List<Tuple<string, string>> Methods = new List<Tuple<string, string>>()
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

        public static IEnumerable<string> TrimList(IEnumerable<string> list)
        {
            return list.Where(s => !String.IsNullOrWhiteSpace(s));
        }

        public static string GetChannel(string description, string payMethod, string cashierCode = null)
        {
            var Channel = "Unknown";

            if (payMethod == "TP" || payMethod == "ATP")
            {
                Channel = "ATP";
            }

            if (payMethod == "Staff" 
                || payMethod == "CASH" 
                || payMethod.StartsWith("CHQ") 
                || payMethod.StartsWith("CASH")
                || payMethod == "CHEQUE"
                || payMethod == "POPOST"
                )
            {
                Channel = "Staff";
            }

            if (description.Trim().EndsWith("CPx"))
            {
                Channel = "Web";
            }

            if (description.Trim().EndsWith("CPi"))
            {
                Channel = "CSC";
            }

            if (cashierCode != null)
            {
                if (cashierCode == "WEB")
                {
                    Channel = "Web";
                }
            }

            return Channel;
        }

        public static string GetMethod(string cardType)
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
