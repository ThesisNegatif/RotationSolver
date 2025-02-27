﻿using RotationSolver.Localization;
using RotationSolver.UI;

namespace RotationSolver.ActionSequencer;

internal class ConditionSet : ICondition
{
    public bool IsTrue(ICustomRotation combo, bool isActionSequencer) => Conditions.Count == 0 ? false :
                          IsAnd ? Conditions.All(c => c.IsTrue(combo, isActionSequencer))
                                : Conditions.Any(c => c.IsTrue(combo, isActionSequencer));
    public List<ICondition> Conditions { get; set; } = new List<ICondition>();
    public bool IsAnd { get; set; }

    [JsonIgnore]
    public float Height => Conditions.Sum(c => c is ConditionSet ? c.Height + 10 : c.Height) + ICondition.DefaultHeight + 12;

    public void Draw(ICustomRotation combo, bool isActionSequencer)
    {
        if (ImGui.BeginChild("ConditionSet" + GetHashCode().ToString(), new Vector2(-1f, Height), true))
        {
            AddButton();

            ImGui.SameLine();

            ImGuiHelper.DrawCondition(IsTrue(combo, isActionSequencer));

            ImGui.SameLine();
            ImGui.SetNextItemWidth(65);
            int isAnd = IsAnd ? 1 : 0;
            if (ImGui.Combo("##Rule" + GetHashCode().ToString(), ref isAnd, new string[]
            {
                "OR", "AND",
            }, 2))
            {
                IsAnd = isAnd != 0;
            }

            ImGui.Separator();

            var relay = Conditions;
            if (ImGuiHelper.DrawEditorList(relay, i => i.Draw(combo, isActionSequencer)))
            {
                Conditions = relay;
            }

            ImGui.EndChild();
        }
    }

    private void AddButton()
    {
        if (ImGuiHelper.IconButton(FontAwesomeIcon.Plus, "AddButton" + GetHashCode().ToString()))
        {
            ImGui.OpenPopup("Popup" + GetHashCode().ToString());
        }

        if (ImGui.BeginPopup("Popup" + GetHashCode().ToString()))
        {
            AddOneCondition<ConditionSet>(LocalizationManager.RightLang.ActionSequencer_ConditionSet);
            AddOneCondition<ActionCondition>(LocalizationManager.RightLang.ActionSequencer_ActionCondition);
            AddOneCondition<TargetCondition>(LocalizationManager.RightLang.ActionSequencer_TargetCondition);
            AddOneCondition<RotationCondition>(LocalizationManager.RightLang.ActionSequencer_RotationCondition);

            ImGui.EndPopup();
        }
    }

    private void AddOneCondition<T>(string name) where T : ICondition
    {
        if (ImGui.Selectable(name))
        {
            Conditions.Add(Activator.CreateInstance<T>());
            ImGui.CloseCurrentPopup();
        }
    }
}
