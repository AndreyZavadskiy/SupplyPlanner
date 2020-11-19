CREATE OR REPLACE PROCEDURE "BlockInventoryList"(person_id int, id_list text, nomenclature_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$
BEGIN

    UPDATE dbo."Inventory"
    SET "IsBlocked" = 1,
        "NomenclatureId" = NULL
    WHERE i."Id" = ANY(string_to_array(id_list, ',')::int[]); 

    GET DIAGNOSTICS total_rows = ROW_COUNT;

END 

$$;