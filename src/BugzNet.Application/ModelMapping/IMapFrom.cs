using AutoMapper;

namespace BugzNet.Application.ModelMapping
{
    public interface IMapFrom<TEntity>
    {
        void CreateMappings(Profile configuration);

    }
}
