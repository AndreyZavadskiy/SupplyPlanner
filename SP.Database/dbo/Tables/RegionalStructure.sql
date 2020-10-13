CREATE TABLE [dbo].[RegionalStructure] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [ParentId] INT            NULL,
    [Name]     NVARCHAR (200) NOT NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_RegionalStructure] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RegionalStructure_RegionalStructure_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[RegionalStructure] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_RegionalStructure_ParentId]
    ON [dbo].[RegionalStructure]([ParentId] ASC);

