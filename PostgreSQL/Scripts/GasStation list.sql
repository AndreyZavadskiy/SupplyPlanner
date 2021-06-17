select 
	gs."Id" as "ID АЗС",
	"Code" as "Код",
	"CodeKSSS" as "Код КССС",
	"CodeSAP" as "Код SAP R/3",
	"StationNumber" as "Номер АЗС",
	rs."Name" as "ID территории",
	s."Name" as "ID населенного пункта",
	"Address" as "Адрес",
	sl."Name" as "Местоположение",
	ss."Name" as "Статус",
	sg."Name" as "Сегмент",
	lv."Name" as "Кластер (уровень сервиса)",
	orf."Name" as "Формат операторной",
	ms."Name" as "Система управления",
	om."Name" as "Режим работы торгового зала",
	cr."Name" as "Санузел для клиентов",
	cl."Name" as "Расчетно-кассовый узел",
	hs."Name" as "Размер торгового зала",
	rt."Name" as "Вид термоленты",
	"CashboxTotal" as "Количество АРМ/касс",
	"ManagerArmTotal" as "Количество АРМ менеджера",
	"PersonnelPerDay" as "Количество персонала в сутки",
	"FuelDispenserTotal" as "Количество ТРК",
	"FuelDispenserPostTotal" as "Количество постов ТРК",
	"FuelDispenserPostWithoutShedTotal" as "Количество постов ТРК без навеса",
	"ClientRestroomTotal" as "Количество сан.комнат для клиентов",
	"ClientTambourTotal" as "Количество тамбуров для клиентов",
	"ClientSinkTotal" as "Количество раковин для клиентов",
	"TradingHallArea" as "Площадь торгового зала",
	"ChequePerDay" as "Среднее количество чеков в сутки",
	"RevenueAvg" as "Выручка в месяц, руб.",
	"HasJointRestroomEntrance" as "Общий тамбур с раковиной",
	"HasSibilla" as "Сибилла",
	"HasBakery" as "Выпечка",
	"HasCakes" as "Торты",
	"DeepFryTotal" as "Количество фритюрных аппаратов",
	"HasMarmite" as "Мармит",
	"HasKitchen" as "Кухня",
	"CoffeeMachineTotal" as "Количество кофеаппаратов на жидком молоке",
	"DishWashingMachineTotal" as "Количество посудомоечных машин",
	"ChequeBandLengthPerDay" as "Расход кассовой ленты за день, м",
	"RepresentativenessFactor" as "Имиджевый коэффициент",
	"RepresentativenessFactor3Quarter" as "Имиджевый коэффициент 3 квартал",
	"MerrychefTotal" as "Количество комбипечей Меришеф",
	"DayCleaningTotal" as "Уборка в день",
	"NightCleaningTotal" as "Уборка в ночь",
	"DayRefuelingTotal" as "Расстановка заправки в день",
	"NightRefuelingTotal" as "Расстановка заправки в ночь",
	"HasFuelCardProgram" as "Участвует в проекте выдачи топливных карт"
from public."GasStation" gs 
left join public."RegionalStructure" rs on gs."TerritoryId" = rs."Id" 
left join dic."Settlement" s on gs."SegmentId" = s."Id" 
left join dic."StationLocation" sl on gs."StationLocationId" = sl."Id" 
left join dic."StationStatus" ss on gs."StationStatusId" = ss."Id" 
left join dic."Segment" sg on gs."SegmentId" = sg."Id" 
left join dic."ServiceLevel" lv on gs."ServiceLevelId" = lv."Id" 
left join dic."OperatorRoomFormat" orf on gs."OperatorRoomFormatId" = orf."Id" 
left join dic."ManagementSystem" ms on gs."ManagementSystemId" = ms."Id" 
left join dic."TradingHallOperatingMode" om on gs."TradingHallOperatingModeId" = om."Id" 
left join dic."ClientRestroom" cr on gs."ClientRestroomId" = cr."Id" 
left join dic."CashboxLocation" cl on gs."CashboxLocationId" = cl."Id" 
left join dic."TradingHallSize" hs on gs."TradingHallSizeId" = hs."Id" 
left join dic."CashRegisterTape" rt on gs."CashRegisterTapeId" = rt."Id";
