using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace FreestyleOrm.Core
{
    internal class MapOptions
    {
        public MapOptions(IQueryDefine queryDefine, Type rootEntityType, Type entityType, string expressionPath, PropertyInfo property, bool isToMany)
        {
            _queryDefine = queryDefine;

            ExpressionPath = expressionPath;
            EntityType = entityType;
            IsToMany = isToMany;            
            FormatPropertyName = column => _queryDefine.GetFormatPropertyName(rootEntityType, entityType, column);
            AutoId = _queryDefine.GetAutoId(rootEntityType, entityType);
            CreateEntity = () => _queryDefine.CreateEntity(rootEntityType, entityType);
            Table = _queryDefine.GetTable(rootEntityType, entityType);

            RelationId relationId = new RelationId();
            queryDefine.SetRelationId(rootEntityType, entityType, relationId);

            RelationIdColumn = relationId.RelationIdColumn;
            RelationEntityPath = relationId.RelationEntityPath;

            OptimisticLock optimisticLock = new OptimisticLock();
            queryDefine.SetOptimisticLock(rootEntityType, entityType, optimisticLock);

            RowVersionColumn = optimisticLock.RowVersionColumn;
            NewRowVersion = optimisticLock.NewRowVersion;

            Binder binder = new Binder();

            SetEntity = binder.Bind;
            SetRow = binder.Bind;
        }

        private IQueryDefine _queryDefine;

        public string ExpressionPath { get; set; }
        public Type EntityType { get; set; }
        public PropertyInfo Property { get; set; }
        public string[] ExpressionSections => ExpressionPath.Split('.');
        public bool IsToMany { get; set; }
        public string UniqueKeys { get; set; }
        public string IncludePrefix { get; set; }
        public Refer Refer { get; set; }
        public Action<Row, object> SetEntity { get; set; }
        public Action<object, IRootEntityNode, Row> SetRow { get; set; }
        public Func<string, string> FormatPropertyName { get; set; }
        public bool AutoId { get; set; }
        public string Table { get; set; }
        public string RelationIdColumn { get; set; }
        public string RelationEntityPath { get; set; }
        public Func<object> CreateEntity { get; set; }
        public string RowVersionColumn { get; set; }
        public Func<object, object> NewRowVersion { get; set; }
    }
}
