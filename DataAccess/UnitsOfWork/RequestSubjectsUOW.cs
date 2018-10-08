using MCT.DataAccess.Models;
using MCT.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCT.DataAccess.UnitsOfWork
{
    class RequestSubjectsUOW
    {
        public class UnitOfWork : IDisposable
        {
            private EFContext context = new EFContext();
            private RequestRepository requestRepository;
            private SubjectRepository subjectRepository;

            public SubjectRepository SubjectRepository
            {
                get
                {

                    if (this.subjectRepository == null)
                    {
                        this.subjectRepository = new SubjectRepository(context);
                    }
                    return subjectRepository;
                }
            }

            public RequestRepository RequestRepository
            {
                get
                {

                    if (this.requestRepository == null)
                    {
                        this.requestRepository = new RequestRepository(context);
                    }
                    return requestRepository;
                }
            }

            public void Save()
            {
                context.SaveChanges();
            }

            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        context.Dispose();
                    }
                }
                this.disposed = true;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
