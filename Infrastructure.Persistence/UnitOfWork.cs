using System;
using Domain.Common.Persistence;
using NHibernate;

namespace Infrastructure.Persistence
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ISession _session;

        public UnitOfWorkFactory(ISession session)
        {
            _session = session;
        }

        public IUnitOfWork Create()
        {
            return new UnitOfWork(_session);
        }
    }

    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private ITransaction _transaction;

        public UnitOfWork(ISession session)
        {
            _session = session;
            _transaction = _session.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch (Exception)
            {
                _transaction.Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
            _transaction = null;

            _session.Clear();
        }
    }
}