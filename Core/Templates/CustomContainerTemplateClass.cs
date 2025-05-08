namespace PackNStrap.Core.Templates;

public class CustomContainerTemplateClass : SearchableItemTemplateClass, GInterface365
{
	string GInterface365.LayoutName
	{
		get
		{
			return CustomLayoutName;
		}
	}

	public string CustomLayoutName;
}
