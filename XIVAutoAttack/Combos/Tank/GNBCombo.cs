using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.GNBCombo;

namespace XIVAutoAttack.Combos.Tank;

internal sealed class GNBCombo : JobGaugeCombo<GNBGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public override uint JobID => 37;
    internal override bool HaveShield => Player.HaveStatus(ObjectStatus.RoyalGuard);
    private protected override BaseAction Shield => Actions.RoyalGuard;

    protected override bool CanHealSingleSpell => false;
    protected override bool CanHealAreaSpell => false;

    /// <summary>
    /// 在4人本的道中已经聚好怪可以使用相关技能(不移动且身边有大于3只小怪)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving 
                                                    && TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// 在4人本的道中
    /// </summary>
    private static bool InDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    internal struct Actions
    {
        public static readonly BaseAction
            //王室亲卫
            RoyalGuard = new(16142, shouldEndSpecial: true),

            //利刃斩
            KeenEdge = new(16137),

            //无情
            NoMercy = new(16138),

            //残暴弹
            BrutalShell = new(16139),

            //伪装
            Camouflage = new(16140)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //恶魔切
            DemonSlice = new(16141),

            //闪雷弹
            LightningShot = new(16143),

            //危险领域
            DangerZone = new(16144),

            //迅连斩
            SolidBarrel = new(16145),

            //爆发击
            BurstStrike = new(16162)
            {
                OtherCheck = b => JobGauge.Ammo > 0,
            },

            //星云
            Nebula = new(16148)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //恶魔杀
            DemonSlaughter = new(16149),

            //极光
            Aurora = new BaseAction(16151, true)
            {
                BuffsProvide = new[] { ObjectStatus.Aurora },
            },

            //超火流星
            Superbolide = new(16152)
            {
                OtherCheck = BaseAction.TankBreakOtherCheck,
            },

            //音速破
            SonicBreak = new(16153),

            //粗分斩
            RoughDivide = new(16154, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindTargetForMoving
            },

            //烈牙
            GnashingFang = new(16146)
            {
                OtherCheck = b => JobGauge.AmmoComboStep == 0 && JobGauge.Ammo > 0,
            },

            //弓形冲波
            BowShock = new(16159),

            //光之心
            HeartofLight = new(16160, true),

            //石之心
            HeartofStone = new(16161, true)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //命运之环
            FatedCircle = new(16163)
            {
                OtherCheck = b => JobGauge.Ammo > (Level >= 88 ? 2 : 1),
            },

            //血壤
            Bloodfest = new(16164)
            {
                OtherCheck = b => JobGauge.Ammo == 0,
            },

            //倍攻
            DoubleDown = new(25760)
            {
                OtherCheck = b => JobGauge.Ammo >= 2,
            },

            //猛兽爪
            SavageClaw = new(16147)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == SavageClaw.ID,
            },

            //凶禽爪
            WickedTalon = new(16150)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(GnashingFang.ID) == WickedTalon.ID,
            },

            //撕喉
            JugularRip = new (16156)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == JugularRip.ID,
            },

            //裂膛
            AbdomenTear = new (16157)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == AbdomenTear.ID,
            },

            //穿目
            EyeGouge = new (16158)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == EyeGouge.ID,
            },

            //超高速
            Hypervelocity = new (25759)
            {
                OtherCheck = b => Service.IconReplacer.OriginalHook(16155) == Hypervelocity.ID,
            };          
    }
    public override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体治疗, $"{Actions.Aurora}"},
        {DescType.范围防御, $"{Actions.HeartofLight}"},
        {DescType.单体防御, $"{Actions.HeartofStone}, {Actions.Nebula}, {Actions.Camouflage}"},
        {DescType.移动技能, $"{Actions.RoughDivide}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //烈牙
        if (CanUseGnashingFang(out act)) return true;
     
        //音速破
        if (CanUseSonicBreak(out act)) return true;

        //倍攻
        if (CanUseDoubleDown(out act)) return true;

        //烈牙后二连
        if (Actions.WickedTalon.ShouldUse(out act)) return true;
        if (Actions.SavageClaw.ShouldUse(out act)) return true;

        //命运之环 AOE
        if (Actions.FatedCircle.ShouldUse(out act)) return true;

        //爆发击   
        if (CanUseBurstStrike(out act)) return true;

        //AOE
        if (Actions.DemonSlaughter.ShouldUse(out act)) return true;
        if (Actions.DemonSlice.ShouldUse(out act)) return true;

        //单体三连
        //如果烈牙剩0.5秒冷却好,不释放基础连击,主要因为技速不同可能会使烈牙延后太多所以判定一下
        if (Actions.GnashingFang.IsCoolDown && Actions.GnashingFang.WillHaveOneCharge((float) 0.5, false) && Actions.GnashingFang.EnoughLevel) return false;
        if (Actions.SolidBarrel.ShouldUse(out act)) return true;
        if (Actions.BrutalShell.ShouldUse(out act)) return true;
        if (Actions.KeenEdge.ShouldUse(out act)) return true;

        
        if (CommandController.Move && MoveAbility(1, out act)) return true;

        if (Actions.LightningShot.ShouldUse(out act))
        {
            if (InDungeonsMiddle && Actions.LightningShot.Target.DistanceToPlayer() > 3) return true;
        }

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //超火流星 如果谢不够了。
        if (Actions.Superbolide.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //无情,目前只有4GCD起手的判断
        if (SettingBreak && abilityRemain == 1 && CanUseNoMercy(out act)) return true;

        //危险领域
        if (Actions.DangerZone.ShouldUse(out act))
        {
            if (InDungeonsMiddle) return true;

            //等级小于烈牙,
            if (!Actions.GnashingFang.EnoughLevel && (Player.HaveStatus(ObjectStatus.NoMercy) || !Actions.NoMercy.WillHaveOneCharge(15, false))) return true;

            //爆发期,烈牙之后
            if (Player.HaveStatus(ObjectStatus.NoMercy) && Actions.GnashingFang.IsCoolDown) return true;

            //非爆发期
            if (!Player.HaveStatus(ObjectStatus.NoMercy) && !Actions.GnashingFang.WillHaveOneCharge(20, false)) return true;
        }

        //弓形冲波
        if (CanUseBowShock(out act)) return true;

        //续剑
        if (Actions.JugularRip.ShouldUse(out act)) return true;
        if (Actions.AbdomenTear.ShouldUse(out act)) return true;
        if (Actions.EyeGouge.ShouldUse(out act)) return true;
        if (Actions.Hypervelocity.ShouldUse(out act)) return true;

        //血壤
        if (Actions.GnashingFang.IsCoolDown && Actions.Bloodfest.ShouldUse(out act)) return true;

        //搞搞攻击,粗分斩
        if (Actions.RoughDivide.Target.DistanceToPlayer() < 1 && !IsMoving)
        {  
            if (Actions.RoughDivide.ShouldUse(out act)) return true;
            if (Player.HaveStatus(ObjectStatus.NoMercy) && Actions.RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        }
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.HeartofLight.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //突进
        if (Actions.RoughDivide.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (abilityRemain == 2)
        {

            //减伤10%）
            if (Actions.HeartofStone.ShouldUse(out act)) return true;

            //星云（减伤30%）
            if (Actions.Nebula.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;

            //伪装（减伤10%）
            if (Actions.Camouflage.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Aurora.ShouldUse(out act, emptyOrSkipCombo: true) && abilityRemain == 1) return true;

        return false;
    }

    private bool CanUseNoMercy(out IAction act)
    {
        if (InDungeonsMiddle && Actions.NoMercy.ShouldUse(out act))
        {
            if (CanUseSpellInDungeonsMiddle) return true;
            return false;
        }
        //等级低于爆发击是判断
        if (!Actions.BurstStrike.EnoughLevel && Actions.NoMercy.ShouldUse(out act)) return true;

        if (Actions.BurstStrike.EnoughLevel && Actions.NoMercy.ShouldUse(out act))
        {
            //4GCD起手判断
            if (IsLastWeaponSkill(Actions.KeenEdge.ID) && JobGauge.Ammo == 1 && !Actions.GnashingFang.IsCoolDown && !Actions.Bloodfest.IsCoolDown) return true;

            //3弹进无情
            else if (JobGauge.Ammo == (Level >= 88 ? 3 : 2)) return true;

            //2弹进无情
            else if (JobGauge.Ammo == 2 && Actions.GnashingFang.IsCoolDown) return true;
        }


        act = null;
        return false;
    }

    private bool CanUseGnashingFang(out IAction act)
    {
        //基础判断
        if (Actions.GnashingFang.ShouldUse(out act))
        {
            //在4人本道中使用
            if (InDungeonsMiddle) return true;

            //无情中3弹烈牙
            if (JobGauge.Ammo == (Level >= 88 ? 3 : 2) && (Player.HaveStatus(ObjectStatus.NoMercy) || !Actions.NoMercy.WillHaveOneCharge(55, false))) return true;

            //无情外烈牙
            if (JobGauge.Ammo > 0 && !Actions.NoMercy.WillHaveOneCharge(17, false) && Actions.NoMercy.WillHaveOneCharge(35, false)) return true;

            //3弹且将会溢出子弹的情况,提前在无情前进烈牙
            if (JobGauge.Ammo == 3 && IsLastWeaponSkill(Actions.BrutalShell.ID) && Actions.NoMercy.WillHaveOneCharge(3, false)) return true;

            //1弹且血壤快冷却好了
            if (JobGauge.Ammo == 1 && !Actions.NoMercy.WillHaveOneCharge(55, false) && Actions.Bloodfest.WillHaveOneCharge(5, false)) return true;

            //4GCD起手烈牙判断
            if (JobGauge.Ammo == 1 && !Actions.NoMercy.WillHaveOneCharge(55, false) && ((!Actions.Bloodfest.IsCoolDown && Actions.Bloodfest.EnoughLevel) || !Actions.Bloodfest.EnoughLevel)) return true;
        }
        return false;   
    }

    /// <summary>
    /// 音速破
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseSonicBreak(out IAction act)
    {
        //基础判断
        if (Actions.SonicBreak.ShouldUse(out act))
        {
            //在4人本道中不使用
            if (InDungeonsMiddle) return false;

            if (!Actions.GnashingFang.EnoughLevel && Player.HaveStatus(ObjectStatus.NoMercy)) return true;

            //在烈牙后面使用音速破
            if (Actions.GnashingFang.IsCoolDown && Player.HaveStatus(ObjectStatus.NoMercy)) return true;

            //其他判断
            if (!Actions.DoubleDown.EnoughLevel && Player.HaveStatus(ObjectStatus.ReadyToRip)
                && Actions.GnashingFang.IsCoolDown) return true;
            
        }        
        return false;
    }

    /// <summary>
    /// 倍攻
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseDoubleDown(out IAction act)
    {      
        //基本判断
        if (Actions.DoubleDown.ShouldUse(out act, mustUse: true))
        {
            //在4人本道中
            if (InDungeonsMiddle)
            {
                //在4人本的道中已经聚好怪可以使用相关技能(不移动且身边有大于3只小怪),有无情buff
                if (Player.HaveStatus(ObjectStatus.NoMercy)) return true;

                return false;
            }

            //在音速破后使用倍攻
            if (Actions.SonicBreak.IsCoolDown && Player.HaveStatus(ObjectStatus.NoMercy)) return true;

            //2弹无情的特殊判断,提前使用倍攻
            if (Player.HaveStatus(ObjectStatus.NoMercy) && !Actions.NoMercy.WillHaveOneCharge(55, false) && Actions.Bloodfest.WillHaveOneCharge(5, false)) return true;

        }
        return false;
    }
    
    /// <summary>
    /// 爆发击
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseBurstStrike(out IAction act)
    {
        if (Actions.BurstStrike.ShouldUse(out act))
        {
            //在4人本道中且AOE时不使用
            if (InDungeonsMiddle && Actions.DemonSlice.ShouldUse(out _)) return false;

            //如果烈牙剩0.5秒冷却好,不释放爆发击,主要因为技速不同可能会使烈牙延后太多所以判定一下
            if (Actions.SonicBreak.IsCoolDown && Actions.SonicBreak.WillHaveOneCharge((float) 0.5, false) && Actions.GnashingFang.EnoughLevel) return false;

            //无情中爆发击判定
            if (Player.HaveStatus(ObjectStatus.NoMercy) &&
                JobGauge.AmmoComboStep == 0 &&
                !Actions.GnashingFang.WillHaveOneCharge(1, false)) return true;
            if (Level < 88 && JobGauge.Ammo == 2) return true;
            //无情外防止溢出
            if (IsLastWeaponSkill(Actions.BrutalShell.ID) &&
                (JobGauge.Ammo == (Level >= 88 ? 3 : 2) ||
                (Actions.Bloodfest.WillHaveOneCharge(6, false) && JobGauge.Ammo <= 2 && !Actions.NoMercy.WillHaveOneCharge(10, false) && Actions.Bloodfest.EnoughLevel))) return true;

        }
        return false;
    }
    
    private bool CanUseBowShock(out IAction act)
    {
        if (Actions.BowShock.ShouldUse(out act, mustUse: true))
        {
            if (InDungeonsMiddle) return true;

            if (!Actions.SonicBreak.EnoughLevel && Player.HaveStatus(ObjectStatus.NoMercy)) return true;

            //爆发期,无情中且音速破在冷却中
            if (Player.HaveStatus(ObjectStatus.NoMercy) && Actions.SonicBreak.IsCoolDown) return true;

        }
        return false;     
    }
}
    
