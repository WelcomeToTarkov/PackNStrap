using System;
using System.Collections.Generic;
using System.Reflection;
using EFT.InventoryLogic;
using PackNStrap.Core.Items;
using PackNStrap.Core.Templates;

namespace PackNStrap.Helpers;

public class NewTemplateIdToObjectIdClass
{
    
    public static readonly List<TemplateIdToObjectType> CustomMappings =
    [
        // Add CustomContainer Template
        new(
            "680fce2ec7b9b222270f074c", // Template ID
            null,   // Item type
            typeof(CustomContainerTemplateClass),         // Template type
            null // Constructor
        ),
        // Add CustomContainer Item
        new(
            "680fd1dae5044e670a092e16", // Template ID
            typeof(CustomContainerItemClass),   // Item type
            typeof(CustomContainerTemplateClass),         // Template type
            (id, template) => new CustomContainerItemClass(id, (CustomContainerTemplateClass)template) // Constructor
        ),
        // Add CustomSecureContainer Item
        new(
            "68154651f849fb4e7d816738", // Template ID
            typeof(CustomSecureContainerClass),   // Item type
            typeof(CustomContainerTemplateClass),         // Template type
            (id, template) => new CustomSecureContainerClass(id, (CustomContainerTemplateClass)template) // Constructor
        ),
        // Add CustomBelt Item
        new(
            "6815465859b8c6ff13f94026", // Template ID
            typeof(CustomBeltItemClass),   // Item type
            typeof(CustomContainerTemplateClass),         // Template type
            (id, template) => new CustomBeltItemClass(id, (CustomContainerTemplateClass)template) // Constructor
        ),
    ];
        public static void AddNewTemplateIdToObjectMapping(List<TemplateIdToObjectType> mappings)
    {
        Type templateIdToObjectMappingsClass = typeof(TemplateIdToObjectMappingsClass);

        foreach (var mapping in mappings)
        {
            // Add to TypeTable
            FieldInfo typeTableField = templateIdToObjectMappingsClass.GetField("TypeTable", BindingFlags.Public | BindingFlags.Static);
            if (typeTableField != null)
            {
                var typeTable = (Dictionary<string, Type>)typeTableField.GetValue(null);
                if (!typeTable.ContainsKey(mapping.TemplateId) && mapping.ItemType != null)
                {
                    typeTable.Add(mapping.TemplateId, mapping.ItemType);
#if DEBUG
                    Console.WriteLine($"Added {mapping.ItemType.Name} to TypeTable.");
#endif
                }
            }

            // Add to TemplateTypeTable
            FieldInfo templateTypeTableField = templateIdToObjectMappingsClass.GetField("TemplateTypeTable", BindingFlags.Public | BindingFlags.Static);
            if (templateTypeTableField != null)
            {
                var templateTypeTable = (Dictionary<string, Type>)templateTypeTableField.GetValue(null);
                if (!templateTypeTable.ContainsKey(mapping.TemplateId))
                {
                    templateTypeTable.Add(mapping.TemplateId, mapping.TemplateType);
#if DEBUG
                    Console.WriteLine($"Added {mapping.TemplateType.Name} to TemplateTypeTable.");
#endif
                }
            }

            // Add to ItemConstructors only if ItemType is not null
            if (mapping.ItemType != null)
            {
                FieldInfo itemConstructorsField = templateIdToObjectMappingsClass.GetField("ItemConstructors", BindingFlags.Public | BindingFlags.Static);
                if (itemConstructorsField != null)
                {
                    var itemConstructors = (Dictionary<string, Func<string, object, Item>>)itemConstructorsField.GetValue(null);
                    if (!itemConstructors.ContainsKey(mapping.TemplateId))
                    {
                        itemConstructors.Add(mapping.TemplateId, mapping.Constructor);
#if DEBUG
                        Console.WriteLine($"Added {mapping.ItemType.Name} constructor to ItemConstructors.");
#endif
                    }
                }
            }
        }
    }

    public class TemplateIdToObjectType(
        string templateId,
        Type itemType,
        Type templateType,
        Func<string, object, Item> constructor)
    {
        public string TemplateId { get; set; } = templateId;
        public Type ItemType { get; set; } = itemType; // Can be null for templates only
        public Type TemplateType { get; set; } = templateType;
        public Func<string, object, Item> Constructor { get; set; } = constructor; // Can also be null for templates only
    }
}