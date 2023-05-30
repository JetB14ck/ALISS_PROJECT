using ALISS.Process.Batch.DataAccess;
using ALISS.Process.Batch.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALISS.Process.Batch.DataService
{
    public class DataMethod
    {

        public T GET_DATA<T>(Func<T, bool> query) where T : class
        {
            var obj = new object();

            using (var _db = new ProcessContext())
            {
                using (var scope = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (typeof(T) == typeof(TRProcessRequest))
                        {

                        }
                        else if (typeof(T) == typeof(TRProcessRequest))
                        {

                        }

                        scope.Commit();
                    }
                    catch(Exception ex)
                    {
                        scope.Rollback();
                    }
                    finally
                    {
                        scope.Dispose();
                        _db.Dispose();
                    }
                }
            }

            return null;
        }

        public List<T> GET_LIST<T>(Func<T, bool> query)
        {

            return null;
        }

        public T INSERT<T>(T dataModel)
        {

            using (var _db = new ProcessContext())
            {
                using (var scope = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (typeof(T) == typeof(TRProcessRequest))
                        {
                            
                        }
                        else if (typeof(T) == typeof(TRProcessRequest))
                        {

                        }

                        _db.SaveChanges();
                        scope.Commit();
                    }
                    catch (Exception ex)
                    {
                        scope.Rollback();
                    }
                    finally
                    {
                        scope.Dispose();
                        _db.Dispose();
                    }
                }
            }

            return dataModel;
        }

        //public T UPDATE<T>(T dataModel)
        //{

        //    return (T)null;
        //}

        //public T DELETE<T>(T dataModel)
        //{

        //    return null;
        //}

    }
}