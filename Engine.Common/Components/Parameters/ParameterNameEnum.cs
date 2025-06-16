using System.ComponentModel;
using Engine.Common.Binders;

namespace Engine.Common.Components.Parameters;

[EnumType("ParameterName")]
public enum ParameterNameEnum {
	[Description("None")] None = 0,
	[Description("Health\t(Жизни)")] Health = 1,
	[Description("Immunity\t(Иммунитет)")] Immunity = 2,
	[Description("Hunger\t(Голод)")] Hunger = 3,
	[Description("Infection\t(Инфекция)")] Infection = 4,

	[Description("Stamina\t(Выносливость)")]
	Stamina = 5,
	[Description("Thirst\t(Жажда)")] Thirst = 6,
	[Description("Fatigue\t(Усталость)")] Fatigue = 7,

	[Description("Test\t(Для тестирования)")]
	Test = 8,
	[Description("Collected\t(Собрано)")] Collected = 9,

	[Description("NOT_USED_CollectedTime\t(Время сбора, не используется)")]
	NOT_USED_CollectedTime = 10,

	[Description("Reputation\t(Репутация)")]
	Reputation = 11,

	[Description("HearingDistance\t(Дистанция слуха)")]
	HearingDistance = 12,

	[Description("EyeDistance\t(Дистанция зрения)")]
	EyeDistance = 13,

	[Description("NoiseDistance\t(Дистанция шума)")]
	NoiseDistance = 14,

	[Description("VisibleDistance\t(Дистанция видимости)")]
	VisibleDistance = 15,
	[Description("Indoor\t(В доме)")] Indoor = 16,
	[Description("Outdoor\t(На улице)")] Outdoor = 17,
	[Description("Rain\t(Дождь)")] Rain = 18,
	[Description("Wet\t(Мокрый)")] Wet = 19,
	[Description("Stealth\t(Скрытный)")] Stealth = 20,

	[Description("EyeAngle\t(Угол зрения)")]
	EyeAngle = 21,

	[Description("VisibleDetectType\t(Тип видимости)")]
	VisibleDetectType = 22,

	[Description("NoiseDetectType\t(Тип шума)")]
	NoiseDetectType = 23,
	[Description("Dead\t(Смерть)")] Dead = 24,

	[Description("BallisticDamage\t(Баллистический урон)")]
	BallisticDamage = 25,

	[Description("FireDamage\t(Урон от огня)")]
	FireDamage = 26,

	[Description("InfectionDamage\t(Урон от инфекции)")]
	InfectionDamage = 27,
	[Description("Disease\t(Заражение)")] Disease = 28,

	[Description("NOT_USED_Thug\t(Бандит, не используется)")]
	NOT_USED_Thug = 29,

	[Description("CraftTime\t(Время крафта)")]
	CraftTime = 30,
	[Description("Sleep\t(Сон)")] Sleep = 31,

	[Description("Away\t(Далеко от персонажа)")]
	Away = 32,

	[Description("MeleeDamage\t(Урон от холодного оружия)")]
	MeleeDamage = 33,

	[Description("Immortal\t(Бессмертный)")]
	Immortal = 34,

	[Description("CanTrade\t(Способность торговать)")]
	CanTrade = 35,
	[Description("Visir")] Visir = 36,
	[Description("Walk")] Walk = 37,
	[Description("Run")] Run = 38,
	[Description("Flashlight")] Flashlight = 39,

	[Description("CanAutopsy\t(Способность вскрытия)")]
	CanAutopsy = 40,

	[Description("CanHeal\t(Способность лечения)")]
	CanHeal = 41,

	[Description("PreInfection\t(Опухоль)")]
	PreInfection = 42,

	[Description("LowStamina\t(Низкая стамина)")]
	LowStamina = 43,

	[Description("ForceTrade\t(Принудительная торговля)")]
	ForceTrade = 44,

	[Description("Available\t(Доступность)")]
	Available = 45,
	[Description("Enabled\t(Активность)")] Enabled = 46,

	[Description("OpenState\t(Состояние контейнера)")]
	OpenState = 47,
	[Description("Opened\t(Открытость)")] Opened = 48,

