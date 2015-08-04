using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.DAL
{
    public class BaseRepository<T> where T : class,new()
    {
        DbContext db = GetDbContext();

        /// <summary>
        /// 获取当前线程的 DbContext 实例
        /// </summary>
        /// <returns>DbContext 实例</returns>
        public static DbContext GetDbContext()
        {
            DbContext context = (DbContext)CallContext.GetData("DbContext");
            if (context == null)
            {
                context = new shtsEntities();
                CallContext.SetData("DbContext", context);
            }
            return context;
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <param name="whereLambda">获取条件</param>
        /// <returns>一个实体</returns>
        public T FindOne(Func<T, bool> whereLambda)
        {
            return db.Set<T>().Where<T>(whereLambda).FirstOrDefault();
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>所有实体</returns>
        public IQueryable<T> FindAll()
        {
            return db.Set<T>().Where<T>(o => true).AsQueryable();
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>所有实体</returns>
        public IQueryable<T> FindAll<S>(Func<T, bool> whereLambda, Func<T, S> orderByLambda, bool isASC)
        {
            var temp = db.Set<T>().Where(whereLambda);
            if (isASC)
            {
                temp = temp.OrderBy<T, S>(orderByLambda);
            }
            else
            {
                temp = temp.OrderByDescending(orderByLambda);
            }
            return temp.AsQueryable();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="S">实体类型</typeparam>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="total">总数</param>
        /// <param name="whereLambda">获取条件</param>
        /// <param name="orderByLambda">排序属性</param>
        /// <param name="isASC">是否升序</param>
        /// <returns>分页实体集</returns>
        public IQueryable<T> FindPage<S>(int pageSize, int pageIndex, out int total, Func<T, bool> whereLambda, Func<T, S> orderByLambda, bool isASC)
        {
            var temp = db.Set<T>().Where<T>(whereLambda);
            total = temp.Count();
            if (isASC)
            {
                temp = temp.OrderBy<T, S>(orderByLambda)
                    .Skip<T>((pageIndex - 1) * pageSize)
                    .Take<T>(pageSize);
            }
            else
            {
                temp = temp.OrderByDescending(orderByLambda)
                    .Skip<T>((pageIndex - 1) * pageSize)
                    .Take<T>(pageSize);
            }
            return temp.AsQueryable();
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否添加成功</returns>
        public bool AddEntitySave(T entity)
        {
            db.Set<T>().Add(entity);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateEntitySave(T entity)
        {
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteEntitySave(T entity)
        {
            db.Set<T>().Remove(entity);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 添加实体（未提交）
        /// </summary>
        /// <param name="entity">实体</param>
        public void AddEntity(T entity)
        {
            db.Set<T>().Add(entity);
        }

        /// <summary>
        /// 删除实体（未提交）
        /// </summary>
        /// <param name="entity">实体</param>
        public void DeleteEntity(T entity)
        {
            db.Set<T>().Remove(entity);
        }

        /// <summary>
        /// 提交操作
        /// </summary>
        /// <returns>受影响行数</returns>
        public int SaveChanges()
        {
            int result = 0;
            db.Configuration.ValidateOnSaveEnabled = false;
            result = db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = true;
            return result;
        }
    }
}
