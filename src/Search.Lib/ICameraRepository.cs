using System.Collections.Generic;
using System.Threading.Tasks;

namespace Search.Lib
{
    public interface ICameraRepository
    {
        Task Add(Camera camera);

        Task Remove(Camera camera);

        Camera this[int number] { get; }

        Camera this[string name] { get; }

        Task<Camera> Get(int number);

        Task<Camera> Get(string name);

        Task<IEnumerable<Camera>> Search(string partialName);

        ICameraRepositorySnapshot GetSnapshot();
    }

    public interface ICameraRepositorySnapshot : IEnumerable<Camera> {
        
    }
}
