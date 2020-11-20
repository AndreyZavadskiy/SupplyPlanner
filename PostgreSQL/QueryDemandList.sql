CREATE OR REPLACE FUNCTION "QueryDemandList"(stations text, nomenclatures text)
RETURNS table (
	"Id" bigint,
	"Code" varchar(20),
	"Name" varchar(100),
	"GasStationName" varchar(5),
	"Quantity" numeric(19,4),
	"MeasureUnitName" varchar(100),
	"FixedAmount" numeric(19,4),
	"Formula" text,
	"Plan" numeric(19,4),
	"OrderQuantity" numeric(19,4),
	"LastOrderDate" timestamp,
	"LastQuantity"  numeric(19,4)
	)
LANGUAGE plpgsql
AS $$

	WITH "Sourse" AS (
        SELECT c.*, 
            o."OrderDate", 
            od."Quantity" AS "OrderQuantity",
            ROW_NUMBER() OVER (
                PARTITION BY c."Id"
                ORDER BY o."OrderDate" DESC
            ) AS row_num
        FROM public."CalcSheet" c
        LEFT JOIN public."OrderDetail" od ON od."GasStationId" = c."GasStationId" AND od."NomenclatureId" = c."NomenclatureId"
            AND od."Quantity" != 0
        LEFT JOIN public."Order" o ON od."OrderId" = o."Id"
		WHERE c."GasStationId" = ANY(string_to_array(stations, ',')::int[])
			AND c."NomenclatureId" = ANY(string_to_array(nomenclatures, ',')::int[])
    )
    SELECT 
        c."Id",
        COALESCE(n."Code", CAST(n."Id" AS varchar(10))) AS "Code",
        n."Name",
        s."StationNumber" AS "GasStationName",
        c."Quantity",
        mu."Name" AS "MeasureUnitName",
        c."FixedAmount",
        c."Formula",
        c."Plan",
        c."Plan" - c."Quantity" AS "OrderQuantity",
        c."OrderDate" AS "LastOrderDate",
        c."OrderQuantity" AS "LastQuantity"
    FROM "Sourse" c
    JOIN public."Nomenclature" n ON c."NomenclatureId" = n."Id"
    JOIN dic."MeasureUnit" mu ON n."MeasureUnitId" = mu."Id"
    JOIN public."GasStation" s ON c."GasStationId" = s."Id";

$$;