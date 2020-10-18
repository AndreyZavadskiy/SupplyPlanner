CREATE PROCEDURE [dbo].[CalculateBalance]
	@Stations nvarchar(max),
	@Nomenclatures nvarchar(max),
	@PersonId int,
	@Rows int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	-- список АЗС
	WITH Station_CTE AS (
        SELECT x.i.value('(./text())[1]', 'varchar(10)') AS [GasStationId]
        FROM (
            SELECT XMLList = CAST('<i>' + REPLACE(@Stations, ',', '</i><i>') + '</i>' AS XML).query('.')
        ) a
        CROSS APPLY XMLList.nodes('i') x(i)
	)
	SELECT c.GasStationId
	INTO #stations
	FROM Station_CTE c
	WHERE EXISTS (
		SELECT * 
		FROM dbo.GasStation s
		WHERE s.Id = c.GasStationId
		);

	-- список номенклатуры
	WITH Nomenclature_CTE AS (
        SELECT x.i.value('(./text())[1]', 'varchar(10)') AS [NomenclatureId]
        FROM (
            SELECT XMLList = CAST('<i>' + REPLACE(@Nomenclatures, ',', '</i><i>') + '</i>' AS XML).query('.')
        ) a
        CROSS APPLY XMLList.nodes('i') x(i)
	)
	SELECT c.NomenclatureId
	INTO #nomenclatures
	FROM Nomenclature_CTE c
	WHERE EXISTS (
		SELECT *
		FROM dbo.Nomenclature n
		WHERE n.Id = c.NomenclatureId
		);

	-- добавляем номенклатуру, которой еще нет в расчетах
	INSERT INTO dbo.CalcSheet (
		[NomenclatureId],
		[GasStationId],
		[Quantity],
		[FixedAmount],
		[Formula],
		[MultipleFactor],
		[Rounding],
		[Plan],
		[LastUpdate])
	SELECT n.NomenclatureId, s.GasStationId, 0, NULL AS [FixedAmount], NULL AS [Formula],
		0, 0, 0, GETDATE()
	FROM #stations s
	CROSS JOIN #nomenclatures n
	WHERE NOT EXISTS (
		SELECT *
		FROM dbo.CalcSheet c
		WHERE c.GasStationId = s.GasStationId AND c.NomenclatureId = n.NomenclatureId
		);

	WITH Balance_CTE AS (
		SELECT c.GasStationId, c.NomenclatureId, SUM(i.Quantity) AS TotalQuantity
		FROM dbo.CalcSheet c
		JOIN #stations s ON c.GasStationId = s.GasStationId
		JOIN #nomenclatures n ON c.NomenclatureId = n.NomenclatureId
		LEFT JOIN dbo.Inventory i ON i.GasStationId = c.GasStationId AND i.NomenclatureId = c.NomenclatureId
		GROUP BY c.GasStationId, c.NomenclatureId
	)
	UPDATE c
	SET c.Quantity = ISNULL(b.TotalQuantity, 0),
		c.LastUpdate = GETDATE()
	FROM dbo.CalcSheet c
	JOIN Balance_CTE b ON c.GasStationId = b.GasStationId AND c.NomenclatureId = b.NomenclatureId;

	SET @Rows = @@ROWCOUNT;

	IF OBJECT_ID('tempdb..#stations') IS NOT NULL
		DROP TABLE #stations;
	IF OBJECT_ID('tempdb..#nomenclatures') IS NOT NULL
		DROP TABLE #nomenclatures;

END