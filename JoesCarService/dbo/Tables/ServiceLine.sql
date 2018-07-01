CREATE TABLE [dbo].[ServiceLine] (
    [ServiceID]         INT           NOT NULL,
    [ServiceLineOrder]  TINYINT       NOT NULL,
    [ServiceLineType]   TINYINT       NOT NULL,
    [ServiceLineDesc]   NVARCHAR (50) NOT NULL,
    [ServiceLineCharge] MONEY         NOT NULL
);

