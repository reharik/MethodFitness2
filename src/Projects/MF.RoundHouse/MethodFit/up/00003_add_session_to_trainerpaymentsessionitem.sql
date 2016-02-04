alter TABLE [dbo].[TrainerPaymentSessionItem] add [SessionId] [int] NULL


ALTER TABLE [dbo].[TrainerPaymentSessionItem]  WITH CHECK ADD  CONSTRAINT [FK_TrainerPaymentSessionItem_manyToOne_Session] FOREIGN KEY([SessionId])
REFERENCES [dbo].[Session] ([EntityId])
GO