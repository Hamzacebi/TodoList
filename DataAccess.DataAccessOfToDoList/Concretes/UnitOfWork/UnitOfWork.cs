﻿using System;
using System.Text;
using System.Collections.Generic;

#region Global Usings
using Microsoft.EntityFrameworkCore;
using Commons.CommonOfToDoList.Constants;
using Helpers.HelperOfToDoList.Extensions;
using Models.EntitiesOfProjects.EntitiesOfToDoList.DatabaseContext;
#endregion Global Usings

namespace DataAccess.DataAccessOfToDoList.Concretes.UnitOfWork
{
    #region Internal Project Usings
    using Abstracts.UnitOfWork;
    using RepositoriesOfEntities;
    using Abstracts.RepositoriesOfEntities;
    using Microsoft.EntityFrameworkCore.Storage;

    #endregion Internal Project Usings

    public class UnitOfWork : IUnitOfWork
    {
        #region Global Properties

        private bool disposedValue;

        private readonly DbContext DbContext;
        private IDbContextTransaction DbContextTransaction;

        private readonly object lockObjectForRepositoryOfUser;
        private readonly object lockObjectForRepositoryOfCategory;
        private readonly object lockObjectForRepositoryOfThingToDo;
        private readonly object lockObjectForRepositoryAssignmentHistoryOfTask;

        private IRepositoryOfUser repositoryOfUser;
        private IRepositoryOfCategory repositoryOfCategory;
        private IRepositoryOfThingToDo repositoryOfThingToDo;
        private IRepositoryAssignmentHistoryOfTask repositoryAssignmentHistoryOfTask;

        #endregion Global Properties


        #region Constructor(s)

        public UnitOfWork(DbContext dbContext)
        {
            this.DbContext = dbContext ?? throw new ArgumentNullException(message: ConstantsOfErrors.ArgumentNullExceptionMessageForDbContext,
                                                                         innerException: null);

            //DbContext nesnesi bos degilse islemler yapilsin
            this.disposedValue = default(bool);

            this.lockObjectForRepositoryOfUser = new object();
            this.lockObjectForRepositoryOfCategory = new object();
            this.lockObjectForRepositoryOfThingToDo = new object();
            this.lockObjectForRepositoryAssignmentHistoryOfTask = new object();
        }

        #endregion Constructor(s)


        #region SaveChanges Function

        int IUnitOfWork.SaveChanges()
        {
            return ExtensionsOfFunction.TryCatch<int>(
                    function: () =>
                    {
                        return this.DbContext.SaveChanges();
                    }
                );
        }

        #endregion SaveChanges Function


        #region Transaction Functions

        void IUnitOfWork.BeginTransaction()
        {
            if (this.DbContext != null)
            {
                ExtensionsOfAction.TryCatch(
                    action: () =>
                    {
                        this.DbContextTransaction = this.DbContext.Database.BeginTransaction(isolationLevel: System.Data.IsolationLevel.ReadUncommitted);
                    },
                    catchAndDo: (Exception exception) => throw exception
                );
            }
            else
            {
                throw new ArgumentNullException(message: ConstantsOfErrors.ArgumentNullExceptionMessageForDbContext,
                                            innerException: null);
            }
        }

        void IUnitOfWork.CommitTransaction()
        {
            if (this.DbContextTransaction != null)
            {
                ExtensionsOfAction.TryCatch(
                        action: () => this.DbContextTransaction.Commit(),
                        catchAndDo: (Exception exception) =>
                        {
                            this.DbContextTransaction.Rollback();
                            throw exception;
                        },
                        finallyDo: () => { this.DbContextTransaction.Dispose(); this.DbContext.Dispose(); }
                    );
            }
            else
            {
                throw new ArgumentNullException(message: ConstantsOfErrors.ArgumentNullExceptionMessageForDbContextTransaction,
                                            innerException: null);
            }
        }

        void IUnitOfWork.RollbackTransaction()
        {
            if (this.DbContextTransaction != null)
            {
                ExtensionsOfAction.TryCatch(
                   action: () => this.DbContextTransaction.Rollback(),
                   catchAndDo: (Exception exception) =>
                   {
                       this.DbContextTransaction.Rollback();
                       throw exception;
                   },
                   finallyDo: () => { this.DbContextTransaction.Dispose(); this.DbContext.Dispose(); }
               );
            }
            else
            {
                throw new ArgumentNullException(message: ConstantsOfErrors.ArgumentNullExceptionMessageForDbContextTransaction,
                                           innerException: null);
            }
        }

        #endregion Transaction Functions


        #region Repositories Properties

        IRepositoryOfUser IUnitOfWork.RepositoryOfUser
        {
            get
            {
                if (this.repositoryOfUser == null)
                {
                    lock (this.lockObjectForRepositoryOfUser)
                    {
                        if (this.repositoryOfUser == null)
                        {
                            this.repositoryOfUser = new RepositoryOfUser(dbContext: this.DbContext, inWhichDbContext: typeof(ToDoListDbContext));
                        }
                    }
                }
                return this.repositoryOfUser;
            }
        }

        IRepositoryOfCategory IUnitOfWork.RepositoryOfCategory
        {
            get
            {
                if (this.repositoryOfCategory == null)
                {
                    lock (this.lockObjectForRepositoryOfCategory)
                    {
                        if (this.repositoryOfCategory == null)
                        {
                            this.repositoryOfCategory = new RepositoryOfCategory(dbContext: this.DbContext, inWhichDbContext: typeof(ToDoListDbContext));
                        }
                    }
                }
                return this.repositoryOfCategory;
            }
        }

        IRepositoryOfThingToDo IUnitOfWork.RepositoryOfThingToDo
        {
            get
            {
                if (this.repositoryOfThingToDo == null)
                {
                    lock (this.lockObjectForRepositoryOfThingToDo)
                    {
                        if (this.repositoryOfThingToDo == null)
                        {
                            this.repositoryOfThingToDo = new RepositoryOfThingToDo(dbContext: this.DbContext, inWhichDbContext: typeof(ToDoListDbContext));
                        }
                    }
                }
                return this.repositoryOfThingToDo;
            }
        }

        IRepositoryAssignmentHistoryOfTask IUnitOfWork.RepositoryAssignmentHistoryOfTask
        {
            get
            {
                if (this.repositoryAssignmentHistoryOfTask == null)
                {
                    lock (this.lockObjectForRepositoryAssignmentHistoryOfTask)
                    {
                        if (this.repositoryAssignmentHistoryOfTask == null)
                        {
                            this.repositoryAssignmentHistoryOfTask = new RepositoryAssignmentHistoryOfTask(dbContext: this.DbContext, inWhichDbContext: typeof(ToDoListDbContext));
                        }
                    }
                }
                return this.repositoryAssignmentHistoryOfTask;
            }
        }
        #endregion Repositories Properties


        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.DbContextTransaction != null)
                    {
                        this.DbContextTransaction.Dispose();
                    }
                    this.DbContext.Dispose();
                }
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion  IDisposable Support
    }
}
