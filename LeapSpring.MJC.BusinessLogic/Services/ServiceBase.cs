using LeapSpring.MJC.Data.Repository;

namespace LeapSpring.MJC.BusinessLogic.Services
{
    public abstract class ServiceBase
    {
        protected readonly IRepository Repository;

        public ServiceBase(IRepository repository)
        {
            if (Repository != null)
                return;

            Repository = repository;
        }
    }
}
