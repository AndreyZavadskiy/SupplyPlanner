CREATE TRIGGER calc_sheet_insert_trigger 
AFTER INSERT ON public."CalcSheet"
FOR EACH ROW
EXECUTE PROCEDURE save_calc_sheet_history();
