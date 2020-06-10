namespace Routine.Api.Services
{
    public interface IPropertyChckerService
    {
        bool TypeHasProperties<TSource>(string fields);
    }
}