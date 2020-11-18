CREATE TABLE [dbo].[StageInventory] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [Code]          NVARCHAR (20)   NULL,
    [Name]          NVARCHAR (100)  NULL,
    [GasStationId]  INT             NOT NULL,
    [MeasureUnitId] INT             NOT NULL,
    [Quantity]      DECIMAL (19, 4) NOT NULL,
    [LastUpdate]    DATETIME2 (7)   NOT NULL,
    [PersonId]      INT             NOT NULL,
    CONSTRAINT [PK_StageInventory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StageInventory_GasStation_GasStationId] FOREIGN KEY ([GasStationId]) REFERENCES [dbo].[GasStation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StageInventory_MeasureUnit_MeasureUnitId] FOREIGN KEY ([MeasureUnitId]) REFERENCES [dic].[MeasureUnit] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StageInventory_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_StageInventory_GasStationId]
    ON [dbo].[StageInventory]([GasStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StageInventory_MeasureUnitId]
    ON [dbo].[StageInventory]([MeasureUnitId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StageInventory_PersonId]
    ON [dbo].[StageInventory]([PersonId] ASC);

