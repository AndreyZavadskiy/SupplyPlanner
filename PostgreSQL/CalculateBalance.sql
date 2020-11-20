CREATE OR REPLACE PROCEDURE "CalculateBalance"(stations text, nomenclatures text, person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$
DECLARE
	station_list int[];
	nomenclature_list int[];

BEGIN

	-- список АЗС
	SELECT string_to_array(stations, ',')::int[]
	INTO station_list;

	-- список номенклатуры
	SELECT string_to_array(nomenclatures, ',')::int[]	
	INTO nomenclature_list;

	-- добавляем номенклатуру, которой еще нет в расчетах
	WITH "StationList" AS (
		SELECT unnest(station_list) AS "GasStationId"
	),
	"NomenclatureList" AS (
		SELECT unnest(nomenclature_list) AS "NomenclatureId"
	)
	INSERT INTO "CalcSheet" (
		"NomenclatureId",
		"GasStationId",
		"Quantity",
		"FixedAmount",
		"Formula",
		"MultipleFactor",
		"Rounding",
		"Plan",
		"LastUpdate")
	SELECT n."NomenclatureId", s."GasStationId", 0, NULL, NULL, 0, 0, 0, current_timestamp
	FROM "StationList" s
	CROSS JOIN "NomenclatureList" n
	WHERE NOT EXISTS (
		SELECT *
		FROM public."CalcSheet" c
		WHERE c."GasStationId" = s."GasStationId" AND c."NomenclatureId" = n."NomenclatureId"
		);

	WITH "Balances" AS (
		SELECT c."GasStationId", c."NomenclatureId", SUM(i."Quantity") AS "TotalQuantity"
		FROM public."CalcSheet" c
		LEFT JOIN public."Inventory" i ON i."GasStationId" = c."GasStationId" AND i."NomenclatureId" = c."NomenclatureId"
		WHERE c."GasStationId" = ANY(station_list)
			AND c."NomenclatureId" = ANY(nomenclature_list)
		GROUP BY c."GasStationId", c."NomenclatureId"
	)
	UPDATE public."CalcSheet" c
	SET "Quantity" = COALESCE(b."TotalQuantity", 0),
		"LastUpdate" = current_timestamp
	FROM "Balances" b 
	WHERE c."GasStationId" = b."GasStationId" AND c."NomenclatureId" = b."NomenclatureId";

	GET DIAGNOSTICS total_rows = ROW_COUNT;

END

$$;