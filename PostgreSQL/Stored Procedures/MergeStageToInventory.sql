CREATE OR REPLACE PROCEDURE "MergeStageToInventory"(person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$

DECLARE statement_rows int;

BEGIN
    
    total_rows := 0;

    CREATE TEMP TABLE updates (
        "Id" BIGINT,
        "OldQuantity" NUMERIC(19, 4),
        "NewQuantity" NUMERIC(19, 4)
    );

    -- сохраняем старые и новые значения
    INSERT INTO updates ("Id", "OldQuantity", "NewQuantity")
    SELECT i."Id", i."Quantity", coalesce(s."Quantity", 0)
    FROM public."Inventory" i
    LEFT JOIN public."StageInventory" s
    ON i."GasStationId" = s."GasStationId"
        AND i."Code" = s."Code"
        AND s."PersonId" = person_id;

    -- перезаписываем количество и ставим новую дату изменения
    UPDATE public."Inventory" ti
    SET "Quantity" = u."NewQuantity",
        "LastUpdate" = current_timestamp
    FROM updates u
    WHERE ti."Id" = u."Id";

    -- логируем только изменения
    INSERT INTO log."Change" ("PersonId", "ChangeDate", "EntityName", "ActionName", "RecordId", "OldValue", "NewValue")
    SELECT person_id, current_timestamp, 'Inventory', 'Upload', "Id", 
        'Остаток: ' || "OldQuantity"::text, 
        'Остаток: ' || "NewQuantity"::text
    FROM updates
    WHERE "OldQuantity" <> "NewQuantity";

    GET DIAGNOSTICS statement_rows = ROW_COUNT;
    total_rows := total_rows + statement_rows;

    DROP TABLE IF EXISTS updates;

    CREATE TEMP TABLE inserts (
        "Code" CHARACTER VARYING(20),
        "Name" CHARACTER VARYING(100),
        "GasStationId" INTEGER NOT NULL,
        "Quantity" NUMERIC(19, 4) NOT NULL,
        "MeasureUnitId" INTEGER NOT NULL
    );

    -- новые записи
    INSERT INTO inserts (
        "Code",
        "Name",
        "GasStationId",
        "Quantity",
        "MeasureUnitId"
    )
    SELECT "Code", "Name", "GasStationId", "Quantity", "MeasureUnitId" 
    FROM public."StageInventory" s
    WHERE s."PersonId" = person_id
        AND NOT EXISTS (
            SELECT *
            FROM public."Inventory" i
            WHERE i."GasStationId" = s."GasStationId" AND i."Code" = s."Code"
        );

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
    FROM inserts;

    INSERT INTO log."Change" ("PersonId", "ChangeDate", "EntityName", "ActionName", "RecordId", "OldValue", "NewValue")
    SELECT person_id, current_timestamp, 'Inventory', 'Upload', i."Id", 
        null, 
        'Остаток: ' || n."Quantity"::text
    FROM inserts n
    JOIN public."Inventory" i ON n."GasStationId" = i."GasStationId" AND n."Code" = i."Code";

    GET DIAGNOSTICS statement_rows = ROW_COUNT;
    total_rows := total_rows + statement_rows;

    DROP TABLE IF EXISTS inserts;
    
END

$$;