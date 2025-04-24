using Contoso.Store.Entities;

namespace Contoso.Domain.Interfaces
{
    public interface IIncident
    {
        List<Incident> GetPaged(int page, int pageSize, out int totalItems);
        List<Incident> GetAll();
        Incident? GetById(Guid id);
    }
}
