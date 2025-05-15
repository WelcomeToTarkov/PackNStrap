using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using EFT.InventoryLogic;
using PackNStrap.Core.Templates;
using SPT;
namespace PackNStrap.Core.Items;

public class CustomContainerItemClass : SearchableItemItemClass
{
	public CustomContainerItemClass(string id, CustomContainerTemplateClass template)
		: base(id, template)
	{
		if (!string.IsNullOrEmpty(template.CustomLayoutName))
		{
			this.Components.Add(new GridLayoutComponent(this, template));
			this.Components.Add(this.Tag = new TagComponent(this));
		}
	}

	public override IEnumerable<EItemInfoButton> ItemInteractionButtons
	{
		get
		{
			yield return EItemInfoButton.Open;
			yield return EItemInfoButton.Tag;
			if (!string.IsNullOrEmpty(this.Tag.Name))
			{
				yield return EItemInfoButton.ResetTag;
			}
		}
	}
	[GAttribute23]
	public readonly TagComponent Tag;
}
