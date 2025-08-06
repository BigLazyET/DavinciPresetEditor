namespace ResponsiveMaui.Extensions;

public class BreakpointBehavior<T> : Behavior<T> where T : VisualElement
{
    private Page? _parentPage;
    private VisualElement? _bindableElement;
    
    protected override void OnAttachedTo(T element)
    {
        _bindableElement = element;
        
        if (element is Page page)
            page.SizeChanged += PageSizeChanged;
        else
            element.Loaded += ElementOnLoaded;

        base.OnAttachedTo(element);
    }

    private void ElementOnLoaded(object? sender, EventArgs e)
    {
        if (sender is not T element) return;
        _parentPage = element.GetParentPage();
        if (_parentPage == null) return;
        _parentPage.SizeChanged += PageSizeChanged;
    }

    protected override void OnDetachingFrom(T element)
    {
        base.OnDetachingFrom(element);
        if (element is Page page)
            page.SizeChanged -= PageSizeChanged;
        else
        {
            if (_parentPage == null) return;
            _parentPage.SizeChanged -= PageSizeChanged;
        }
    }

    private void PageSizeChanged(object? sender, EventArgs e)
    {
        if (sender is Page page && _bindableElement != null)
            VisualStateManager.GoToState(_bindableElement, ToState(page.Width));
    }

    private static string ToState(double width)
    {
        return width switch
        {
            >= 1400 => "ExtraExtraLarge",
            >= 1200 => "ExtraLarge",
            >= 992 => "Large",
            >= 768 => "Medium",
            >= 576 => "Small",
            _ => "ExtraSmall"
        };
    }
}