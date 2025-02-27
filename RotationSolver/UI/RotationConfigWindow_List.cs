﻿using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.ActionSequencer;
using RotationSolver.Basic.Data;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.UI;

internal partial class RotationConfigWindow
{
    private static BaseStatus[] _allDispelStatus = null;
    public static BaseStatus[] AllDispelStatus
    {
        get
        {
            if (_allDispelStatus == null)
            {
                _allDispelStatus = Service.GetSheet<Status>()
                    .Where(s => s.CanDispel)
                    .Select(s => new BaseStatus(s))
                    .ToArray();
            }
            return _allDispelStatus;
        }
    }

    private static BaseStatus[] _allInvStatus = null;
    public static BaseStatus[] AllInvStatus
    {
        get
        {
            if (_allInvStatus == null)
            {
                _allInvStatus = Service.GetSheet<Status>()
                    .Where(s => !s.CanDispel && !s.LockMovement && !s.IsPermanent && !s.IsGaze && !s.IsFcBuff && s.HitEffect.Row == 16 && s.ClassJobCategory.Row == 1 && s.StatusCategory == 1
                        && !string.IsNullOrEmpty(s.Name.ToString()) && s.Icon != 0)
                    .Select(s => new BaseStatus(s))
                    .ToArray();
            }
            return _allInvStatus;
        }
    }

