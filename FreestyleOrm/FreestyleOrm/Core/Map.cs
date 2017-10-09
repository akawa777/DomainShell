using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace FreestyleOrm.Core
{

    internal class Map<TRootEntity> : IMap<TRootEntity> where TRootEntity : class
    {
        public Map(IQueryDefine queryDefine)
        {
            _queryDefine = queryDefine;
        }

        private IQueryDefine _queryDefine;
        private List<MapOptions> _mapOptionsList = new List<MapOptions>();

        public MapOptions RootMapOptions => _mapOptionsList.FirstOrDefault(x => string.IsNullOrEmpty(x.ExpressionPath));
        public IEnumerable<MapOptions> MapOptionsListWithoutRoot => _mapOptionsList.Where(x => x != RootMapOptions).OrderBy(x => x.ExpressionSections.Length);

        public IMapOptions<TRootEntity, TRootEntity> To()
        {
            MapOptions<TRootEntity, TRootEntity> mapOptions = new MapOptions<TRootEntity, TRootEntity>(_queryDefine, x => x);

            _mapOptionsList.Add(mapOptions.GetMapOptions());

            return mapOptions;
        }

        public IMapOptions<TRootEntity, TEntity> ToMany<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> target) where TEntity : class
        {
            MapOptions<TRootEntity, TEntity> mapOptions = new MapOptions<TRootEntity, TEntity>(_queryDefine, target);

            _mapOptionsList.Add(mapOptions.GetMapOptions());

            return mapOptions;
        }

        public IMapOptions<TRootEntity, TEntity> ToOne<TEntity>(Expression<Func<TRootEntity, TEntity>> target) where TEntity : class
        {
            MapOptions<TRootEntity, TEntity> mapOptions = new MapOptions<TRootEntity, TEntity>(_queryDefine, target);

            _mapOptionsList.Add(mapOptions.GetMapOptions());

            return mapOptions;
        }
    }

    internal class MapOptions<TRootEntity, TEntity> : IMapOptions<TRootEntity, TEntity> where TRootEntity : class where TEntity : class
    {
        public MapOptions(IQueryDefine queryDefine, Expression<Func<TRootEntity, TEntity>> target)
        {            
            _mapOptions = new MapOptions(queryDefine, typeof(TRootEntity), typeof(TEntity), target.GetEntityPath(out PropertyInfo property), property, false);
        }

        public MapOptions(IQueryDefine queryDefine, Expression<Func<TRootEntity, IEnumerable<TEntity>>> target)
        {
            _mapOptions = new MapOptions(queryDefine, typeof(TRootEntity), typeof(TEntity), target.GetEntityPath(out PropertyInfo property), property, true);
        }

        private MapOptions _mapOptions;

        public MapOptions GetMapOptions() => _mapOptions;

        public IMapOptions<TRootEntity, TEntity> AutoId(bool autoId)
        {
            _mapOptions.AutoId = autoId;

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> CreateEntity(Func<TEntity> createEntity)
        {
            _mapOptions.CreateEntity = () => createEntity();

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> FormatPropertyName(Func<string, string> formatPropertyName)
        {
            _mapOptions.FormatPropertyName = formatPropertyName;

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> IncludePrefix(string prefix)
        {
            _mapOptions.IncludePrefix = prefix;

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> Refer(Refer refer)
        {
            _mapOptions.Refer = refer;

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> SetEntity(Action<IRow, TEntity> setEntity)
        {
            _mapOptions.SetEntity = (row, entity) => setEntity(row, (TEntity)entity);

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> SetRow(Action<TEntity, IRootEntityNode, IRow> setRow)
        {
            _mapOptions.SetRow = (entity, rootNode, row) => setRow((TEntity)entity, rootNode, row);

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> Table(Action<ITable<TRootEntity>> table)
        {
            table(new Table<TRootEntity>(_mapOptions.Table));

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> UniqueKeys(string columns)
        {
            _mapOptions.UniqueKeys = columns;

            return this;
        }

        public IMapOptions<TRootEntity, TEntity> OptimisticLock(string rowVersionColumn, Func<TEntity, object> newRowVersion = null)
        {
            _mapOptions.RowVersionColumn = rowVersionColumn;
            _mapOptions.NewRowVersion = entity => newRowVersion(entity as TEntity);

            return this;
        }

    }
}
