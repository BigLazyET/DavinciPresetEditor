using PresetEditor.Models;

namespace PresetEditor.Services
{
    public interface IPresetSettingSegment
    {
        string OrderedInputs2Json(string text);

        IList<InstanceInput>? OrderedInputs2List(string json);

        string OrderedInputs2Text(List<InstanceInput> inputs);
    }
}
