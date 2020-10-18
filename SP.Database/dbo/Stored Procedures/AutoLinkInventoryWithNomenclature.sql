CREATE PROCEDURE [dbo].[AutoLinkInventoryWithNomenclature]
    @PersonId int,
    @Rows int OUTPUT
AS
BEGIN

    DECLARE @StatementRows int;

    SET @Rows = 0;

    -- стыкуем по точному совпадению названия
    WITH EqualName AS (
        SELECT i.Id AS InventoryId, 
            n.Id AS NomenclatureId
        FROM dbo.Inventory i
        JOIN dbo.Nomenclature n ON i.[Name] = n.[PetronicsName]
        WHERE i.NomenclatureId IS NULL
            AND i.IsBlocked = 0
    )
    UPDATE i
    SET i.NomenclatureId = c.NomenclatureId,
        i.LastUpdate = GETDATE()
    FROM dbo.Inventory i
    JOIN EqualName c ON i.Id = c.InventoryId;

    SET @StatementRows = @@ROWCOUNT;
    SET @Rows += @StatementRows;

    -- стыкуем по ранее привязанным кодам
    WITH SingleCodes AS (
        SELECT Code
        FROM dbo.Inventory
        WHERE NomenclatureId IS NOT NULL
            AND IsBlocked = 0
        GROUP BY Code, NomenclatureId
        HAVING COUNT(NomenclatureId) = 1
    ),
    SingleLinkedInventory AS (
        SELECT Code, NomenclatureId
        FROM dbo.Inventory
        WHERE Code IN (SELECT Code FROM SingleCodes)
    )
    UPDATE i
    SET i.NomenclatureId = c.NomenclatureId
    FROM dbo.Inventory i
    JOIN SingleLinkedInventory c ON i.Code = c.Code
    WHERE i.NomenclatureId IS NULL;

    SET @StatementRows = @@ROWCOUNT;
    SET @Rows += @StatementRows;

END