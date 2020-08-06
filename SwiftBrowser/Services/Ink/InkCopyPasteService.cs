using System.Linq;
using Windows.Foundation;

namespace flowpad.Services.Ink
{
    public class InkCopyPasteService
    {
        private const int PasteDistance = 20;
        private readonly InkStrokesService _strokesService;
        private Point pastePosition;

        public InkCopyPasteService(InkStrokesService strokesService)
        {
            _strokesService = strokesService;
        }

        public bool CanCopy => _strokesService.GetSelectedStrokes().Any();

        public bool CanCut => _strokesService.GetSelectedStrokes().Any();

        public bool CanPaste => _strokesService.CanPaste;

        public Point Copy()
        {
            if (!CanCopy) return default;

            var rect = _strokesService.CopySelectedStrokes();

            pastePosition = new Point(rect.X, rect.Y);
            return pastePosition;
        }

        public Point Cut()
        {
            if (!CanCut) return default;

            var rect = _strokesService.CutSelectedStrokes();

            pastePosition = new Point(rect.X, rect.Y);
            return pastePosition;
        }

        public Rect Paste()
        {
            pastePosition.X += PasteDistance;
            pastePosition.Y += PasteDistance;

            return Paste(pastePosition);
        }

        public Rect Paste(Point position)
        {
            return _strokesService.PasteSelectedStrokes(position);
        }
    }
}
