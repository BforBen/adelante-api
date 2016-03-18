CREATE VIEW [dbo].[TransactionRefunds]
AS
SELECT        dbo.ls_payrefunds.tSessionId, dbo.ls_payDetail.seqNo, 3 AS tStatus, dbo.ls_payrefunds.tDate AS tStartDate, dbo.ls_payrefunds.tDate AS tEndDate, 
                         dbo.ls_payrefunds.slTransNo, dbo.ls_payrefunds.slQRC, dbo.ls_funds.fundCode, 
                         CASE WHEN dbo.ls_funds.costCode = '' THEN dbo.ls_payDetail.custRef ELSE dbo.ls_funds.costCode END AS ledgerCode, dbo.ls_payDetail.custRef, 
                         dbo.ls_payDetail.custRef1, dbo.ls_payDetail.custRef2, dbo.ls_payDetail.custRef3, dbo.ls_payrefunds.reason AS description, 
                         CASE WHEN ls_payDetail.tAmount = 0 THEN (ls_payrefunds.tAmount * - 1) ELSE (ls_payDetail.tAmount * - 1) END AS tAmount, 
                         CASE WHEN ls_payDetail.tAmount = 0 THEN (ls_payrefunds.tVAT * - 1) ELSE (ls_payDetail.tVAT * - 1) END AS tVAT, dbo.ls_payDetail.refTransNo, 
                         dbo.ls_payrefunds.paymethod, dbo.ls_payheader.cardType, dbo.ls_adminusers.cashiercode
FROM            dbo.ls_payrefunds INNER JOIN
                         dbo.ls_payDetail ON dbo.ls_payrefunds.slTransNo = dbo.ls_payDetail.refTransNo AND dbo.ls_payrefunds.tSessionId = dbo.ls_payDetail.tSessionId INNER JOIN
                         dbo.ls_funds ON dbo.ls_payDetail.productId = dbo.ls_funds.fundID INNER JOIN
                         dbo.ls_payheader ON dbo.ls_payDetail.tSessionId = dbo.ls_payheader.tSessionId LEFT OUTER JOIN
                         dbo.ls_adminusers ON dbo.ls_payrefunds.opUserId = dbo.ls_adminusers.uid

GO


CREATE VIEW [dbo].[TransactionPayments]
AS
SELECT        dbo.ls_payheader.tSessionId, dbo.ls_payDetail.seqNo, dbo.ls_payheader.tStatus, dbo.ls_payheader.tStartDate, dbo.ls_payheader.tEndDate, 
                         dbo.ls_payheader.slReceiptNo, dbo.ls_payheader.slQRC, dbo.ls_funds.fundCode, 
                         CASE WHEN dbo.ls_funds.costCode = '' THEN dbo.ls_payDetail.custRef ELSE dbo.ls_funds.costCode END AS ledgerCode, dbo.ls_payDetail.custRef, 
                         dbo.ls_payDetail.custRef1, dbo.ls_payDetail.custRef2, dbo.ls_payDetail.custRef3, dbo.ls_payDetail.description, dbo.ls_payDetail.tAmount, dbo.ls_payDetail.tVAT, 
                         dbo.ls_payDetail.refTransNo, dbo.ls_payDetail.paymethod, dbo.ls_payheader.cardType, dbo.ls_payheader.cashierCode, 
                         CASE WHEN dbo.ls_payDetail.description LIKE '%CPi' THEN 'CPi' WHEN dbo.ls_payDetail.description LIKE '%CPx' THEN 'CPx' ELSE '' END AS source
FROM            dbo.ls_payheader INNER JOIN
                         dbo.ls_payDetail ON dbo.ls_payheader.tSessionId = dbo.ls_payDetail.tSessionId INNER JOIN
                         dbo.ls_funds ON dbo.ls_payDetail.productId = dbo.ls_funds.fundID

GO