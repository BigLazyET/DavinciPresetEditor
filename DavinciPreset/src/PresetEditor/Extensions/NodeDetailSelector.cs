using PresetEditor;
using PresetEditor.Models;
using PresetEditor.Views;

namespace ResponsiveMaui.Extensions;

public class NodeDetailSelector : DataTemplateSelector
{
    private static readonly DataTemplate GroupNodeTemplate = new(() => App.Current.GetView<GroupNodeView>());
    private static readonly DataTemplate PresetNodeTemplate = new(() => App.Current.GetView<PresetNodeView>());
    
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is not NodeMenuItem nodeMenuItem) return null;
        return nodeMenuItem.Id switch
        {
            NodeType.GroupNode => GroupNodeTemplate,
            NodeType.PresetNode => PresetNodeTemplate,
            _ => null
        };
    }
}