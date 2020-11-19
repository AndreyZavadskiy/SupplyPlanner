CREATE OR REPLACE PROCEDURE "AutoLinkInventoryWithNomenclature"(person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$
DECLARE statement_rows int;

BEGIN
    total_rows := 0;

    -- стыкуем по точному совпадению названия
    WITH "EqualName" AS (
        SELECT i."Id" AS "InventoryId", 
            n."Id" AS "NomenclatureId"
        FROM public."Inventory" i
        JOIN public."Nomenclature" n ON i."Name" = n."PetronicsName"
        WHERE i."NomenclatureId" IS NULL
            AND i."IsBlocked" = false
    )
    UPDATE public."Inventory" i
    SET "NomenclatureId" = c."NomenclatureId",
        "LastUpdate" = current_timestamp
    FROM "EqualName" c 
	WHERE i."Id" = c."InventoryId";

    GET DIAGNOSTICS statement_rows = ROW_COUNT;
    total_rows := total_rows + statement_rows;

    -- стыкуем по ранее привязанным кодам
    WITH "SingleCodes" AS (
        SELECT "Code"
        FROM public."Inventory"
        WHERE "NomenclatureId" IS NOT NULL
            AND "IsBlocked" = false
        GROUP BY "Code", "NomenclatureId"
        HAVING COUNT("NomenclatureId") = 1
    ),
    "SingleLinkedInventory" AS (
        SELECT "Code", "NomenclatureId"
        FROM public."Inventory"
        WHERE "Code" IN (SELECT "Code" FROM "SingleCodes")
    )
    UPDATE public."Inventory" i
    SET "NomenclatureId" = c."NomenclatureId"
    FROM "SingleLinkedInventory" c 
	WHERE i."Code" = c."Code"
		AND i."NomenclatureId" IS NULL;

    GET DIAGNOSTICS statement_rows = ROW_COUNT;
    total_rows := total_rows + statement_rows;

END

$$;