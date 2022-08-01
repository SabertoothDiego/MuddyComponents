using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace SabertoothDiego.MuddyComponents;
/// <summary>
/// Represents an option of a select or multi-select. To be used inside MudSelect.
/// </summary>
public partial class MuddySelectItem<T> : MudBaseSelectItem, IDisposable
{
    private IMuddySelect? _parent;
    private IMuddyShadowSelect _shadowParent;
    private bool _isSelected;

    public string ItemId { get; protected internal set; } = $"_{Guid.NewGuid().ToString()[..8]}";

    /// <summary>
    /// A user-defined option that can be selected
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.FormComponent.Behavior)]
    public T Value { get; set; }

    /// <summary>
    /// Mirrors the MultiSelection status of the parent select
    /// </summary>
    public bool MultiSelection
    {
        get
        {
            if (MuddySelect == null)
                return false;
            return MuddySelect.MultiSelection;
        }
    }

    /// <summary>
    /// Selected state of the option. Only works if the parent is a mulit-select
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// The checkbox icon reflects the multi-select option's state
    /// </summary>
    protected string CheckBoxIcon
    {
        get
        {
            if (!MultiSelection)
                return null;
            return IsSelected ? Icons.Material.Filled.CheckBox : Icons.Material.Filled.CheckBoxOutlineBlank;
        }
    }

    protected string DisplayString
    {
        get
        {
            var converter = MuddySelect?.Converter;
            if (converter == null)
                return $"{Value}";
            return converter.Set(Value);
        }
    }

    /// <summary>
    /// The parent select component
    /// </summary>
    [CascadingParameter]
    protected internal IMuddySelect IMuddySelect
    {
        get => _parent;
        set
        {
            _parent = value;
            if (_parent == null)
                return;
            _parent.CheckGenericTypeMatch(this);
            if (MuddySelect == null)
                return;
            bool isSelected = MuddySelect.Add(this);
            if (_parent.MultiSelection)
            {
                MuddySelect.SelectionChangedFromOutside += OnUpdateSelectionStateFromOutside;
                InvokeAsync(() => OnUpdateSelectionStateFromOutside(MuddySelect.SelectedValues));
            }
            else
            {
                IsSelected = isSelected;
            }
        }
    }

    [CascadingParameter]
    internal IMuddyShadowSelect? IMuddyShadowSelect
    {
        get => _shadowParent;
        set
        {
            _shadowParent = value;
            ((MuddySelect<T>)_shadowParent)?.RegisterShadowItem(this);
        }
    }

    /// <summary>
    /// Select items with HideContent==true are only there to register their RenderFragment with the select but
    /// wont render and have no other purpose!
    /// </summary>
    [CascadingParameter(Name = "HideContent")]
    protected bool HideContent { get; set; }

    protected virtual string GetCssClasses() => new CssBuilder()
        .AddClass(Class)
        .Build();

    protected MuddySelect<T> MuddySelect => (MuddySelect<T>)IMuddySelect;

    protected void OnUpdateSelectionStateFromOutside(IEnumerable<T> selection)
    {
        if (selection == null)
            return;
        var old_is_selected = IsSelected;
        IsSelected = selection.Contains(Value);
        if (old_is_selected != IsSelected)
            InvokeAsync(StateHasChanged);
    }

    protected void OnClicked()
    {
        if (MultiSelection)
            IsSelected = !IsSelected;

        MuddySelect?.SelectOption(Value);
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        try
        {
            MuddySelect?.Remove(this);
            ((MuddySelect<T>)_shadowParent)?.UnregisterShadowItem(this);
        }
        catch (Exception) { }
    }
}