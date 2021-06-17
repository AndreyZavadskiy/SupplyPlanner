CREATE TRIGGER calc_sheet_update_trigger 
AFTER UPDATE ON public."CalcSheet"
FOR EACH ROW
WHEN (OLD.* IS DISTINCT FROM NEW.*)
EXECUTE PROCEDURE save_calc_sheet_history();
