CREATE TABLE [dbo].[Inventory] (
    [Id]             BIGINT          IDENTITY (1, 1) NOT NULL,
    [Code]           NVARCHAR (450)  NULL,
    [Name]           NVARCHAR (100)  NULL,
    [GasStationId]   INT             NOT NULL,
    [MeasureUnitId]  INT             NOT NULL,
    [NomenclatureId] INT             NULL,
    [Quantity]       DECIMAL (19, 4) NOT NULL,
    [IsBlocked]      BIT             NOT NULL,
    [LastUpdate]     DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Inventory_GasStation_GasStationId] FOREIGN KEY ([GasStationId]) REFERENCES [dbo].[GasStation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Inventory_MeasureUnit_MeasureUnitId] FOREIGN KEY ([MeasureUnitId]) REFERENCES [dic].[MeasureUnit] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Inventory_Nomenclature_NomenclatureId] FOREIGN KEY ([NomenclatureId]) REFERENCES [dbo].[Nomenclature] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Inventory_MeasureUnitId]
    ON [dbo].[Inventory]([MeasureUnitId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Inventory_NomenclatureId]
    ON [dbo].[Inventory]([NomenclatureId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Inventory_GasStationId_Code]
    ON [dbo].[Inventory]([GasStationId] ASC, [Code] ASC) WHERE ([Code] IS NOT NULL);

