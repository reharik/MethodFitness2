
/****** Object:  Table [dbo].[ClientStatus]    Script Date: 8/31/2014 12:01:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientStatus](
	[EntityId] [int] IDENTITY(1,1) NOT NULL,
	[AdminAlerted] [bit] NULL,
	[ClientContacted] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[CreatedById] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ChangedById] [int] NULL,
	[ChangedDate] [datetime] NULL,
	[CompanyId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[EntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClientStatus] ADD  CONSTRAINT [DF_ClientStatus_AdminAlerted]  DEFAULT ((0)) FOR [AdminAlerted]
GO

ALTER TABLE [dbo].[ClientStatus] ADD  CONSTRAINT [DF_ClientStatus_ClientContacted]  DEFAULT ((0)) FOR [ClientContacted]
GO


