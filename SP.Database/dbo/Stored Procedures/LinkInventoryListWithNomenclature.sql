﻿CREATE PROCEDURE [dbo].[LinkInventoryListWithNomenclature]
    @IdList nvarchar(max),  --  передается ограниченное количество идентификаторов
    @NomenclatureId int,
    @PersonId int,
    @Rows int OUTPUT
AS
BEGIN

    DECLARE @StatementRows int;

    WITH Id_CTE AS (
        SELECT x.i.value('(./text())[1]', 'varchar(10)') AS [Id]
        FROM (
            SELECT XMLList = CAST('<i>' + REPLACE(@IdList, ',', '</i><i>') + '</i>' AS XML).query('.')
        ) a
        CROSS APPLY XMLList.nodes('i') x(i)
    )
    UPDATE i
    SET i.NomenclatureId = @NomenclatureId
    FROM dbo.Inventory i
    JOIN Id_CTE c ON i.Id = c.Id;

    SET @StatementRows = @@ROWCOUNT;

    SET @Rows = @StatementRows;

END
