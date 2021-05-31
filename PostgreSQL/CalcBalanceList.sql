CREATE OR REPLACE PROCEDURE "CalcBalanceList"(id_list text, person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$
DECLARE
    id_array int[];

BEGIN

    id_array := string_to_array(id_list, ',')::int[];

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
        WHERE c."Id" = ANY(id_array)
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