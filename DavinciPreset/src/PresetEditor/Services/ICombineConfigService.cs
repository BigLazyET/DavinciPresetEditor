namespace PresetEditor.Services;

public interface ICombineConfigService
{
    string? ReplaceGroupBlockByKeyword(string original, string keyword, string newContent,
        string subKeyword = "UserControls = ordered()");

    string? ReplaceBlockByKeyword(string original, string keyword, string newContent);
}