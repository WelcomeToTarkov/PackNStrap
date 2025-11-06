#if !UNITY_EDITOR
using BepInEx;
using EFT.InventoryLogic;
using System;
using System.IO;
using System.Reflection;
using PackNStrap.Patches;
using EFT.UI;
using EFT;
using SPT.Reflection.Utils;
using Comfort.Common;
using PackNStrap.Helpers;
using WTTClientCommonLib.Services;


namespace PackNStrap
{
    [BepInPlugin("com.wtt.packnstrap", "WTT-PackNStrap", "2.0.0")]

    internal class PackNStrap : BaseUnityPlugin
    {
        public static PackNStrap Instance;
        private static GameWorld _gameWorld;
        public static Player Player;
        public static string PlayerNickname;
        private static GameUI _gameUI;
        private static Profile _playerProfile;
        public static ISession BackEndSession;

        public static readonly string PluginPath = Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins");

        #region Proper Armband Slots Info
        public FieldInfo FastAccessSlots { get; set; }

        private static readonly EquipmentSlot[] NewFastAccessSlots =
        [
            EquipmentSlot.Pockets, 
            EquipmentSlot.TacticalVest, 
            EquipmentSlot.ArmBand
        ];
        public FieldInfo BindAvailableSlots { get; set; }

        private static readonly EquipmentSlot[] NewBindAvailableSlots =
        [
            EquipmentSlot.FirstPrimaryWeapon,
            EquipmentSlot.SecondPrimaryWeapon,
            EquipmentSlot.Holster,
            EquipmentSlot.Scabbard,
            EquipmentSlot.Pockets,
            EquipmentSlot.TacticalVest,
            EquipmentSlot.ArmBand
        ];


        private FieldInfo _traderServicesEligibleSlots;

        private static readonly EquipmentSlot[] NewTraderServicesEligibleSlots =
        [
            EquipmentSlot.Backpack,
            EquipmentSlot.TacticalVest,
            EquipmentSlot.Pockets,
            EquipmentSlot.SecuredContainer,
            EquipmentSlot.ArmBand
        ];


        #endregion

        internal void Awake()
        {
            Instance = this;
            CustomTemplateIdToObjectService.AddNewTemplateIdToObjectMapping(NewTemplateIdToObjectIdClass.CustomMappings);
            #region Proper Belt Fast Access
            FastAccessSlots = FastAccessSlots ?? typeof(Inventory).GetField("FastAccessSlots");
            FastAccessSlots?.SetValue(FastAccessSlots, NewFastAccessSlots);

            BindAvailableSlots = BindAvailableSlots ?? typeof(Inventory).GetField("BindAvailableSlotsExtended");
            BindAvailableSlots?.SetValue(BindAvailableSlots, NewBindAvailableSlots);

            _traderServicesEligibleSlots = _traderServicesEligibleSlots ?? typeof(InventoryEquipment).GetField("TraderServicesEligibleSlots");
            _traderServicesEligibleSlots?.SetValue(_traderServicesEligibleSlots, NewTraderServicesEligibleSlots);
            
            #endregion

            new GetPrioritizedGridsForUnloadedObjectPatch().Enable();
            new ContainerSlotsPatch().Enable();
            new PaymentSlotsPatch().Enable();
            new GrenadeThrowingSlotsPatch().Enable();
            new MergeContainerWithChildrenPatch().Enable();
            new UnloadWeaponPatch().Enable();
            new FindSlotForPickupPatch().Enable();
        }

        internal void Update()
        {
            if (Singleton<GameWorld>.Instantiated && (_gameWorld == null || _gameUI == null || Player == null))
            {
                _gameWorld = Singleton<GameWorld>.Instance;
                _gameUI = MonoBehaviourSingleton<GameUI>.Instance;
                Player = Singleton<GameWorld>.Instance.MainPlayer;
                _playerProfile = PatchConstants.BackEndSession.Profile;
                PlayerNickname = _playerProfile.Nickname;
                BackEndSession = PatchConstants.BackEndSession;
            }
        }
    }
}
#endif