//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Reflection;
//using System.Linq.Expressions;

//namespace FreestyleOrm.Core
//{
//    public class Table<TRootEntity> : ITable<TRootEntity> where TRootEntity : class
//    {
//        public Table(Table table)
//        {
//            _table = table;
//        }

//        public Table _table;

//        public ITable<TRootEntity> Name(string name)
//        {
//            _table.Name = name;

//            return this;
//        }

//        public ITable<TRootEntity> RelationId<TRelationEntity>(string relationIdColumn, Expression<Func<TRootEntity, TRelationEntity>> getRelationEntity) where TRelationEntity : class
//        {
//            _table.RelationIdColumn = relationIdColumn;
//            _table.RelationEntityPath = getRelationEntity.GetEntityPath(out PropertyInfo property);

//            return this;
//        }
//    }
//}
