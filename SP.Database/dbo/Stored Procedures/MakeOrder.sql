CREATE PROCEDURE [dbo].[MakeOrder]
    @IdList nvarchar(max),	--  передается ограниченное количество идентификаторов
    @PersonId int,
	@OrderNum int OUTPUT
AS
BEGIN

	DECLARE @OrderId int;

	WITH IdList AS (
		SELECT x.i.value('(./text())[1]', 'varchar(10)') AS Id
		FROM (
			SELECT XMLList = CAST('<i>' + REPLACE(@IdList, ',', '</i><i>') + '</i>' AS XML).query('.')
			) a
		CROSS APPLY XMLList.nodes('i') x(i)
	)
	SELECT c.NomenclatureId, c.GasStationId, c.[Plan] - c.Quantity AS Amount
	INTO #OrderDetails
	FROM dbo.CalcSheet c
	JOIN IdList i ON c.Id = i.Id
	WHERE c.[Plan] != 0;

	IF (SELECT COUNT(*) FROM #OrderDetails) > 0
	BEGIN
		INSERT INTO dbo.[Order] (
			OrderDate,
			OrderType,
			PersonId
			)
		VALUES (
			GETDATE(),
			2,
			@PersonId
			);	
		SET @OrderId = SCOPE_IDENTITY();

		INSERT INTO dbo.OrderDetail (
			NomenclatureId,
			GasStationId,
			Quantity,
			OrderId
			)
		SELECT
			NomenclatureId,
			GasStationId,
			Amount,
			@OrderId
		FROM #OrderDetails;
	END

	SET @OrderNum = @OrderId;

END