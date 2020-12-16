CREATE OR REPLACE PROCEDURE "LinkInventoryListWithNomenclature"(id_list text, nomenclature_id int, person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $procedure$
DECLARE
	id_array integer ARRAY;
    nomenclatureName TEXT;
   
BEGIN

    id_array := string_to_array(id_list, ',')::int[];

	SELECT "Name"
	INTO nomenclatureName
	FROM public."Nomenclature"
	WHERE "Id" = nomenclature_id;
   
    CREATE TEMP TABLE changes (
    	"Id" BIGINT,
    	"NomenclatureId" INTEGER
   	);
   
    INSERT INTO changes ("Id", "NomenclatureId")
    SELECT "Id", "NomenclatureId"
    FROM public."Inventory"
    WHERE "Id" = ANY(id_array);
    
    UPDATE public."Inventory" i
    SET "NomenclatureId" = nomenclature_id
    WHERE i."Id" = ANY(id_array);

    GET DIAGNOSTICS total_rows = ROW_COUNT;
   
	INSERT INTO log."Change" ("PersonId", "ChangeDate", "EntityName", "ActionName", "RecordId", "OldValue", "NewValue")
	SELECT person_id, current_timestamp, 'Inventory', 'ManualMerge', o."Id", 
		'Номенклатура: ' || o."NomenclatureId"::text || ', ' || n."Name",
		'Номенклатура: ' || nomenclature_id::text || ', ' || nomenclatureName 
	FROM changes o 
	JOIN public."Nomenclature" n ON o."NomenclatureId" = n."Id";
 
    DROP TABLE IF EXISTS changes;
   
END

$procedure$;
