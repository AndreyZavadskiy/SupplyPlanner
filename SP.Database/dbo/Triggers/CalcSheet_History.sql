CREATE TRIGGER [CalcSheet_History]
ON [dbo].[CalcSheet]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON
    IF (ROWCOUNT_BIG() = 0)
    RETURN;
    INSERT INTO dbo.CalcSheetHistory (
        [RecordId],
        [EffectiveDate],
        [NomenclatureId],
        [GasStationId],
        [Quantity],
        [FixedAmount],
        [Formula],
        [MultipleFactor],
        [Rounding],
        [Plan],
        [LastUpdate]
        )
    SELECT Id, LastUpdate, NomenclatureId, GasStationId, Quantity, FixedAmount, Formula, MultipleFactor, Rounding, [Plan], LastUpdate
    FROM inserted;
END