    private void DrawListTab()
    {
        ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("List Items"))
        {
            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Hostile, DrawParamHostile, () =>
            {
                if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_AddOne))
                {
                    Service.Config.TargetingTypes.Add(TargetingType.Big);
                }
                ImGui.SameLine();
                ImGuiHelper.Spacing();
                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_Param_HostileDesc);
            });

            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_NoHostile, DrawParamNoHostile, () =>
            {
                if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_AddOne))
                {
                    Service.Config.NoHostileNames = Service.Config.NoHostileNames.Append(string.Empty).ToArray();
                }
                ImGui.SameLine();
                ImGuiHelper.Spacing();
                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_NoHostileDesc);
            });

            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Invincibility, DrawInvincibility, () =>
            {
                ImGui.SetNextItemWidth(200);
                ImGuiHelper.SearchCombo("##AddInvincibleStatus",
                    LocalizationManager.RightLang.ConfigWindow_Param_AddOne,
                    ref searchText, AllInvStatus, s =>
                    {
                        StatusHelper.InvincibleStatus.Add((uint)s.ID);
                        StatusHelper.SaveInvincibleStatus();

                    });

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_InvincibilityDesc);
            });

            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatus, DrawDangerousStatus, () =>
            {
                ImGui.SetNextItemWidth(200);
                ImGuiHelper.SearchCombo("##AddDangerousStatus",
                    LocalizationManager.RightLang.ConfigWindow_Param_AddOne,
                    ref searchText, AllDispelStatus, s =>
                    {
                        StatusHelper.DangerousStatus.Add((uint)s.ID);
                        StatusHelper.SaveDangerousStatus();
                    });

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                ImGui.TextWrapped(LocalizationManager.RightLang.ConfigWindow_List_DangerousStatusDesc);
            });


            DrawParamTabItem(LocalizationManager.RightLang.ConfigWindow_List_Rotations, DrawRotationDevTab, () =>
            {
                if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Rotation_DownloadRotationsButton))
                {
                    RotationUpdater.GetAllCustomRotations(RotationUpdater.DownloadOption.MustDownload | RotationUpdater.DownloadOption.ShowList);
                }

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                if (ImGui.Button("Load Rotations Local"))
                {
                    RotationUpdater.GetAllCustomRotations(RotationUpdater.DownloadOption.ShowList);
                }

                ImGui.SameLine();
                ImGuiHelper.Spacing();

                if (ImGui.Button("Dev Wiki"))
                {
                    Util.OpenLink("https://archidog1998.github.io/RotationSolver/#/RotationDev/");
                }

                if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_AddOne))
                {
                    Service.Config.OtherLibs = Service.Config.OtherLibs.Append(string.Empty).ToArray();
                }
                ImGui.SameLine();
                ImGuiHelper.Spacing();

                ImGui.TextWrapped("Third-party Rotation Libraries");
            });

            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
    }

    private void DrawParamHostile()
    {
        for (int i = 0; i < Service.Config.TargetingTypes.Count; i++)
        {
            ImGui.Separator();

            var names = Enum.GetNames(typeof(TargetingType));
            var targingType = (int)Service.Config.TargetingTypes[i];

            ImGui.SetNextItemWidth(150);

            if (ImGui.Combo(LocalizationManager.RightLang.ConfigWindow_Param_HostileCondition + "##HostileCondition" + i.ToString(), ref targingType, names, names.Length))
            {
                Service.Config.TargetingTypes[i] = (TargetingType)targingType;
                Service.Config.Save();
            }

            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionUp + "##HostileUp" + i.ToString()))
            {
                if (i != 0)
                {
                    var value = Service.Config.TargetingTypes[i];
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(i - 1, value);
                }
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();
            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionDown + "##HostileDown" + i.ToString()))
            {
                if (i < Service.Config.TargetingTypes.Count - 1)
                {
                    var value = Service.Config.TargetingTypes[i];
                    Service.Config.TargetingTypes.RemoveAt(i);
                    Service.Config.TargetingTypes.Insert(i + 1, value);
                }
            }

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGui.Button(LocalizationManager.RightLang.ConfigWindow_Param_ConditionDelete + "##HostileDelete" + i.ToString()))
            {
                Service.Config.TargetingTypes.RemoveAt(i);
            }
        }
    }

    string searchText = string.Empty;
    private void DrawDangerousStatus()
    {
        uint removeId = 0;
        uint addId = 0;
        foreach (var statusId in StatusHelper.DangerousStatus)
        {
            var status = Service.GetSheet<Status>().GetRow(statusId);
            ImGui.Image(IconSet.GetTexture(status.Icon).ImGuiHandle, new Vector2(24, 30));

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            ImGui.SetNextItemWidth(150);

            ImGuiHelper.SearchCombo($"##DangerousStatus{statusId}",
                $"{status.Name} ({status.RowId})",
                ref searchText, AllDispelStatus, s =>
                {
                    removeId = statusId;
                    addId = (uint)s.ID;
                });

            ImGui.SameLine();
            ImGuiHelper.Spacing();


            if (ImGui.Button($"X##RemoveDangerous{statusId}"))
            {
                removeId = statusId;
            }
        }

        if(removeId != 0)
        {
            StatusHelper.DangerousStatus.Remove(removeId);
            StatusHelper.SaveDangerousStatus();
        }
        if (addId != 0)
        {
            StatusHelper.DangerousStatus.Add(addId);
            StatusHelper.SaveDangerousStatus();
        }
    }

    private void DrawInvincibility()
    {
        uint removeId = 0;
        uint addId = 0;
        foreach (var statusId in StatusHelper.InvincibleStatus)
        {
            var status = Service.GetSheet<Status>().GetRow(statusId);
            ImGui.Image(IconSet.GetTexture(status.Icon).ImGuiHandle, new Vector2(24, 30));

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            ImGui.SetNextItemWidth(150);

            ImGuiHelper.SearchCombo($"##InvincibleStatus{statusId}",
                $"{status.Name} ({status.RowId})",
                ref searchText, AllInvStatus, s =>
                {
                    removeId = statusId;
                    addId = (uint)s.ID;
                });

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGui.Button($"X##InvincibleStatus{statusId}"))
            {
                removeId = statusId;
            }
        }

        if (removeId != 0)
        {
            StatusHelper.InvincibleStatus.Remove(removeId);
            StatusHelper.SaveInvincibleStatus();
        }
        if (addId != 0)
        {
            StatusHelper.InvincibleStatus.Add(addId);
            StatusHelper.SaveInvincibleStatus();
        }
    }

    private void DrawRotationDevTab()
    {
        int removeIndex = -1;
        for (int i = 0; i < Service.Config.OtherLibs.Length; i++)
        {
            if (ImGui.InputText($"##OtherLib{i}", ref Service.Config.OtherLibs[i], 1024))
            {
                Service.Config.Save();
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGui.Button($"X##RemoveOtherLibs{i}"))
            {
                removeIndex = i;
            }
        }
        if (removeIndex > -1)
        {
            var list = Service.Config.OtherLibs.ToList();
            list.RemoveAt(removeIndex);
            Service.Config.OtherLibs = list.ToArray();
            Service.Config.Save();
        }
    }

    private void DrawParamNoHostile()
    {
        int removeIndex = -1;
        for (int i = 0; i < Service.Config.NoHostileNames.Length; i++)
        {
            if (ImGui.InputText($"##NoHostileNames{i}", ref Service.Config.NoHostileNames[i], 1024))
            {
                Service.Config.Save();
            }
            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGui.Button($"X##RemoveNoHostileNames{i}"))
            {
                removeIndex = i;
            }
        }
        if (removeIndex > -1)
        {
            var list = Service.Config.NoHostileNames.ToList();
            list.RemoveAt(removeIndex);
            Service.Config.NoHostileNames = list.ToArray();
            Service.Config.Save();
        }
    }

}
