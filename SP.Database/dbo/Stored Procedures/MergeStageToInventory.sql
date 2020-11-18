CREATE PROCEDURE [dbo].[MergeStageToInventory]
    @PersonId int,
	@Rows int OUTPUT
AS
BEGIN

	DECLARE @StatementRows int;

	SET @Rows = 0;

	UPDATE i
	SET i.Quantity = ISNULL(s.Quantity, 0),
		i.LastUpdate = GETDATE()
	FROM dbo.Inventory i
	LEFT JOIN dbo.StageInventory s
		ON i.GasStationId = s.GasStationId 
			AND i.Code = s.Code
			AND s.PersonId = @PersonId;

	SET @StatementRows = @@ROWCOUNT;
	SET @Rows += @StatementRows;

	WITH NewInventory AS (
		SELECT *
		FROM dbo.StageInventory s
		WHERE s.PersonId = @PersonId
			AND NOT EXISTS (
				SELECT *
				FROM dbo.Inventory i
				WHERE i.GasStationId = s.GasStationId AND i.Code = s.Code
			)
	)
	INSERT INTO dbo.Inventory (
		[Code],
		[Name],
		[GasStationId],
		[Quantity],
		[IsBlocked],
		[LastUpdate],
		[MeasureUnitId]
		)
	SELECT
		[Code],
		[Name],
		[GasStationId],
		[Quantity],
		0,
		GETDATE(),
		[MeasureUnitId]
	FROM NewInventory;

	SET @StatementRows = @@ROWCOUNT;
	SET @Rows += @StatementRows;

END
