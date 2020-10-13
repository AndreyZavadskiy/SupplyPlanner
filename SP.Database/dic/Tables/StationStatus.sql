CREATE TABLE [dic].[StationStatus] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (100) NOT NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_StationStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