	[Description("LockState\t(Состояние двери)")]
	LockState = 49,

	[Description("Bolted\t(Заблокирована)")]
	Bolted = 50,
	[Description("Marked\t(Помечена)")] Marked = 51,

	[Description("SendEnterWithoutKnock\t(Херня какая то)")]
	SendEnterWithoutKnock = 52,
	[Description("IsFree\t(Ничейный)")] IsFree = 53,

	[Description("DiseaseLevel\t(Уровень заражения)")]
	DiseaseLevel = 54,

	[Description("CanBeMarked\t(Способность двери быть отмеченной крестом)")]
	CanBeMarked = 55,

	[Description("FallDamage\t(Урон от падения)")]
	FallDamage = 56,

	[Description("Durability\t(Прочность)")]
	Durability = 100,

	[Description("Customization\t(Кастомизация)")]
	Customization = 1001,
	[Description("Model\t(Модель)")] Model = 1002,
	[Description("Pain\t(Боль)")] Pain = 1003,

	[Description("WeaponKind\t(Тип оружия)")]
	WeaponKind = 1004,
	[Description("Block\t(Блок)")] Block = 1005,

	[Description("BlockDisabled\t(Блок отключен)")]
	BlockDisabled = 1006,

	[Description("MovementControlBlock\t(Блок самостоятельного передвижения)")]
	MovementControlBlock = 1007,
	[Description("WalkSpeedModifier\t()")] WalkSpeedModifier = 1008,

	[Description("CanFight\t(Может драться)")]
	CanFight = 1009,
	[Description("Surrender\t(Сдаётся)")] Surrender = 1010,

	[Description("WasAttackedByPlayer\t(Уже был атакован игроком)")]
	WasAttackedByPlayer = 1011,

	[Description("StammKind\t(Тип штамма болезни)")]
	StammKind = 1012,

	[Description("IsFighting\t(сражается прямо сейчас)")]
	IsFighting = 1013,
	[Description("IsBurning\t(горит)")] IsBurning = 1014,

	[Description("IsCombatIgnored\t(игнорируется всеми боевыми системами)")]
	IsCombatIgnored = 1015,
	[Description("Fraction\t(фракция)")] Fraction = 1016,
	[Description("IsOpen\t(открыто)")] IsOpen = 1017,

	[Description("RunSpeedModifier\t(модификатор скорости бега)")]
	RunSpeedModifier = 1018,

	[Description("UseMoneyInTrade\t(использует ли торговец деньги")]
	UseMoneyInTrade = 1019,

	[Description("LinesVision\t(видение линий")]
	LinesVision = 1020,

	[Description("Bullets\t(количество патронов")]
	Bullets = 1021,

	[Description("CloudInfectionDamage\t(Урон заразой от облака)")]
	CloudInfectionDamage = 1022,

	[Description("ContainerInfectionDamage\t(Урон заразой от контейнера)")]
	ContainerInfectionDamage = 1023,

	[Description("DiseasedInfectionDamage\t(Урон заразой от заражённого)")]
	DiseasedInfectionDamage = 1024,

	[Description("WorkTime\t(Время работы)")]
	WorkTime = 1025,

	[Description("WorkEndTime\t(Время завершения работы)")]
	WorkEndTime = 1026,

	[Description("BlockType\t(Тип блока - обычный, быстры, отскок)")]
	BlockType = 1027,

	[Description("CombatStyle\t(Стиль ведения боя)")]
	CombatStyle = 1028,

	[Description("MeleeArmor\t(Броня против холодного оружия)")]
	MeleeArmor = 1029,

	[Description("BallisticArmor\t(Броня против стрелкового оружия)")]
	BallisticArmor = 1030,

	[Description("FireArmor\t(Броня против огня)")]
	FireArmor = 1031,

	[Description("BoundHealthState\t(Состояние здоровья приближенного)")]
	BoundHealthState = 1032,

	[Description("LiquidType\t(Тип жидкости, наливаемой источником воды)")]
	LiquidType = 1033,

	[Description("HealingAttempted\t(Флаг - уже давал антибиотик в этот день, или нет)")]
	HealingAttempted = 1034,

