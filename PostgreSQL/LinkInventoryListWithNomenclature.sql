CREATE OR REPLACE PROCEDURE "LinkInventoryListWithNomenclature"(id_list text, nomenclature_id int, person_id int, total_rows INOUT int)
LANGUAGE plpgsql
AS $$
BEGIN

    UPDATE "Inventory" i
    SET "NomenclatureId" = nomenclature_id
    WHERE i."Id" = ANY(string_to_array(id_list, ',')::int[]);

    GET DIAGNOSTICS total_rows = ROW_COUNT;
    
END

$$;