CREATE TABLE [dbo].[Service] (
    [ServiceID]   INT           IDENTITY (1, 1) NOT NULL,
    [ServiceDate] SMALLDATETIME NOT NULL,
    [TechName]    NVARCHAR (50) NOT NULL,
    [LaborCost]   MONEY         NOT NULL,
    [PartsCost]   MONEY         NOT NULL,
    [CarID]       INT           NOT NULL,
    CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED ([ServiceID] ASC),
	INDEX [IX_Service] (CarID),
    CONSTRAINT [FK_Service_Car] FOREIGN KEY ([CarID]) REFERENCES [dbo].[Car] ([CarID])
);

