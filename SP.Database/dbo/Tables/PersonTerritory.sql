CREATE TABLE [dbo].[PersonTerritory] (
    [Id]                  INT IDENTITY (1, 1) NOT NULL,
    [PersonId]            INT NOT NULL,
    [RegionalStructureId] INT NOT NULL,
    CONSTRAINT [PK_PersonTerritory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PersonTerritory_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PersonTerritory_RegionalStructure_RegionalStructureId] FOREIGN KEY ([RegionalStructureId]) REFERENCES [dbo].[RegionalStructure] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PersonTerritory_PersonId]
    ON [dbo].[PersonTerritory]([PersonId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonTerritory_RegionalStructureId]
    ON [dbo].[PersonTerritory]([RegionalStructureId] ASC);

