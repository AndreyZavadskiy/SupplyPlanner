CREATE OR REPLACE FUNCTION save_calc_sheet_history() RETURNS TRIGGER 
LANGUAGE plpgsql
AS $$
BEGIN
	INSERT INTO public."CalcSheetHistory"
	("RecordId", "EffectiveDate", "NomenclatureId", "GasStationId", "Quantity", "FixedAmount", "Formula", "MultipleFactor", "Rounding", "Plan", "LastUpdate")
	VALUES(
		NEW."Id",
		NEW."LastUpdate",
		NEW."NomenclatureId",
		NEW."GasStationId",
		NEW."Quantity",
		NEW."FixedAmount",
		NEW."Formula",
		NEW."MultipleFactor",
		NEW."Rounding",
		NEW."Plan",
		NEW."LastUpdate"
		);

	RETURN NEW;
END;
$$;
