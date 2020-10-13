CREATE TABLE [dbo].[GasStation] (
    [Id]                         INT             IDENTITY (1, 1) NOT NULL,
    [CodeKSSS]                   INT             NOT NULL,
    [CodeSAP]                    NVARCHAR (20)   NULL,
    [StationNumber]              NVARCHAR (5)    NULL,
    [TerritoryId]                INT             NOT NULL,
    [SettlementId]               INT             NOT NULL,
    [Address]                    NVARCHAR (200)  NULL,
    [StationLocationId]          INT             NOT NULL,
    [StationStatusId]            INT             NOT NULL,
    [ServiceLevelId]             INT             NOT NULL,
    [OperatorRoomFormatId]       INT             NOT NULL,
    [ManagementSystemId]         INT             NOT NULL,
    [TradingHallOperatingModeId] INT             NULL,
    [ClientRestroomId]           INT             NULL,
    [CashboxLocationId]          INT             NULL,
    [TradingHallSizeId]          INT             NULL,
    [CashboxTotal]               INT             NOT NULL,
    [PersonnelPerDay]            INT             NOT NULL,
    [FuelDispenserTotal]         INT             NOT NULL,
    [ClientRestroomTotal]        INT             NOT NULL,
    [TradingHallArea]            DECIMAL (8, 2)  NULL,
    [ChequePerDay]               DECIMAL (18, 2) NOT NULL,
    [RevenueAvg]                 DECIMAL (12, 2) NOT NULL,
    [HasSibilla]                 BIT             NOT NULL,
    [HasBakery]                  BIT             NOT NULL,
    [HasCakes]                   BIT             NOT NULL,
    [HasMarmite]                 BIT             NOT NULL,
    [HasKitchen]                 BIT             NOT NULL,
    [CoffeeMachineTotal]         INT             NOT NULL,
    [DishWashingMachineTotal]    INT             NOT NULL,
    [SettlememtId]               INT             NULL,
    [Code]                       NVARCHAR (20)   NULL,
    [HasJointRestroomEntrance]   BIT             DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [DeepFryTotal]               INT             DEFAULT ((0)) NOT NULL,
    [ChequeBandLengthPerDay]     DECIMAL (8, 2)  DEFAULT ((0.0)) NOT NULL,
    [RepresentativenessFactor]   DECIMAL (8, 2)  DEFAULT ((0.0)) NOT NULL,
    CONSTRAINT [PK_GasStation] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GasStation_CashboxLocation_CashboxLocationId] FOREIGN KEY ([CashboxLocationId]) REFERENCES [dic].[CashboxLocation] ([Id]),
    CONSTRAINT [FK_GasStation_ClientRestroom_ClientRestroomId] FOREIGN KEY ([ClientRestroomId]) REFERENCES [dic].[ClientRestroom] ([Id]),
    CONSTRAINT [FK_GasStation_ManagementSystem_ManagementSystemId] FOREIGN KEY ([ManagementSystemId]) REFERENCES [dic].[ManagementSystem] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GasStation_OperatorRoomFormat_OperatorRoomFormatId] FOREIGN KEY ([OperatorRoomFormatId]) REFERENCES [dic].[OperatorRoomFormat] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GasStation_RegionalStructure_TerritoryId] FOREIGN KEY ([TerritoryId]) REFERENCES [dbo].[RegionalStructure] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GasStation_ServiceLevel_ServiceLevelId] FOREIGN KEY ([ServiceLevelId]) REFERENCES [dic].[ServiceLevel] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GasStation_Settlement_SettlememtId] FOREIGN KEY ([SettlememtId]) REFERENCES [dic].[Settlement] ([Id]),
    CONSTRAINT [FK_GasStation_StationLocation_StationLocationId] FOREIGN KEY ([StationLocationId]) REFERENCES [dic].[StationLocation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GasStation_StationStatus_StationStatusId] FOREIGN KEY ([StationStatusId]) REFERENCES [dic].[StationStatus] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GasStation_TradingHallOperatingMode_TradingHallOperatingModeId] FOREIGN KEY ([TradingHallOperatingModeId]) REFERENCES [dic].[TradingHallOperatingMode] ([Id]),
    CONSTRAINT [FK_GasStation_TradingHallSize_TradingHallSizeId] FOREIGN KEY ([TradingHallSizeId]) REFERENCES [dic].[TradingHallSize] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_CashboxLocationId]
    ON [dbo].[GasStation]([CashboxLocationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_ClientRestroomId]
    ON [dbo].[GasStation]([ClientRestroomId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_ManagementSystemId]
    ON [dbo].[GasStation]([ManagementSystemId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_OperatorRoomFormatId]
    ON [dbo].[GasStation]([OperatorRoomFormatId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_ServiceLevelId]
    ON [dbo].[GasStation]([ServiceLevelId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_SettlememtId]
    ON [dbo].[GasStation]([SettlememtId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_StationLocationId]
    ON [dbo].[GasStation]([StationLocationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_StationStatusId]
    ON [dbo].[GasStation]([StationStatusId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_TerritoryId]
    ON [dbo].[GasStation]([TerritoryId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_TradingHallOperatingModeId]
    ON [dbo].[GasStation]([TradingHallOperatingModeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GasStation_TradingHallSizeId]
    ON [dbo].[GasStation]([TradingHallSizeId] ASC);

