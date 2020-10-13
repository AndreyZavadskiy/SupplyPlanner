CREATE TABLE [dic].[TradingHallOperatingMode] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (100) NOT NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_TradingHallOperatingMode] PRIMARY KEY CLUSTERED ([Id] ASC)
);

