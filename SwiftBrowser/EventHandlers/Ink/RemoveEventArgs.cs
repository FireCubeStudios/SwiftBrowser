using Windows.UI.Input.Inking;

namespace flowpad.EventHandlers.Ink
{
    public class RemoveEventArgs
    {
        public RemoveEventArgs(InkStroke removedStroke)
        {
            RemovedStroke = removedStroke;
        }

        public InkStroke RemovedStroke { get; set; }
    }
}
