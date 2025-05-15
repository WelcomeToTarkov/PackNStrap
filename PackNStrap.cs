#if !UNITY_EDITOR
using BepInEx;
using EFT.InventoryLogic;
using System;
using System.IO;
using System.Reflection;
using PackNStrap.Core.UI;
using PackNStrap.Patches;
using EFT.UI;
using EFT;
using SPT.Reflection.Utils;
using Comfort.Common;
using CustomBelt.Patches;
using PackNStrap.Helpers;


namespace PackNStrap
{
    [BepInPlugin("com.aaaWTT-PacknStrap.Core", "WTT-PackNStrap", "1.0.9")]

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

        private static readonly EquipmentSlot[] NewFastAccessSlots = { 
            EquipmentSlot.Pockets, 
            EquipmentSlot.TacticalVest, 
            EquipmentSlot.ArmBand 
        };
        public FieldInfo BindAvailableSlots { get; set; }

        private static readonly EquipmentSlot[] NewBindAvailableSlots = {
            EquipmentSlot.FirstPrimaryWeapon,
            EquipmentSlot.SecondPrimaryWeapon,
            EquipmentSlot.Holster,
            EquipmentSlot.Scabbard,
            EquipmentSlot.Pockets,
            EquipmentSlot.TacticalVest,
            EquipmentSlot.ArmBand 
        };


        public FieldInfo TraderServicesEligibleSlots;

        private static readonly EquipmentSlot[] NewTraderServicesEligibleSlots =
        {
        EquipmentSlot.Backpack,
        EquipmentSlot.TacticalVest,
        EquipmentSlot.Pockets,
        EquipmentSlot.SecuredContainer,
        EquipmentSlot.ArmBand
        };

        private readonly EquipmentSlot[] _newequipmentSlot0 = 
        {
        EquipmentSlot.TacticalVest,
        EquipmentSlot.Pockets,
        EquipmentSlot.Backpack,
        EquipmentSlot.SecuredContainer,
        EquipmentSlot.Earpiece,
        EquipmentSlot.Headwear,
        EquipmentSlot.Eyewear,
        EquipmentSlot.FaceCover,
        EquipmentSlot.ArmorVest,
        EquipmentSlot.ArmBand
        };



        #endregion

        internal void Awake()
        {
            Instance = this;
            NewTemplateIdToObjectIdClass.AddNewTemplateIdToObjectMapping(NewTemplateIdToObjectIdClass.CustomMappings);
            #region Proper Belt Fast Access
            FastAccessSlots = FastAccessSlots ?? typeof(Inventory).GetField("FastAccessSlots");
            FastAccessSlots?.SetValue(FastAccessSlots, NewFastAccessSlots);

            BindAvailableSlots = BindAvailableSlots ?? typeof(Inventory).GetField("BindAvailableSlotsExtended");
            BindAvailableSlots?.SetValue(BindAvailableSlots, NewBindAvailableSlots);

            TraderServicesEligibleSlots = TraderServicesEligibleSlots ?? typeof(InventoryEquipment).GetField("TraderServicesEligibleSlots");
            TraderServicesEligibleSlots?.SetValue(TraderServicesEligibleSlots, NewTraderServicesEligibleSlots);
            
            

            #endregion

            new GetPrioritizedGridsForUnloadedObjectPatch().Enable();
            new ContainerSlotsPatch().Enable();
            new PaymentSlotsPatch().Enable();
            new GrenadeThrowingSlotsPatch().Enable();
            new MergeContainerWithChildrenPatch().Enable();
            new UnloadWeaponPatch().Enable();
            new FindSlotForPickupPatch().Enable();
        }


        private void Start()
        {
            CustomRigLayouts.LoadRigLayouts();
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