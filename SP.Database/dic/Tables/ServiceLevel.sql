CREATE TABLE [dic].[ServiceLevel] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (100) NOT NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_ServiceLevel] PRIMARY KEY CLUSTERED ([Id] ASC)
);

