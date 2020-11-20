CREATE OR REPLACE PROCEDURE "MakeFixedOrder"(id_list text, person_id int, order_num INOUT int)
LANGUAGE plpgsql
AS $$
DECLARE order_id int;

BEGIN

	CREATE TEMP TABLE order_details (
		"NomenclatureId" int,
		"GasStationId" int,
		"Amount" numeric(19,4)
		);

	INSERT INTO order_details
	SELECT c."NomenclatureId", c."GasStationId", c."Plan"
	FROM public."CalcSheet" c
	WHERE c."Id" = ANY(string_to_array(id_list, ',')::int[]) 
		AND c."Plan" != 0;

	IF (SELECT COUNT(*) FROM order_details) > 0 THEN
		SELECT MAX("Id")
		INTO order_id
		FROM public."Order";
		
		order_id := COALESCE(order_id, 0) + 1;
	
		INSERT INTO public."Order" (
			"Id",
			"OrderDate",
			"OrderType",
			"PersonId"
			)
		VALUES (
			order_id,
			current_timestamp,
			2,
			person_id
			);	

		INSERT INTO public."OrderDetail" (
			"NomenclatureId",
			"GasStationId",
			"Quantity",
			"OrderId"
			)
		SELECT
			"NomenclatureId",
			"GasStationId",
			"Amount",
			order_id
		FROM order_details;
	END IF;

	order_num := order_id;

END
$$