CREATE TABLE [stage].[Inventory] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Code]          NVARCHAR (20)   NULL,
    [Name]          NVARCHAR (100)  NULL,
    [GasStationId]  INT             NOT NULL,
    [Quantity]      DECIMAL (19, 4) NOT NULL,
    [LastUpdate]    DATETIME2 (7)   NOT NULL,
    [MeasureUnitId] INT             DEFAULT ((0)) NOT NULL,
    [PersonId]      INT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Inventory_GasStation_GasStationId] FOREIGN KEY ([GasStationId]) REFERENCES [dbo].[GasStation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Inventory_MeasureUnit_MeasureUnitId] FOREIGN KEY ([MeasureUnitId]) REFERENCES [dic].[MeasureUnit] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Inventory_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Inventory_GasStationId]
    ON [stage].[Inventory]([GasStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Inventory_MeasureUnitId]
    ON [stage].[Inventory]([MeasureUnitId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Inventory_PersonId]
    ON [stage].[Inventory]([PersonId] ASC);

