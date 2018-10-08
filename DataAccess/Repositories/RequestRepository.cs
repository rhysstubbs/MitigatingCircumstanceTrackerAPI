using DataAccess.DataTransferObjects;
using DataAccess.Interfaces.Repositories;
using MCT.DataAccess.Models;
using System;

namespace MCT.DataAccess.Repositories
{
    public class RequestRepository : IRequestRepository, IDisposable
    {
        private EFContext context;
        private GenericRepository<Request> requestRepo = null;
        private bool disposed = false;

        public RequestRepository(EFContext context)
        {
            this.context = context;
        }

        public GenericRepository<Request> RequestRepo
        {
            get
            {
                if (this.requestRepo == null)
                {
                    this.requestRepo = new GenericRepository<Request>(context);
                }
                return this.requestRepo;
            }
        }

        public RequestDTO Insert(RequestDTO requestDTO)
        {
            var request = requestDTO.ToDbType();

            var result = RequestRepo.Insert(request);
            if (result == null)
            {
                return null;
            }

            return new RequestDTO(result);
        }

        public RequestDTO Get(int requestId)
        {
            var request = RequestRepo.GetByID(requestId);
            if (request == null)
            {
                return null;
            }

            return new RequestDTO(request);
        }

        public bool Delete(RequestDTO requestDTO)
        {
            try
            {
                RequestRepo.Delete(requestDTO);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool Delete(int id)
        {
            try
            {
                RequestRepo.Delete(id);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public RequestDTO Update(RequestDTO requestDTO)
        {
            var request = requestDTO.ToDbType();

            try
            {
                RequestRepo.Update(request);
            }
            catch (Exception)
            {
                return null;
            }

            return new RequestDTO(request);
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