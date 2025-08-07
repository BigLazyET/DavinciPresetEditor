namespace PresetEditor.Pages.Controls;

public class TemplatedContentView : ContentView
{
    public static readonly BindableProperty ContentTemplateSelectorProperty =
        BindableProperty.Create(nameof(ContentTemplateSelector), typeof(DataTemplateSelector), typeof(TemplatedContentView), propertyChanged: OnTemplateChanged);

    public DataTemplateSelector ContentTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
        set => SetValue(ContentTemplateSelectorProperty, value);
    }

    public static readonly BindableProperty TemplateContextProperty =
        BindableProperty.Create(nameof(TemplateContext), typeof(object), typeof(TemplatedContentView), propertyChanged: OnTemplateChanged);

    public object TemplateContext
    {
        get => GetValue(TemplateContextProperty);
        set => SetValue(TemplateContextProperty, value);
    }

    private static void OnTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not TemplatedContentView view) return;

        if (view.ContentTemplateSelector == null || view.TemplateContext == null) return;

        var dt = view.ContentTemplateSelector.SelectTemplate(view.TemplateContext, view);
        var content = (View)dt?.CreateContent();
        if (content == null) return;
        view.Content = content;
    }

    // public void SetContent()
    // {
    //     if (ContentTemplateSelector == null || TemplateContext == null) return;
    //     var dt = ContentTemplateSelector.SelectTemplate(TemplateContext, this);
    //     var content = (View)dt?.CreateContent();
    //     if (content == null) return;
    //     Content = content;
    // }
}