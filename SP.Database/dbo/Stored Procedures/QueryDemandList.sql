CREATE PROCEDURE [dbo].[QueryDemandList]
	@Stations nvarchar(max),
	@Nomenclatures nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;

	WITH Station_CTE AS (
        SELECT x.i.value('(./text())[1]', 'varchar(10)') AS [GasStationId]
        FROM (
            SELECT XMLList = CAST('<i>' + REPLACE(@Stations, ',', '</i><i>') + '</i>' AS XML).query('.')
        ) a
        CROSS APPLY XMLList.nodes('i') x(i)
	),
	Nomenclature_CTE AS (
        SELECT x.i.value('(./text())[1]', 'varchar(10)') AS [NomenclatureId]
        FROM (
            SELECT XMLList = CAST('<i>' + REPLACE(@Nomenclatures, ',', '</i><i>') + '</i>' AS XML).query('.')
        ) a
        CROSS APPLY XMLList.nodes('i') x(i)
	),
    Sourse_CTE AS (
        SELECT c.*, 
            o.OrderDate, 
            od.Quantity AS [OrderQuantity],
            ROW_NUMBER() OVER (
                PARTITION BY c.Id
                ORDER BY o.OrderDate DESC
            ) AS row_num
        FROM dbo.CalcSheet c
        JOIN Station_CTE sc ON c.GasStationId = sc.GasStationId
        JOIN Nomenclature_CTE nc ON c.NomenclatureId = nc.NomenclatureId
        LEFT JOIN dbo.OrderDetail od ON od.GasStationId = sc.GasStationId AND od.NomenclatureId = nc.NomenclatureId
            AND od.Quantity != 0
        LEFT JOIN dbo.[Order] o ON od.OrderId = o.Id
    )
    SELECT 
        c.Id,
        ISNULL(n.Code, CAST(n.Id AS nvarchar(10))) AS [Code],
        n.[Name],
        s.StationNumber AS [GasStationName],
        c.Quantity,
        mu.[Name] AS MeasureUnitName,
        c.FixedAmount,
        c.Formula,
        c.[Plan],
        c.[Plan] - c.Quantity AS [OrderQuantity],
        c.OrderDate AS [LastOrderDate],
        c.OrderQuantity AS [LastQuantity]
    FROM Sourse_CTE c
    JOIN dbo.Nomenclature n ON c.NomenclatureId = n.Id
    JOIN dic.MeasureUnit mu ON n.MeasureUnitId = mu.Id
    JOIN dbo.GasStation s ON c.GasStationId = s.Id;

END