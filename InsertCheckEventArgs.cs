﻿using System;

namespace Zeroit.Framework.CodeBox
{
    /// <summary>
    /// Provides data for the <see cref="Scintilla.InsertCheck" /> event.
    /// </summary>
    public class InsertCheckEventArgs : EventArgs
    {
        private readonly ZeroitCodeExplorer Scintilla;
        private readonly int bytePosition;
        private readonly int byteLength;
        private readonly IntPtr textPtr;

        internal int? CachedPosition { get; set; }
        internal string CachedText { get; set; }

        /// <summary>
        /// Gets the zero-based document position where text will be inserted.
        /// </summary>
        /// <returns>The zero-based character position within the document where text will be inserted.</returns>
        public int Position
        {
            get
            {
                if (CachedPosition == null)
                    CachedPosition = Scintilla.Lines.ByteToCharPosition(bytePosition);

                return (int)CachedPosition;
            }
        }

        /// <summary>
        /// Gets or sets the text being inserted.
        /// </summary>
        /// <returns>The text being inserted into the document.</returns>
        public unsafe string Text
        {
            get
            {
                if (CachedText == null)
                    CachedText = Helpers.GetString(textPtr, byteLength, Scintilla.Encoding);

                return CachedText;
            }
            set
            {
                CachedText = value ?? string.Empty;

                var bytes = Helpers.GetBytes(CachedText, Scintilla.Encoding, zeroTerminated: false);
                fixed (byte* bp = bytes)
                    Scintilla.DirectMessage(NativeMethods.SCI_CHANGEINSERTION, new IntPtr(bytes.Length), new IntPtr(bp));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCheckEventArgs" /> class.
        /// </summary>
        /// <param name="Scintilla">The <see cref="Scintilla" /> control that generated this event.</param>
        /// <param name="bytePosition">The zero-based byte position within the document where text is being inserted.</param>
        /// <param name="byteLength">The length in bytes of the inserted text.</param>
        /// <param name="text">A pointer to the text being inserted.</param>
        public InsertCheckEventArgs(ZeroitCodeExplorer Scintilla, int bytePosition, int byteLength, IntPtr text)
        {
            this.Scintilla = Scintilla;
            this.bytePosition = bytePosition;
            this.byteLength = byteLength;
            this.textPtr = text;
        }
    }
}