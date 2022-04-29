using System;
using System.Transactions;

namespace Audit_service.Repositories
{
    public interface ITransaction
    {
        void Commit();
        void Rollback();
    }
    class Transaction:ITransaction
    {
        protected UnitOfWork uow { get; private set; }
        protected TransactionScope ts { get; private set; }

        public Transaction(UnitOfWork u)
        {
            this.uow = u;
            this.ts = new TransactionScope();
        }

        public void Commit()
        {
            this.uow.Save();
            this.ts.Complete();
        }

        public void Rollback()
        {
        }
        public void Dispose()
        {
            if (this.ts != null)
            {
                (this.ts as IDisposable).Dispose();
                this.ts = null;
                this.uow = null;
            }
        }
    }
}
