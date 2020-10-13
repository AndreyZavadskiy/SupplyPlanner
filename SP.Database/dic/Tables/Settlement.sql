CREATE TABLE [dic].[Settlement] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (100) NOT NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_Settlement] PRIMARY KEY CLUSTERED ([Id] ASC)
);

