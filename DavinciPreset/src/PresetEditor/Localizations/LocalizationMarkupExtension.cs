namespace PresetEditor.Localizations;

[ContentProperty(nameof(Name))]
public class LocalizationMarkupExtension : BindableObject, IMarkupExtension<BindingBase> {
    public static BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(LocalizationMarkupExtension), null,
        propertyChanged: (b, o, n) => ((LocalizationMarkupExtension)b).OnTranslatedNameChanged());
    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static BindableProperty X0Property = BindableProperty.Create(nameof(X0), typeof(object), typeof(LocalizationMarkupExtension), null,
        propertyChanged: (b, o, n) => ((LocalizationMarkupExtension)b).OnTranslatedNameChanged());
    public object X0
    {
        get => GetValue(X0Property);
        set => SetValue(X0Property, value);
    }

    public static BindableProperty X1Property = BindableProperty.Create(nameof(X1), typeof(object), typeof(LocalizationMarkupExtension), null,
        propertyChanged: (b, o, n) => ((LocalizationMarkupExtension)b).OnTranslatedNameChanged());
    public object X1
    {
        get => GetValue(X1Property);
        set => SetValue(X1Property, value);
    }

    public string? TranslatedName
        => (Name is string name && LocalizationResourceManager.Instance[name] is string translatedName)
            ? String.Format(translatedName, new object?[] { X0, X1 })
            : null;

    public void OnTranslatedNameChanged() => OnPropertyChanged(nameof(TranslatedName));

    public LocalizationMarkupExtension()
    {
        LocalizationResourceManager.Instance.PropertyChanged += (s, e) => OnTranslatedNameChanged();
    }

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
        => BindingBase.Create<LocalizationMarkupExtension, string?>(
            static source => source.TranslatedName,
            mode: BindingMode.OneWay,
            source: this
        );

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        => ProvideValue(serviceProvider);
}