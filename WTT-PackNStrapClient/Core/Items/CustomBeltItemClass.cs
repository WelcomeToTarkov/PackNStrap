using System.Collections.Generic;
using EFT.InventoryLogic;
using PackNStrap.Core.Templates;
namespace PackNStrap.Core.Items;

public class CustomBeltItemClass : SearchableItemItemClass
{
    public CustomBeltItemClass(string id, CustomContainerTemplateClass template)
        : base(id, template)
    {
        if (!string.IsNullOrEmpty(template.LayoutName))
        {
            Components.Add(new GridLayoutComponent(this, template));
        }
        Components.Add(Tag = new TagComponent(this));
    }

    public override IEnumerable<EItemInfoButton> ItemInteractionButtons
    {
        get
        {
            // Yield base buttons first
            foreach (var button in base.ItemInteractionButtons)
            {
                yield return button;
            }

            // Add container-specific buttons
            yield return EItemInfoButton.Tag;
            if (!string.IsNullOrEmpty(Tag.Name))
            {
                yield return EItemInfoButton.ResetTag;
            }
        }
    }
    [GAttribute23]
    public readonly TagComponent Tag;
}