CREATE TABLE [dbo].[Car] (
    [CarID] INT           IDENTITY (1, 1) NOT NULL,
    [Make]  NVARCHAR (50) NOT NULL,
    [Model] NVARCHAR (50) NOT NULL,
    [Year]  INT           NOT NULL,
    [Owner] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Auto] PRIMARY KEY CLUSTERED ([CarID] ASC)
);

