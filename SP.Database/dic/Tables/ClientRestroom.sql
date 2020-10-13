CREATE TABLE [dic].[ClientRestroom] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (100) NOT NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_ClientRestroom] PRIMARY KEY CLUSTERED ([Id] ASC)
);

