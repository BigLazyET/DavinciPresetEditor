using PresetEditor.Models;

namespace PresetEditor.Services
{
    public interface IPresetSettingSegment
    {
        IEnumerable<InstanceInput>? GetOrderedInstanceInputs(string text);

        IEnumerable<GroupInput>? GetGroupInputs(string text, string keyword,
            string subKeyword = "UserControls = ordered()");
    }
}
