using PresetEditor.Models;

namespace PresetEditor.Services
{
    public interface IPresetSettingSegment
    {
        IEnumerable<InstanceInput>? GetOrderedInstanceInputs(string text);

        IEnumerable<GroupInput>? GetGroupInputs(string text, string keyword,
            string subKeyword = "UserControls = ordered()");

        string OrderedInputs2Text(IList<InstanceInput> inputs);
        
        string OrderedGroups2Text(IList<GroupInput> inputs);
    }
}
