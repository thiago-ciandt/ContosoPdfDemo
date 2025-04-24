using Contoso.Domain.Interfaces;
using Contoso.Store.Entities;

namespace Contoso.Domain.Services
{
    public class IncidentService : IIncident
    {
        private readonly List<Incident> _incidents;

        public IncidentService(List<Incident> incidents)
        {
            _incidents = incidents;
        }

        public List<Incident> GetPaged(int page, int pageSize, out int totalItems)
        {
            totalItems = _incidents.Count;
            return _incidents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<Incident> GetAll()
        {
            return _incidents;
        }

        public Incident? GetById(Guid id)
        {
            return _incidents.FirstOrDefault(i => i.Id == id);
        }
    }
}
