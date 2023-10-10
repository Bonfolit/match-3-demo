namespace BonLib.Events
{

    public interface IEvent
    {
        bool IsConsumed { get; set; }
    }

}