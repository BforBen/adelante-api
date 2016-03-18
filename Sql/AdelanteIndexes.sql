

CREATE CLUSTERED INDEX [IX_ProductId_SessionId] ON [dbo].[ls_payDetail]
(
	[productId] ASC,
	[tSessionId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY];

CREATE NONCLUSTERED INDEX [IX_SessionId_QRC_Status] ON [dbo].[ls_payheader]
(
	[tSessionId] ASC,
	[slQRC] ASC,
	[tStatus] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY];


CREATE NONCLUSTERED INDEX [IX_ProductId_SessionId_Inc] ON [dbo].[ls_payDetail]
(
	[tSessionId] ASC,
	[productId] ASC
)
INCLUDE ( 	[custRef],
	[custRef1],
	[custRef2],
	[custRef3],
	[description],
	[tAmount],
	[tVAT],
	[refTransNo],
	[paymethod]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY];


CREATE STATISTICS [STAT_Status_QRC] ON [dbo].[ls_payheader]([tStatus], [slQRC]);
CREATE STATISTICS [STAT_SessionId_Status_QRC] ON [dbo].[ls_payheader]([tSessionId], [tStatus], [slQRC]);
CREATE STATISTICS [STAT_SessionId_QRC] ON [dbo].[ls_payheader]([tSessionId], [slQRC]);
CREATE STATISTICS [STAT_ProductId_SessionId] ON [dbo].[ls_payDetail]([productId], [tSessionId]);
CREATE STATISTICS [STAT_ProductId_CustRefs3] ON [dbo].[ls_payDetail]([productId], [custRef], [custRef1], [custRef2]);
CREATE STATISTICS [STAT_SessionId_CustRefs3] ON [dbo].[ls_payDetail]([tSessionId], [custRef], [custRef1], [custRef2]);
CREATE STATISTICS [STAT_ProductId_CustRefs4] ON [dbo].[ls_payDetail]([custRef], [custRef1], [custRef2], [custRef3], [productId]);
CREATE STATISTICS [STAT_SessionId_CustRefs4] ON [dbo].[ls_payDetail]([custRef], [custRef1], [custRef2], [custRef3], [tSessionId]);
