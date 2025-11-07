using System.Threading.Tasks;

namespace Arcube
{
    public interface IManager
    {
        /// <summary>
        /// use this to register events
        /// </summary>
        /// <returns></returns>
        Task Register();

        /// <summary>
        /// initialize the class
        /// </summary>
        /// <returns></returns>
        Task<bool> Initialize();
    }
}