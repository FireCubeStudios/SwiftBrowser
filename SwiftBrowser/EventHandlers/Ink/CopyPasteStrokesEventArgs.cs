﻿using System.Collections.Generic;
using Windows.UI.Input.Inking;

namespace flowpad.EventHandlers.Ink
{
    public class CopyPasteStrokesEventArgs
    {
        public CopyPasteStrokesEventArgs(IEnumerable<InkStroke> strokes)
        {
            Strokes = strokes;
        }

        public IEnumerable<InkStroke> Strokes { get; set; }
    }
}
