using CQRS.ES;
using SimpleCQRS;

namespace SyncApi
{
    public static class ServiceLocator
    {
        public static EventBus Bus { get; set; }
       
    }
}