/****** Object:  View [dbo].[TrainerSessions]    Script Date: 12/29/2012 7:56:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[TrainerSessions]')) 
drop view [dbo].[TrainerSessions]
go

CREATE VIEW [dbo].[TrainerSessions]
AS
SELECT        ses.EntityId, ses.Cost AS PricePerSession, ses.AppointmentType AS Type, ses.TrainerVerified, ses.TrainerSessionVerificationId, ses.InArrears, ses.ClientId, 
                         ses.TrainerPaid, app.Date AS AppointmentDate, Client.FirstName, Client.LastName, ISNULL(tcr.[Percent],
                             (SELECT        ClientRateDefault
                               FROM            dbo.[User]
                               WHERE        (EntityId = app.TrainerId))) AS TrainerPercentage, app.EntityId AS AppId, app.TrainerId
FROM            dbo.Appointment AS app LEFT OUTER JOIN
                         dbo.Session AS ses ON ses.AppointmentId = app.EntityId LEFT OUTER JOIN
                         dbo.Client AS Client ON ses.ClientId = Client.EntityId LEFT OUTER JOIN
                         dbo.TrainerClientRate AS tcr ON tcr.ClientId = Client.EntityId AND tcr.TrainerId = app.TrainerId
WHERE        (app.IsDeleted = 0)

GO

