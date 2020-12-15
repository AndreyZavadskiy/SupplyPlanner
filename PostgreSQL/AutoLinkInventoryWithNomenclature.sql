CREATE OR REPLACE PROCEDURE "AutoLinkInventoryWithNomenclature"(person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$
DECLARE statement_rows int;

BEGIN
    total_rows := 0;
	
    CREATE TEMP TABLE equal_names (
    	"InventoryId" BIGINT,
    	"NomenclatureId" INTEGER
    );
   
    -- стыкуем по точному совпадению названия
	INSERT INTO equal_names ("InventoryId", "NomenclatureId") 
    SELECT i."Id", n."Id"
    FROM public."Inventory" i
    JOIN public."Nomenclature" n ON i."Name" = n."PetronicsName"
    WHERE i."NomenclatureId" IS NULL
        AND i."IsBlocked" = false;
   
    UPDATE public."Inventory" i
    SET "NomenclatureId" = c."NomenclatureId",
        "LastUpdate" = current_timestamp
    FROM equal_names c 
    WHERE i."Id" = c."InventoryId";

    GET DIAGNOSTICS statement_rows = ROW_COUNT;
    total_rows := total_rows + statement_rows;

	INSERT INTO log."Change" ("PersonId", "ChangeDate", "EntityName", "ActionName", "RecordId", "OldValue", "NewValue")
	SELECT person_id, current_timestamp, 'Inventory', 'AutoMerge', "InventoryId", 
		null, 
		'Номенклатура: ' || "NomenclatureId"::text
	FROM equal_names;
    
	DROP TABLE IF EXISTS equal_names;

    -- стыкуем по ранее привязанным кодам
    CREATE TEMP TABLE equal_code (
    	"InventoryId" BIGINT,
    	"NomenclatureId" INTEGER
    );

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
    INSERT INTO equal_codes ("InventoryId", "NomenclatureId")
	SELECT i."Id", c."NomenclatureId"
    FROM public."Inventory" i
    JOIN "SingleLinkedInventory" c ON i."Code" = c."Code"
    WHERE i."NomenclatureId" IS NULL;
    
    UPDATE public."Inventory" i
    SET "NomenclatureId" = c."NomenclatureId",
        "LastUpdate" = current_timestamp
    FROM equal_codes c 
    WHERE i."Id" = c."InventoryId";

    GET DIAGNOSTICS statement_rows = ROW_COUNT;
    total_rows := total_rows + statement_rows;
   
	INSERT INTO log."Change" ("PersonId", "ChangeDate", "EntityName", "ActionName", "RecordId", "OldValue", "NewValue")
	SELECT person_id, current_timestamp, 'Inventory', 'AutoMerge', "InventoryId", 
		null, 
		'Номенклатура: ' || "NomenclatureId"::text
	FROM equal_codes;

END

$$;