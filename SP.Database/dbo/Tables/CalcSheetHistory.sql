CREATE TABLE [dbo].[CalcSheetHistory] (
    [Id]             BIGINT          IDENTITY (1, 1) NOT NULL,
    [RecordId]       BIGINT          NOT NULL,
    [EffectiveDate]  DATETIME2 (7)   NOT NULL,
    [NomenclatureId] INT             NOT NULL,
    [GasStationId]   INT             NOT NULL,
    [Quantity]       DECIMAL (19, 4) NOT NULL,
    [FixedAmount]    DECIMAL (19, 4) NULL,
    [Formula]        NVARCHAR (MAX)  NULL,
    [MultipleFactor] DECIMAL (19, 4) NOT NULL,
    [Rounding]       INT             NOT NULL,
    [Plan]           DECIMAL (19, 4) NOT NULL,
    [LastUpdate]     DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_CalcSheetHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CalcSheetHistory_GasStation_GasStationId] FOREIGN KEY ([GasStationId]) REFERENCES [dbo].[GasStation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CalcSheetHistory_Nomenclature_NomenclatureId] FOREIGN KEY ([NomenclatureId]) REFERENCES [dbo].[Nomenclature] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CalcSheetHistory_GasStationId]
    ON [dbo].[CalcSheetHistory]([GasStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CalcSheetHistory_NomenclatureId]
    ON [dbo].[CalcSheetHistory]([NomenclatureId] ASC);

