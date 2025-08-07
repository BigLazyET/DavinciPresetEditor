using CommunityToolkit.Mvvm.ComponentModel;

namespace PresetEditor.Models;

public partial class NodeMenuItem : ObservableObject
{
    [ObservableProperty] private NodeType _id;
    [ObservableProperty] private string _title;
    [ObservableProperty] private Color _backColor = Colors.Transparent;
}

public enum NodeType
{
    GroupNode,
    PresetNode
}