CREATE TABLE [dbo].[Person] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [AspNetUserId] NVARCHAR (450) NOT NULL,
    [LastName]     NVARCHAR (50)  NOT NULL,
    [FirstName]    NVARCHAR (50)  NOT NULL,
    [MiddleName]   NVARCHAR (50)  NULL,
    [Code]         NVARCHAR (20)  NULL,
    CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Person_AspNetUsers_AspNetUserId] FOREIGN KEY ([AspNetUserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Person_AspNetUserId]
    ON [dbo].[Person]([AspNetUserId] ASC);

