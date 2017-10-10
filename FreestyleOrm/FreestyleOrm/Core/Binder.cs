using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace FreestyleOrm.Core
{
    internal class Binder
    {
        public void Bind(Row row, object entity)
        {
            if (entity == null) return;

            Dictionary<string, PropertyInfo> propertyMap = entity.GetType().GetPropertyMap(BindingFlags.SetProperty, PropertyTypeFilters.IgonreClass);

            Dictionary<string, string> formatedPropertyNameMap = GetFormatedPropertyNameMap(row);

            foreach (var formatedPropertyName in formatedPropertyNameMap)
            {
                if (propertyMap.TryGetValue(formatedPropertyName.Key, out PropertyInfo property)) property.Set(entity, row[formatedPropertyName.Value]);
            }
        }

        private Dictionary<string, string> GetFormatedPropertyNameMap(Row row)
        {
            List<string> columnWithPrefixList = new List<string>();
            Dictionary<string, string> map = new Dictionary<string, string>();

            foreach (var column in row.Columns)
            {
                if (row.StartWithPrefix(column))
                {
                    columnWithPrefixList.Add(column);
                    continue;
                }

                string columnWithoutPrefix = row.GetColumnWithoutPrefix(column);
                string propertyName = row.FormatPropertyName(columnWithoutPrefix);

                map[propertyName] = column;
            }

            foreach (var column in columnWithPrefixList)
            {
                string columnWithoutPrefix = row.GetColumnWithoutPrefix(column);
                string propertyName = row.FormatPropertyName(columnWithoutPrefix);

                map[propertyName] = column;
            }

            return map;
        }

        public void Bind(object entity, IRootEntityNode rootEntityNode, Row row)
        {
            if (entity == null) return;

            List<object> relationEntities = new List<object>();

            SetRelationEntitis(entity, rootEntityNode, relationEntities);

            if (entity != rootEntityNode.Entity)
            {
                Dictionary<string, PropertyInfo> map = entity.GetType().GetPropertyMap(BindingFlags.GetProperty, PropertyTypeFilters.OnlyClass);

                foreach (var entry in map)
                {
                    if (entry.Value.PropertyType.IsList()) continue;

                    relationEntities.Add(entry.Value.Get(entity));
                }

                relationEntities.Add(entity);
            }

            SetRow(relationEntities, row);
        }

        private void SetRelationEntitis(object entity, IRootEntityNode rootEntityNode, List<object> relationEntities)
        {
            SetRelationEntitis(entity, rootEntityNode.Entity, relationEntities);

            if (rootEntityNode.Child != null) SetRelationEntitis(entity, rootEntityNode.Child, relationEntities);
        }

        private void SetRelationEntitis(object entity, object rootEntity, List<object> relationEntities)
        {
            if (relationEntities.Count == 0) relationEntities.Add(rootEntity);

            Dictionary<string, PropertyInfo> map = rootEntity.GetType().GetPropertyMap(BindingFlags.GetProperty, PropertyTypeFilters.OnlyClass);

            foreach (var property in map.Values)
            {
                if (property.PropertyType.IsList()) continue;
                
                object childEntity = property.Get(rootEntity);

                if (childEntity == null) continue;
                if (childEntity == entity) continue;
                if (relationEntities.Any(x => x == childEntity)) continue;

                relationEntities.Add(childEntity);

                SetRelationEntitis(entity, childEntity, relationEntities);
            }
        }

        private void SetRow(List<object> relationEntities, Row row)
        {
            foreach (var relationEntity in relationEntities)
            {
                Dictionary<string, PropertyInfo> propertyMap = relationEntity.GetType().GetPropertyMap(BindingFlags.GetProperty, PropertyTypeFilters.IgonreClass);
                Dictionary<string, string> formatedPropertyNameMap = GetFormatedPropertyNameMap(row);

                foreach (var formatedPropertyName in formatedPropertyNameMap)
                {
                    if (propertyMap.TryGetValue(formatedPropertyName.Key, out PropertyInfo property)) row[formatedPropertyName.Value] = property.Get(relationEntity);
                }
            }
        }
    }
}