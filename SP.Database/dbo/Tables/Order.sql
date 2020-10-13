CREATE TABLE [dbo].[Order] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [OrderDate] DATETIME2 (7) NOT NULL,
    [PersonId]  INT           NOT NULL,
    [OrderType] INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Order_Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Order_PersonId]
    ON [dbo].[Order]([PersonId] ASC);

