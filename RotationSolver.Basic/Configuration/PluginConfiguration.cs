using Dalamud.Configuration;
using Dalamud.Game.ClientState.GamePad;
using Dalamud.Logging;

namespace RotationSolver.Basic.Configuration;

[Serializable]
public class PluginConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 6;

    public int VoiceVolume = 100;
    public SortedSet<string> DisabledCombos { get; private set; } = new SortedSet<string>();
    public SortedSet<uint> DisabledActions { get; private set; } = new SortedSet<uint>();
    public SortedSet<uint> NotInCoolDownActions { get; private set; } = new SortedSet<uint>();
    public SortedSet<uint> DisabledItems { get; private set; } = new SortedSet<uint>();
    public SortedSet<uint> NotInCoolDownItems { get; private set; } = new SortedSet<uint>();

    public List<ActionEventInfo> Events { get; private set; } = new List<ActionEventInfo>();
    public Dictionary<uint, Dictionary<string, Dictionary<string, string>>> RotationsConfigurations { get; private set; }
        = new Dictionary<uint, Dictionary<string, Dictionary<string, string>>>();
    public Dictionary<uint, string> RotationChoices { get; private set; } = new Dictionary<uint, string>();
    public Dictionary<uint, byte> TargetToHostileTypes { get; set; } =
        new Dictionary<uint, byte>();

    [JsonProperty]
    private Dictionary<SettingsCommand, bool> SettingsBools { get; set; } = new Dictionary<SettingsCommand, bool>();
    public bool GetValue(SettingsCommand command) => Service.Config.SettingsBools.TryGetValue(command, out var value) ? value : command.GetDefault();
    public void SetValue(SettingsCommand command, bool value) => Service.Config.SettingsBools[command] = value;

    public int AddDotGCDCount = 2;

    public int ActionSequencerIndex = 0;

    public bool AutoOffBetweenArea = true;
    public bool AutoOffCutScene = true;
    public bool AutoOffWhenDead = true;
    public bool PreventActionsIfOutOfCombat = false;
    public bool ChangeTargetForFate = true;
    public bool MoveTowardsScreenCenter = true;

    public bool SayOutStateChanged = true;

    public bool ShowInfoOnDtr = true;

    public bool SayPositional = true;

    public bool ToastPositional = true;
    public bool HealOutOfCombat = false;
    public bool ShowInfoOnToast = true;
    public bool RaiseAll = false;
    public bool CastingDisplay = true;
    public bool PoslockCasting = false;
    public int PoslockModifier = 0;
    public bool RaisePlayerByCasting = true;
    public bool RaiseBrinkOfDeath = true;
    public int LessMPNoRaise = 0;
    public bool AddEnemyListToHostile = true;
    public bool UseItem = false;
    public bool PositionalFeedback = true;
    public bool DrawPositional = true;
    public bool DrawMeleeRange = true;
    public bool DrawMeleeOffset = true;
    public bool ShowMoveTarget = true;
    public bool ShowHealthRatio = false;
    public bool ShowTarget = true;
    public bool ChooseAttackMark = true;
    public bool CanAttackMarkAOE = true;
    public bool FilterStopMark = true;
    public bool UseOverlayWindow = true;
    public bool TeachingMode = true;
    public Vector3 TeachingModeColor = new(0f, 1f, 0.8f);
    public Vector3 MovingTargetColor = new(0f, 1f, 0.8f);
    public Vector3 TargetColor = new(1f, 0.2f, 0f);
    public Vector3 SubTargetColor = new(1f, 0.9f, 0f);
    public bool KeyBoardNoise = true;
    public int KeyBoardNoiseMin = 2;
    public int KeyBoardNoiseMax = 3;
    public float KeyBoardNoiseTimeMin = 0.1f;
    public float KeyBoardNoiseTimeMax = 0.2f;
    public bool MoveAreaActionFarthest = true;
    public bool StartOnCountdown = true;
    public bool NoNewHostiles = false;
    public bool UseHealWhenNotAHealer = true;
    public float ObjectMinRadius = 0f;
    public float HealthDifference = 0.25f;
    public float MeleeRangeOffset = 1;
    public bool TargetFriendly = false;
    public float AlphaInFill = 0.15f;
    public float MinLastAbilityAdvanced = 0.1f;

    public Dictionary<ClassJobID, float> HealingOfTimeSubtractSingles { get; set; } = new Dictionary<ClassJobID, float>();

    public Dictionary<ClassJobID, float> HealingOfTimeSubtractAreas { get; set; } = new Dictionary<ClassJobID, float>();
    public Dictionary<ClassJobID, float> HealthAreaAbilities { get; set; } = new Dictionary<ClassJobID, float>();
    public float HealthAreaAbility = 0.75f;

    public Dictionary<ClassJobID, float> HealthAreaSpells { get; set; } = new Dictionary<ClassJobID, float>();
    public float HealthAreaSpell = 0.65f;

    public Dictionary<ClassJobID, float> HealthSingleAbilities { get; set; } = new Dictionary<ClassJobID, float>();
    public float HealthSingleAbility = 0.7f;

    public Dictionary<ClassJobID, float> HealthSingleSpells { get; set; } = new Dictionary<ClassJobID, float>();
    public float HealthSingleSpell = 0.55f;

    public Dictionary<ClassJobID, float> HealthForDyingTanks { get; set; } = new Dictionary<ClassJobID, float>();

    public float HealthTankRatio = 0.4f;
    public float HealthHealerRatio = 0.4f;

    public bool InterruptibleMoreCheck = true;
    public float SpecialDuration = 3;

    public float ActionAhead = 0.08f;
    public float ActionAheadForLast0GCD = 0.06f;

    public float WeaponDelayMin = 0;
    public float WeaponDelayMax = 0;

    public float DeathDelayMin = 0.5f;
    public float DeathDelayMax = 1;

    public float WeakenDelayMin = 0.5f;
    public float WeakenDelayMax = 1;

    public float HostileDelayMin = 0;
    public float HostileDelayMax = 0;

    public float HealDelayMin = 0;
    public float HealDelayMax = 0;
    public float StopCastingDelayMin = 0.5f;
    public float StopCastingDelayMax = 1;

    public float InterruptDelayMin = 0.5f;
    public float InterruptDelayMax = 1;

    public float NotInCombatDelayMin = 1f;
    public float NotInCombatDelayMax = 2;

    public float ClickingDelayMin = 0.1f;
    public float ClickingDelayMax = 0.15f;

    public bool UseWorkTask = true;

    public bool UseStopCasting = false;
    public bool EsunaAll = false;
    public bool OnlyAttackInView = false;
    public bool OnlyHotOnTanks = false;
    public bool BeneficialAreaOnTarget = false;

    public string PositionalErrorText = string.Empty;
    public float CountDownAhead = 0.6f;

    public int NamePlateIconId = 61437; // 61435, 0
    public bool ShowActionFlag = false;


    public int MoveTargetAngle = 24;
    public float HealthRatioBoss = 1.85f;
    public float HealthRatioDying = 0.8f;
    public float HealthRatioDot = 1.2f;

    public bool InDebug = false;
    public bool AutoUpdateLibs = true;
    public string[] OtherLibs = new string[0];
    public string[] NoHostileNames = new string[0];

    public List<TargetingType> TargetingTypes { get; set; } = new List<TargetingType>();
    public int TargetingIndex { get; set; } = 0;
    public MacroInfo DutyStart { get; set; } = new MacroInfo();
    public MacroInfo DutyEnd { get; set; } = new MacroInfo();

    public bool DownloadRotations = true;
    public bool AutoUpdateRotations = true;

    public bool ToggleManual = false;
    public bool OnlyShowWithHostileOrInDuty = true;
    public bool ShowControlWindow = false;
    public bool IsControlWindowLock = false;
    public bool ShowNextActionWindow = true;
    public bool IsInfoWindowNoInputs = false;
    public bool IsInfoWindowNoMove = false;
    public bool UseKeyboardCommand = false;
    public bool UseGamepadCommand = false;
    public bool ShowItemsCooldown = false;
    public bool ShowGCDCooldown = false;
    public bool UseOriginalCooldown = true;
    public int CooldownActionOneLine = 15;
    public float CooldownFontSize = 16;

    public Vector4 ControlWindowLockBg = new Vector4(0, 0, 0, 0.6f);
    public Vector4 ControlWindowUnlockBg = new Vector4(0, 0, 0, 0.9f);
    public Vector4 InfoWindowBg = new Vector4(0, 0, 0, 0.4f);

    public float ControlWindowGCDSize = 40;
    public float ControlWindow0GCDSize = 30;
    public float CooldownWindowIconSize = 30;
    public float ControlWindowNextSizeRatio = 1.5f;
    public float ControlProgressHeight = 8;
    public bool ShowCooldownWindow = false;

    public Dictionary<StateCommandType, KeyRecord> KeyState { get; set; } = new Dictionary<StateCommandType, KeyRecord>();
    public Dictionary<SpecialCommandType, KeyRecord> KeySpecial { get; set; } = new Dictionary<SpecialCommandType, KeyRecord>();
    public KeyRecord KeyDoAction { get; set; } = null;
    public Dictionary<StateCommandType, ButtonRecord> ButtonState { get; set; } = new Dictionary<StateCommandType, ButtonRecord>()
    {
        {StateCommandType.Smart, new ButtonRecord( GamepadButtons.East, false, true) },
        {StateCommandType.Manual, new ButtonRecord( GamepadButtons.North, false, true) },
        {StateCommandType.Cancel, new ButtonRecord( GamepadButtons.South, false, true) },
    };
    public Dictionary<SpecialCommandType, ButtonRecord> ButtonSpecial { get; set; } = new Dictionary<SpecialCommandType, ButtonRecord>()
    {
        {SpecialCommandType.EndSpecial, new ButtonRecord( GamepadButtons.West, false, true) },

        {SpecialCommandType.EsunaStanceNorth, new ButtonRecord( GamepadButtons.DpadRight, false, true) },
        {SpecialCommandType.MoveForward, new ButtonRecord( GamepadButtons.DpadUp, false, true) },
        {SpecialCommandType.MoveBack, new ButtonRecord( GamepadButtons.DpadDown, false, true) },
        {SpecialCommandType.RaiseShirk, new ButtonRecord( GamepadButtons.DpadLeft, false, true) },

        {SpecialCommandType.DefenseArea, new ButtonRecord( GamepadButtons.North, true, false) },
        {SpecialCommandType.DefenseSingle, new ButtonRecord( GamepadButtons.East, true, false) },
        {SpecialCommandType.HealArea, new ButtonRecord( GamepadButtons.South, true, false) },
        {SpecialCommandType.HealSingle, new ButtonRecord( GamepadButtons.West, true, false) },

        {SpecialCommandType.Burst, new ButtonRecord( GamepadButtons.DpadDown, true, false) },
        {SpecialCommandType.AntiKnockback, new ButtonRecord( GamepadButtons.DpadUp, true, false) },

    };

    public ButtonRecord ButtonDoAction { get; set; } = null;

    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }
}
