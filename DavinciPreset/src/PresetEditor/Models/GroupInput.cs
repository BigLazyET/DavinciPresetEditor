using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PresetEditor.Models;

public partial class GroupInput : ObservableObject
{
    [ObservableProperty] private string _groupSourceName;

    [ObservableProperty]
    private ObservableCollection<InputItem> _propertyList = [];

    public GroupInput()
    {
        // 1) 监听集合本身增删
        PropertyList.CollectionChanged += OnPropertyListCollectionChanged;

        // 2) 给现有的每个 item 也挂一次 PropertyChanged
        foreach (var item in PropertyList)
            SubscribeToItem(item);
    }

    // 当集合增删了，要给新加的 item 订阅，给移除的取消订阅
    private void OnPropertyListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
            foreach (InputItem oldItem in e.OldItems)
                oldItem.PropertyChanged -= OnItemPropertyChanged;

        if (e.NewItems is not null)
            foreach (InputItem newItem in e.NewItems)
                SubscribeToItem(newItem);

        // 触发一次 PropertyList 的通知，保证 Binding 再跑 Converter
        OnPropertyChanged(nameof(PropertyList));
    }

    private void SubscribeToItem(InputItem item)
    {
        // 要确保 InputItem 实现了 INotifyPropertyChanged，并在 Value 变化时抛出通知
        item.PropertyChanged += OnItemPropertyChanged;
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // 只要某个 InputItem.Value 变了，就让 Binding 再跑一次 Converter
        // 或者你也可以更精准地 OnPropertyChanged(nameof(PropertyList)) + nameof(SomeOther)
        OnPropertyChanged(nameof(PropertyList));
    }
}