CREATE TABLE [dbo].[Nomenclature] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [Code]                NVARCHAR (20)   NULL,
    [Name]                NVARCHAR (100)  NULL,
    [PetronicsCode]       NVARCHAR (20)   NULL,
    [PetronicsName]       NVARCHAR (100)  NULL,
    [MeasureUnitId]       INT             NOT NULL,
    [NomenclatureGroupId] INT             NOT NULL,
    [UsefulLife]          INT             NOT NULL,
    [IsActive]            BIT             NOT NULL,
    [Description]         NVARCHAR (2000) NULL,
    CONSTRAINT [PK_Nomenclature] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Nomenclature_MeasureUnit_MeasureUnitId] FOREIGN KEY ([MeasureUnitId]) REFERENCES [dic].[MeasureUnit] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Nomenclature_NomenclatureGroup_NomenclatureGroupId] FOREIGN KEY ([NomenclatureGroupId]) REFERENCES [dic].[NomenclatureGroup] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Nomenclature_MeasureUnitId]
    ON [dbo].[Nomenclature]([MeasureUnitId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Nomenclature_NomenclatureGroupId]
    ON [dbo].[Nomenclature]([NomenclatureGroupId] ASC);

