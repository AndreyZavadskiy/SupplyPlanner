using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SP.Core.Master;

namespace SP.Core.Model
{
    /// <summary>
    /// АЗС
    /// </summary>
    public class GasStation
    {
        public int Id { get; set; }
        [StringLength(20)]
        public string Code { get; set; }
        public int CodeKSSS { get; set; }
        [StringLength(20)]
        public string CodeSAP { get; set; }
        [StringLength(5)]
        public string StationNumber { get; set; }
        public int TerritoryId { get; set; }
        public int SettlementId { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        public int StationLocationId { get; set; }
        public int StationStatusId { get; set; }
        public int ServiceLevelId { get; set; }
        public int OperatorRoomFormatId { get; set; }
        public int ManagementSystemId { get; set; }
        public int TradingHallOperatingModeId { get; set; }
        public int ClientRestroomId { get; set; }
        public int CashboxLocationId { get; set; }
        public int TradingHallSizeId { get; set; }
        public int CashboxTotal { get; set; }
        public int PersonnelPerDay { get; set; }
        public int FuelDispenserTotal { get; set; }
        public int ClientRestroomTotal { get; set; }
        [Column(TypeName = "decimal(8,2)")]
        public decimal TradingHallArea { get; set; }
        public int ChequePerDay { get; set; }
        [Column(TypeName = "decimal(12,2)")]
        public decimal RevenueAvg { get; set; }
        public bool HasJointRestroomEntrance { get; set; }
        public bool HasSibilla { get; set; }
        public bool HasBakery { get; set; }
        public bool HasCakes { get; set; }
        public bool HasFrenchFry { get; set; }
        public bool HasMarmite { get; set; }
        public bool HasKitchen { get; set; }
        public int CoffeeMachineTotal { get; set; }
        public int DishWashingMachineTotal { get; set; }

        #region Navigation properties

        [ForeignKey("TerritoryId")]
        public RegionalStructure Territory { get; set; }
        [ForeignKey("SettlememtId")]
        public Settlement Settlement { get; set; }
        [ForeignKey("StationLocationId")]
        public StationLocation StationLocation { get; set; }
        [ForeignKey("StationStatusId")]
        public StationStatus StationStatus { get; set; }
        [ForeignKey("ServiceLevelId")]
        public ServiceLevel ServiceLevel { get; set; }
        [ForeignKey("OperatorRoomFormatId")]
        public OperatorRoomFormat OperatorRoomFormat { get; set; }
        [ForeignKey("ManagementSystemId")]
        public ManagementSystem ManagementSystem { get; set; }
        [ForeignKey("TradingHallOperatingModeId")]
        public TradingHallOperatingMode TradingHallOperatingMode { get; set; }
        [ForeignKey("ClientRestroomId")]
        public ClientRestroom ClientRestroom { get; set; }
        [ForeignKey("CashboxLocationId")]
        public CashboxLocation CashboxLocation { get; set; }
        [ForeignKey("TradingHallSizeId")]
        public TradingHallSize TradingHallSize { get; set; }

        #endregion
    }
}
