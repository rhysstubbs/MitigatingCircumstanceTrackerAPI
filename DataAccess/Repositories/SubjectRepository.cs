using MCT.DataAccess.DataTransferObjects;
using MCT.DataAccess.Models;
using System;

namespace MCT.DataAccess.Repositories
{
    public class SubjectRepository : ISubjectRepository, IDisposable
    {
        private EFContext context;
        private GenericRepository<Subject> subjectRepo = null;
        private bool disposed = false;

        public SubjectRepository(EFContext context)
        {
            this.context = context;
        }

        public GenericRepository<Subject> SubjectRepo
        {
            get
            {
                if (this.subjectRepo == null)
                {
                    this.subjectRepo = new GenericRepository<Subject>(this.context);
                }
                return this.subjectRepo;
            }
        }

        public SubjectDTO Get(int subjectId)
        {
            var request = this.subjectRepo.GetByID(subjectId);
            if (request == null)
            {
                return null;
            }

            return new SubjectDTO(request);
        }

        public SubjectDTO Insert(SubjectDTO subjectDTO)
        {
            var request = subjectDTO.ToDbType();

            var result = this.subjectRepo.Insert(request);
            if (result == null)
            {
                return null;
            }

            return new SubjectDTO(result);
        }

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