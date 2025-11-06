using System;
using System.Collections.Generic;
using System.Reflection;
using EFT.InventoryLogic;
using JetBrains.Annotations;
using PackNStrap.Core.Items;
using PackNStrap.Core.Templates;

namespace PackNStrap.Helpers;

public abstract class NewTemplateIdToObjectIdClass
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
}