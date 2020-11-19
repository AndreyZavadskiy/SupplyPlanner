CREATE OR REPLACE PROCEDURE "MergeStageToInventory"(person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$

DECLARE statement_rows int;

BEGIN
	
	total_rows := 0;

	UPDATE public."Inventory" ti
	SET "Quantity" = coalesce(s."Quantity", 0),
		"LastUpdate" = current_timestamp
	FROM public."Inventory" i
	LEFT JOIN public."StageInventory" s
		ON i."GasStationId" = s."GasStationId"
			AND i."Code" = s."Code"
			AND s."PersonId" = 1
	WHERE ti."Id" = i."Id";

	GET DIAGNOSTICS statement_rows = ROW_COUNT;
	total_rows := total_rows + statement_rows;

	WITH NewInventory AS (
		SELECT *
		FROM public."StageInventory" s
		WHERE s."PersonId" = person_id
			AND NOT EXISTS (
				SELECT *
				FROM public."Inventory" i
				WHERE i."GasStationId" = s."GasStationId" AND i."Code" = s."Code"
			)
	)
	INSERT INTO public."Inventory" (
		"Code",
		"Name",
		"GasStationId",
		"Quantity",
		"IsBlocked",
		"LastUpdate",
		"MeasureUnitId"
		)
	SELECT
		"Code",
		"Name",
		"GasStationId",
		"Quantity",
		false,
		current_timestamp,
		"MeasureUnitId"
	FROM NewInventory;

	GET DIAGNOSTICS statement_rows = ROW_COUNT;
	total_rows := total_rows + statement_rows;
	
END

$$;