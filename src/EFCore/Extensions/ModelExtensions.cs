// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Extension methods for <see cref="IModel" />.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        ///     Gets the entity that maps the given entity class. Returns <see langword="null" /> if no entity type with
        ///     the given CLR type is found or the given CLR type is being used by shared type entity type
        ///     or the entity type has a defining navigation.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="type"> The type to find the corresponding entity type for. </param>
        /// <returns> The entity type, or <see langword="null" /> if none if found. </returns>
        [DebuggerStepThrough]
        public static IEntityType FindEntityType([NotNull] this IModel model, [NotNull] Type type)
            => ((Model)Check.NotNull(model, nameof(model))).FindEntityType(Check.NotNull(type, nameof(type)));

        /// <summary>
        ///     Gets the entity that maps the given entity class, where the class may be a proxy derived from the
        ///     actual entity type. Returns <see langword="null" /> if no entity type with the given CLR type is found
        ///     or the given CLR type is being used by shared type entity type
        ///     or the entity type has a defining navigation.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="type"> The type to find the corresponding entity type for. </param>
        /// <returns> The entity type, or <see langword="null" /> if none if found. </returns>
        public static IEntityType FindRuntimeEntityType([NotNull] this IModel model, [NotNull] Type type)
        {
            Check.NotNull(type, nameof(type));
            var realModel = (Model)Check.NotNull(model, nameof(model));

            return realModel.FindEntityType(type)
                ?? (type.BaseType == null
                    ? null
                    : realModel.FindEntityType(type.BaseType));
        }

        /// <summary>
        ///     Gets the entity type for the given type, defining navigation name
        ///     and the defining entity type. Returns <see langword="null" /> if no matching entity type is found.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="type"> The type of the entity type to find. </param>
        /// <param name="definingNavigationName"> The defining navigation of the entity type to find. </param>
        /// <param name="definingEntityType"> The defining entity type of the entity type to find. </param>
        /// <returns> The entity type, or <see langword="null" /> if none are found. </returns>
        [DebuggerStepThrough]
        public static IEntityType FindEntityType(
            [NotNull] this IModel model,
            [NotNull] Type type,
            [NotNull] string definingNavigationName,
            [NotNull] IEntityType definingEntityType)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(type, nameof(type));
            Check.NotNull(definingNavigationName, nameof(definingNavigationName));
            Check.NotNull(definingEntityType, nameof(definingEntityType));

            return model.AsModel().FindEntityType(
                type,
                definingNavigationName,
                definingEntityType.AsEntityType());
        }

        /// <summary>
        ///     Gets the entity types matching the given type.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="type"> The type of the entity type to find. </param>
        /// <returns> The entity types found. </returns>
        [DebuggerStepThrough]
        public static IReadOnlyCollection<IEntityType> GetEntityTypes([NotNull] this IModel model, [NotNull] Type type)
            => model.AsModel().GetEntityTypes(type);

        /// <summary>
        ///     Gets the entity types matching the given name.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="name"> The name of the entity type to find. </param>
        /// <returns> The entity types found. </returns>
        [DebuggerStepThrough]
        public static IReadOnlyCollection<IEntityType> GetEntityTypes([NotNull] this IModel model, [NotNull] string name)
            => model.AsModel().GetEntityTypes(name);

        /// <summary>
        ///     Gets a value indicating whether the model contains a corresponding entity type with a defining navigation.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="type"> The type used to find an entity type a defining navigation. </param>
        /// <returns> <see langword="true" /> if the model contains a corresponding entity type with a defining navigation. </returns>
        [DebuggerStepThrough]
        public static bool HasEntityTypeWithDefiningNavigation([NotNull] this IModel model, [NotNull] Type type)
            => Check.NotNull(model, nameof(model)).AsModel()
                .HasEntityTypeWithDefiningNavigation(Check.NotNull(type, nameof(type)));

        /// <summary>
        ///     Gets a value indicating whether the model contains a corresponding entity type with a defining navigation.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="name"> The name used to find an entity type with a defining navigation. </param>
        /// <returns> <see langword="true" /> if the model contains a corresponding entity type with a defining navigation. </returns>
        [DebuggerStepThrough]
        public static bool HasEntityTypeWithDefiningNavigation([NotNull] this IModel model, [NotNull] string name)
            => Check.NotNull(model, nameof(model)).AsModel()
                .HasEntityTypeWithDefiningNavigation(Check.NotNull(name, nameof(name)));

        /// <summary>
        ///     Gets whether the CLR type is used by shared type entities in the model.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="type"> The CLR type. </param>
        /// <returns> Whether the CLR type is used by shared type entities in the model. </returns>
        [DebuggerStepThrough]
        public static bool IsShared([NotNull] this IModel model, [NotNull] Type type)
            => Check.NotNull(model, nameof(model)).AsModel().IsShared(Check.NotNull(type, nameof(type)));

        /// <summary>
        ///     Gets the default change tracking strategy being used for entities in the model. This strategy indicates how the
        ///     context detects changes to properties for an instance of an entity type.
        /// </summary>
        /// <param name="model"> The model to get the default change tracking strategy for. </param>
        /// <returns> The change tracking strategy. </returns>
        [DebuggerStepThrough]
        public static ChangeTrackingStrategy GetChangeTrackingStrategy([NotNull] this IModel model)
            => (ChangeTrackingStrategy?)Check.NotNull(model, nameof(model))[CoreAnnotationNames.ChangeTrackingStrategy]
                ?? ChangeTrackingStrategy.Snapshot;

        /// <summary>
        ///     <para>
        ///         Gets the <see cref="PropertyAccessMode" /> being used for properties of entity types in this model.
        ///     </para>
        ///     <para>
        ///         Note that individual entity types can override this access mode, and individual properties of
        ///         entity types can override the access mode set on the entity type. The value returned here will
        ///         be used for any property for which no override has been specified.
        ///     </para>
        /// </summary>
        /// <param name="model"> The model to get the access mode for. </param>
        /// <returns> The access mode being used, or null if the default access mode is being used. </returns>
        [DebuggerStepThrough]
        public static PropertyAccessMode GetPropertyAccessMode([NotNull] this IModel model)
            => (PropertyAccessMode?)Check.NotNull(model, nameof(model))[CoreAnnotationNames.PropertyAccessMode]
                ?? PropertyAccessMode.PreferField;

        /// <summary>
        ///     Gets the EF Core assembly version used to build this model
        /// </summary>
        /// <param name="model"> The model to get the version for. </param>
        public static string GetProductVersion([NotNull] this IModel model)
            => model[CoreAnnotationNames.ProductVersion] as string;

        /// <summary>
        ///     Gets a value indicating whether the given MethodInfo reprensent an indexer access.
        /// </summary>
        /// <param name="model"> The model to use. </param>
        /// <param name="methodInfo"> The MethodInfo to check for. </param>
        public static bool IsIndexerMethod([NotNull] this IModel model, [NotNull] MethodInfo methodInfo)
             => !methodInfo.IsStatic
                && methodInfo.IsSpecialName
                && model.AsModel().FindIndexerPropertyInfo(methodInfo.DeclaringType) is PropertyInfo indexerProperty
                && (methodInfo == indexerProperty.GetMethod || methodInfo == indexerProperty.SetMethod);

        /// <summary>
        ///     <para>
        ///         Creates a human-readable representation of the given metadata.
        ///     </para>
        ///     <para>
        ///         Warning: Do not rely on the format of the returned string.
        ///         It is designed for debugging only and may change arbitrarily between releases.
        ///     </para>
        /// </summary>
        /// <param name="model"> The metadata item. </param>
        /// <param name="options"> Options for generating the string. </param>
        /// <param name="indent"> The number of indent spaces to use before each new line. </param>
        /// <returns> A human-readable representation. </returns>
        public static string ToDebugString(
            [NotNull] this IModel model,
            MetadataDebugStringOptions options,
            int indent = 0)
        {
            var builder = new StringBuilder();
            var indentString = new string(' ', indent);

            builder.Append(indentString).Append("Model: ");

            if (model.GetPropertyAccessMode() != PropertyAccessMode.PreferField)
            {
                builder.Append(" PropertyAccessMode.").Append(model.GetPropertyAccessMode());
            }

            if (model.GetChangeTrackingStrategy() != ChangeTrackingStrategy.Snapshot)
            {
                builder.Append(" ChangeTrackingStrategy.").Append(model.GetChangeTrackingStrategy());
            }

            foreach (var entityType in model.GetEntityTypes())
            {
                builder.AppendLine().Append(entityType.ToDebugString(options, indent + 2));
            }

            if ((options & MetadataDebugStringOptions.IncludeAnnotations) != 0)
            {
                builder.Append(model.AnnotationsToDebugString(indent));
            }

            return builder.ToString();
        }
    }
}