	[Description("ImmuneBoostAttempted\t(Флаг - уже давал иммуник в этот день, или нет)")]
	ImmuneBoostAttempted = 1035,

	[Description("HealingPower\t(Доля инфекции, которая остается после применения на больного)")]
	HealingPower = 1036,

	[Description("ImmuneBoostPower\t(Увеличение иммунитета после применения на больного)")]
	ImmuneBoostPower = 1037,

	[Description("Quality\t(Качество, устойчивость к поломке)")]
	Quality = 1038,

	[Description("NoiseCoefficient\t(коэфициент шума)")]
	NoiseCoefficient = 1039,

	[Description("MeleeAdsorbtion\t(адсорбция мили урона)")]
	MeleeAdsorbtion = 1040,

	[Description("BallisticAdsorbtion\t(адсорбция балистик урона)")]
	BallisticAdsorbtion = 1041,

	[Description("FireAdsorbtion\t(адсорбция огненного урона)")]
	FireAdsorbtion = 1042,

	[Description("InfectionAdsorbtion\t(адсорбция инфекционного урона)")]
	InfectionAdsorbtion = 1043,

	[Description("CloudInfectionAdsorbtion\t(адсорбция облачно-инфекционного урона)")]
	CloudInfectionAdsorbtion = 1044,

	[Description("ContainerInfectionAdsorbtion\t(адсорбция контейнерно-инфекционного урона)")]
	ContainerInfectionAdsorbtion = 1045,

	[Description("DiseasedInfectionAdsorbtion\t(адсорбция заражённо-инфекционного урона)")]
	DiseasedInfectionAdsorbtion = 1046,

	[Description("FistsAdsorbtion\t(адсорбция кулачного урона)")]
	FistsAdsorbtion = 1047,

	[Description("FistsDamage\t(кулачный урон)")]
	FistsDamage = 1048,

	[Description("FistsArmor\t(Броня против кулаков)")]
	FistsArmor = 1049,

	[Description("SavePointIcon\t(Флаг на карте - здесь можно сохраняться)")]
	SavePointIcon = 1050,

	[Description("SleepIcon\t(Флаг на карте - здесь можно спать)")]
	SleepIcon = 1051,

	[Description("CraftIcon\t(Флаг на карте - здесь есть аппараты для крафта)")]
	CraftIcon = 1052,

	[Description("StorageIcon\t(Флаг на карте - здесь можно хранить вещи)")]
	StorageIcon = 1053,

	[Description("MerchantIcon\t(Флаг на карте - здесь есть торговец)")]
	MerchantIcon = 1054,

	[Description("FastTravelPrice\t(Цена фаст тревела в монетах)")]
	FastTravelPrice = 1055,

	[Description("FastTravelPointIndex\t(Локация точки фаст тревела)")]
	FastTravelPointIndex = 1056,

	[Description("CanFastTravel\t(Способность возить игрока)")]
	CanFastTravel = 1057,

	[Description("DoorKnockable\t(DoorKnockable)")]
	DoorKnockable = 1058,

	[Description("DoorPickable\t(DoorPickable)")]
	DoorPickable = 1059,

	[Description("DoorDifficulty\t(DoorDifficulty)")]
	DoorDifficulty = 1060,

	[Description("HealingSuccessfull\t(HealingSuccessfull)")]
	HealingSuccessfull = 1061,

	[Description("ReputationForGifts\t(ReputationForGifts)")]
	ReputationForGifts = 1062,

	[Description("ReputationForGifts\t(SayReplicsInIdle)")]
	SayReplicsInIdle = 1063,

	[Description("FundEnabled\t(FundEnabled)")]
	FundEnabled = 1064,

	[Description("FundFinished\t(FundFinished)")]
	FundFinished = 1065,

	[Description("FundPoints\t(FundPoints)")]
	FundPoints = 1066,

	[Description("RandomRoll\t(Сохранённое значение рандома)")]
	RandomRoll = 1067,

	[Description("CanReceiveMail\t(Может ли получать почту)")]
	CanReceiveMail = 1068,

	[Description("LootAsNPC\t(При луте считается за НПС)")]
	LootAsNPC = 1069
}