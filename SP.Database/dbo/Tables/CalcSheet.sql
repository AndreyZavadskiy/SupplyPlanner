CREATE TABLE [dbo].[CalcSheet] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [NomenclatureId] INT             NOT NULL,
    [GasStationId]   INT             NOT NULL,
    [Quantity]       DECIMAL (19, 4) NOT NULL,
    [FixedAmount]    DECIMAL (19, 4) NULL,
    [Formula]        NVARCHAR (MAX)  NULL,
    [MultipleFactor] DECIMAL (19, 4) NOT NULL,
    [Rounding]       INT             NOT NULL,
    [Plan]           DECIMAL (19, 4) NOT NULL,
    [LastUpdate]     DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_CalcSheet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CalcSheet_GasStation_GasStationId] FOREIGN KEY ([GasStationId]) REFERENCES [dbo].[GasStation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CalcSheet_Nomenclature_NomenclatureId] FOREIGN KEY ([NomenclatureId]) REFERENCES [dbo].[Nomenclature] ([Id]) ON DELETE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CalcSheet_GasStationId_NomenclatureId]
    ON [dbo].[CalcSheet]([GasStationId] ASC, [NomenclatureId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CalcSheet_NomenclatureId]
    ON [dbo].[CalcSheet]([NomenclatureId] ASC);

