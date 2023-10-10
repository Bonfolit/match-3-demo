namespace BonLib.Events
{

    public interface IEventHandler : IEventHandlerBase
    {
        void OnEventReceived();
    }

    public interface IEventHandler<T> : IEventHandlerBase where T : IEvent
    {
        void OnEventReceived(ref T evt);
    }

}