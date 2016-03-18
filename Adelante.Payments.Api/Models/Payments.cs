using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adelante.Payments.Api.Models
{
    public class ls_adminusers
    {
        [Key]
        //[StringLength(20)]
        public int uid { get; set; }
        [StringLength(50)]
        public string username { get; set; }
        [StringLength(50)]
        public string pwd { get; set; }
        public int adminRole { get; set; }
        [StringLength(30)]
        public string fullname { get; set; }
        [StringLength(20)]
        public string location { get; set; }
        [StringLength(50)]
        public string emailAddr { get; set; }
        [StringLength(30)]
        public string telno { get; set; }
        public DateTime? lastAccess { get; set; }
        public DateTime? thisAccess { get; set; }
        [StringLength(10)]
        public string cashiercode { get; set; }
    }

    [Table("TransactionHistory")]
    public class TransactionHistory
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string tSessionId { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int seqNo { get; set; }
        public int tStatus { get; set; }
        public DateTime? tStartDate { get; set; }
        public DateTime? tEndDate { get; set; }
        [StringLength(30)]
        public string title { get; set; }
        [StringLength(60)]
        public string firstname { get; set; }
        [StringLength(30)]
        public string surname { get; set; }
        [StringLength(30)]
        public string tel { get; set; }
        [StringLength(50)]
        public string email { get; set; }
        [StringLength(40)]
        public string slReceiptNo { get; set; }
        [StringLength(40)]
        public string slBatchNo { get; set; }
        public int? slQRC { get; set; }
        [StringLength(30)]
        public string fullname { get; set; }
        [StringLength(10)]
        public string cardType { get; set; }
        [StringLength(30)]
        public string fundCode { get; set; }
        [StringLength(20)]
        public string custRef { get; set; }
        [StringLength(20)]
        public string custRef1 { get; set; }
        [StringLength(20)]
        public string custRef2 { get; set; }
        [StringLength(100)]
        public string custRef3 { get; set; }
        [StringLength(255)]
        public string description { get; set; }
        [DataType(DataType.Currency)]
        public int tAmount { get; set; }
        [DataType(DataType.Currency)]
        public int tVAT { get; set; }
        [StringLength(10)]
        public string tVATCode { get; set; }
        public int refTransNo { get; set; }
        [StringLength(10)]
        public string paymethod { get; set; }
        [StringLength(10)]
        public string cashierCode { get; set; }
    }

    public class ls_payDetail
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string tSessionId { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int seqNo { get; set; }
        [StringLength(20)]
        public string productId { get; set; }
        [StringLength(20)]
        public string custRef { get; set; }
        [StringLength(20)]
        public string custRef1 { get; set; }
        [StringLength(20)]
        public string custRef2 { get; set; }
        [StringLength(100)]
        public string custRef3 { get; set; }
        [StringLength(255)]
        public string description { get; set; }
        [DataType(DataType.Currency)]
        public int tAmount { get; set; }
        [DataType(DataType.Currency)]
        public int tVAT { get; set; }
        [StringLength(10)]
        public string tVATCode { get; set; }
        public int refTransNo { get; set; }
        [StringLength(10)]
        public string paymethod { get; set; }
        [StringLength(30)]
        public string chqnumber { get; set; }
    }

    public class ls_payheader
    {
        [Key]
        [StringLength(20)]
        public string tSessionId { get; set; }
        public int tStatus { get; set; }
        [StringLength(20)]
        public string tMerchantAc { get; set; }
        public DateTime? tStartDate { get; set; }
        public DateTime? tEndDate { get; set; }
        public int custId { get; set; }
        public int? slTransNo { get; set; }
        [StringLength(40)]
        public string slReceiptNo { get; set; }
        [StringLength(40)]
        public string slBatchNo { get; set; }
        public int? slQRC { get; set; }
        [StringLength(3)]
        public string slARC { get; set; }
        public int recStatus { get; set; }
        [StringLength(50)]
        public string exportId { get; set; }
        [StringLength(10)]
        public string cashierCode { get; set; }
        [StringLength(10)]
        public string cardType { get; set; }
        [StringLength(10)]
        public string pinAuth { get; set; }
        [StringLength(10)]
        public string till_code { get; set; }
        [StringLength(10)]
        public string location { get; set; }
    }

    public class ls_payrefunds
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string tSessionId { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int seqNo { get; set; }
        [StringLength(20)]
        public string opUserId { get; set; }
        [StringLength(100)]
        public string reason { get; set; }
        public DateTime tDate { get; set; }
        public int tAmount { get; set; }
        public int tVAT { get; set; }
        public int? slQRC { get; set; }
        [StringLength(3)]
        public string slARC { get; set; }
        public int? slTransNo { get; set; }
        public int recStatus { get; set; }
        [StringLength(50)]
        public string exportId { get; set; }
        [StringLength(10)]
        public string paymethod { get; set; }
    }

    public class TransactionPayment
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string tSessionId { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int seqNo { get; set; }

        public int tStatus { get; set; }

        public DateTime? tStartDate { get; set; }
        public DateTime? tEndDate { get; set; }

        [StringLength(40)]
        public string slReceiptNo { get; set; }

        public int? slQRC { get; set; }

        [StringLength(30)]
        public string fundCode { get; set; }

        [StringLength(30)]
        public string ledgerCode { get; set; }

        [StringLength(20)]
        public string custRef { get; set; }
        [StringLength(20)]
        public string custRef1 { get; set; }
        [StringLength(20)]
        public string custRef2 { get; set; }
        [StringLength(100)]
        public string custRef3 { get; set; }

        [StringLength(255)]
        public string description { get; set; }

        public int tAmount { get; set; }
        public int tVAT { get; set; }

        public int refTransNo { get; set; }

        [StringLength(10)]
        public string paymethod { get; set; }
        [StringLength(10)]
        public string cardType { get; set; }
        [StringLength(10)]
        public string cashierCode { get; set; }
        [StringLength(3)]
        public string source { get; set; }
    }

    public class TransactionRefund
    {
        [Key, Column(Order = 0)]
        [StringLength(20)]
        public string tSessionId { get; set; }
        
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int seqNo { get; set; }

        public int tStatus { get; set; }

        public DateTime tStartDate { get; set; }
        public DateTime tEndDate { get; set; }

        public int? slTransNo { get; set; }

        public int? slQRC { get; set; }

        [StringLength(30)]
        public string fundCode { get; set; }

        [StringLength(30)]
        public string ledgerCode { get; set; }

        [StringLength(20)]
        public string custRef { get; set; }

        [StringLength(20)]
        public string custRef1 { get; set; }

        [StringLength(20)]
        public string custRef2 { get; set; }

        [StringLength(100)]
        public string custRef3 { get; set; }

        [StringLength(100)]
        public string description { get; set; }

        public int tAmount { get; set; }

        public int tVAT { get; set; }

        public int refTransNo { get; set; }

        [StringLength(10)]
        public string paymethod { get; set; }

        [StringLength(10)]
        public string cardType { get; set; }

        [StringLength(10)]
        public string cashiercode { get; set; }
    }

    public class PaymentsData : DbContext
    {
        public PaymentsData()
            : base("name=Payments")
        {
        }

        public virtual DbSet<ls_adminusers> ls_adminusers { get; set; }
        public virtual DbSet<ls_payDetail> ls_payDetail { get; set; }
        public virtual DbSet<ls_payheader> ls_payheader { get; set; }
        public virtual DbSet<ls_payrefunds> ls_payrefunds { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }

        public virtual DbSet<TransactionPayment> Payments { get; set; }
        public virtual DbSet<TransactionRefund> Refunds { get; set; }
    }
}
