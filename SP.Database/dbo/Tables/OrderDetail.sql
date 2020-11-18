CREATE TABLE [dbo].[OrderDetail] (
    [Id]             BIGINT          IDENTITY (1, 1) NOT NULL,
    [OrderId]        BIGINT          NOT NULL,
    [NomenclatureId] INT             NOT NULL,
    [GasStationId]   INT             NOT NULL,
    [Quantity]       DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderDetail_GasStation_GasStationId] FOREIGN KEY ([GasStationId]) REFERENCES [dbo].[GasStation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderDetail_Nomenclature_NomenclatureId] FOREIGN KEY ([NomenclatureId]) REFERENCES [dbo].[Nomenclature] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderDetail_Order_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_OrderDetail_GasStationId]
    ON [dbo].[OrderDetail]([GasStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OrderDetail_NomenclatureId]
    ON [dbo].[OrderDetail]([NomenclatureId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OrderDetail_OrderId]
    ON [dbo].[OrderDetail]([OrderId] ASC);

