CREATE OR REPLACE PROCEDURE "CalculateBalance"(stations text, nomenclatures text, person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$
DECLARE
    station_list int[];
    nomenclature_list int[];

BEGIN

    -- список АЗС
    station_list := string_to_array(stations, ',')::int[];

    -- список номенклатуры
    nomenclature_list := string_to_array(nomenclatures, ',')::int[];

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

    -- обновляем остатки в расчетах
    CREATE TEMP TABLE changes (
        "Id" BIGINT,
        "OldQuantity" NUMERIC(19, 4),
        "NewQuantity" NUMERIC(19, 4)
    );
    
    WITH "Balances" AS (
        SELECT c."GasStationId", c."NomenclatureId", SUM(i."Quantity") AS "TotalQuantity"
        FROM public."CalcSheet" c
        LEFT JOIN public."Inventory" i ON i."GasStationId" = c."GasStationId" AND i."NomenclatureId" = c."NomenclatureId"
        WHERE c."GasStationId" = ANY(station_list)
            AND c."NomenclatureId" = ANY(nomenclature_list)
        GROUP BY c."GasStationId", c."NomenclatureId"
    )
    INSERT INTO changes ("Id", "OldQuantity", "NewQuantity")
    SELECT c."Id", c."Quantity", COALESCE(b."TotalQuantity", 0)
    FROM public."CalcSheet" c
    JOIN "Balances" b ON c."GasStationId" = b."GasStationId" AND c."NomenclatureId" = b."NomenclatureId";
    
    UPDATE public."CalcSheet" c
    SET "Quantity" = t."NewQuantity",
        "LastUpdate" = current_timestamp
    FROM changes t 
    WHERE c."Id" = t."Id";

    GET DIAGNOSTICS total_rows = ROW_COUNT;

    INSERT INTO log."Change" ("PersonId", "ChangeDate", "EntityName", "ActionName", "RecordId", "OldValue", "NewValue")
    SELECT person_id, current_timestamp, 'CalcSheet', 'CalcBalance', "Id", 
        'Остаток: ' || "OldQuantity"::text,
        'Остаток: ' || "NewQuantity"::text 
    FROM changes 
    WHERE "OldQuantity" <> "NewQuantity";
 
    DROP TABLE IF EXISTS changes;

END

$$;