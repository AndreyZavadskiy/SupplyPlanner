CREATE OR REPLACE PROCEDURE "BlockInventoryList"(id_list text, person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $procedure$
DECLARE
    id_array integer ARRAY;

BEGIN

    id_array := string_to_array(id_list, ',')::int[];

    CREATE TEMP TABLE changes (
        "Id" BIGINT,
        "IsBlocked" BOOL
    );
   
    INSERT INTO changes ("Id", "IsBlocked")
    SELECT "Id", "IsBlocked"
    FROM public."Inventory"
    WHERE "Id" = ANY(id_array);
   
    UPDATE public."Inventory"
    SET "IsBlocked" = true,
        "NomenclatureId" = NULL
    WHERE "Id" = ANY(id_array); 

    GET DIAGNOSTICS total_rows = ROW_COUNT;

    INSERT INTO log."Change" ("PersonId", "ChangeDate", "EntityName", "ActionName", "RecordId", "OldValue", "NewValue")
    SELECT person_id, current_timestamp, 'Inventory', 'ManualMerge', o."Id", 
        CASE WHEN o."IsBlocked" = true THEN 'Исключена' ELSE 'Активная' END,
        'Исключена' 
    FROM changes o;
   
    DROP TABLE IF EXISTS changes;

END 

$procedure$;