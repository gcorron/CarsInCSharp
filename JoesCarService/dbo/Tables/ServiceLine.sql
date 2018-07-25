CREATE TABLE [dbo].[ServiceLine] (
    [ServiceLineID]     INT           IDENTITY (1, 1) NOT NULL,
    [ServiceID]         INT           NOT NULL,
    [ServiceLineOrder]  TINYINT       NOT NULL,
    [ServiceLineType]   TINYINT       NOT NULL,
    [ServiceLineDesc]   NVARCHAR (50) NOT NULL,
    [ServiceLineCharge] MONEY         NOT NULL,
    CONSTRAINT [PK_ServiceLine] PRIMARY KEY NONCLUSTERED ([ServiceLineID] ASC)
);






GO
CREATE CLUSTERED INDEX [ClusteredIndex-20180702-190200]
    ON [dbo].[ServiceLine]([ServiceID] ASC);

