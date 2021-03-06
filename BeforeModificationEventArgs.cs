﻿// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeBox
// Author           : ZEROIT
// Created          : 03-19-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-05-2019
// ***********************************************************************
// <copyright file="BeforeModificationEventArgs.cs" company="Zeroit Dev">
//    This program is for creating a Code Editor control.
//    Copyright ©  2017  Zeroit Dev Technologies
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
//    You can contact me at zeroitdevnet@gmail.com or zeroitdev@outlook.com
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace Zeroit.Framework.CodeBox
{
    /// <summary>
    /// Provides data for the <see cref="Scintilla.BeforeInsert" /> and <see cref="Scintilla.BeforeDelete" /> events.
    /// </summary>
    public class BeforeModificationEventArgs : EventArgs
    {
        private readonly ZeroitCodeExplorer Scintilla;
        private readonly int bytePosition;
        private readonly int byteLength;
        private readonly IntPtr textPtr;

        internal int? CachedPosition { get; set; }
        internal string CachedText { get; set; }

        /// <summary>
        /// Gets the zero-based document position where the modification will occur.
        /// </summary>
        /// <returns>The zero-based character position within the document where text will be inserted or deleted.</returns>
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
        /// Gets the source of the modification.
        /// </summary>
        /// <returns>One of the <see cref="ModificationSource" /> enum values.</returns>
        public ModificationSource Source { get; private set; }

        /// <summary>
        /// Gets the text being inserted or deleted.
        /// </summary>
        /// <returns>
        /// The text about to be inserted or deleted, or null when the the source of the modification is an undo/redo operation.
        /// </returns>
        /// <remarks>
        /// This property will return null when <see cref="Source" /> is <see cref="ModificationSource.Undo" /> or <see cref="ModificationSource.Redo" />.
        /// </remarks>
        public unsafe virtual string Text
        {
            get
            {
                if (Source != ModificationSource.User)
                    return null;

                if (CachedText == null)
                {
                    // For some reason the Scintilla overlords don't provide text in
                    // SC_MOD_BEFOREDELETE... but we can get it from the document.
                    if (textPtr == IntPtr.Zero)
                    {
                        var ptr = Scintilla.DirectMessage(NativeMethods.SCI_GETRANGEPOINTER, new IntPtr(bytePosition), new IntPtr(byteLength));
                        CachedText = new string((sbyte*)ptr, 0, byteLength, Scintilla.Encoding);
                    }
                    else
                    {
                        CachedText = Helpers.GetString(textPtr, byteLength, Scintilla.Encoding);
                    }
                }

                return CachedText;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeModificationEventArgs" /> class.
        /// </summary>
        /// <param name="Scintilla">The <see cref="Scintilla" /> control that generated this event.</param>
        /// <param name="source">The source of the modification.</param>
        /// <param name="bytePosition">The zero-based byte position within the document where text is being modified.</param>
        /// <param name="byteLength">The length in bytes of the text being modified.</param>
        /// <param name="text">A pointer to the text being inserted.</param>
        public BeforeModificationEventArgs(ZeroitCodeExplorer Scintilla, ModificationSource source, int bytePosition, int byteLength, IntPtr text)
        {
            this.Scintilla = Scintilla;
            this.bytePosition = bytePosition;
            this.byteLength = byteLength;
            this.textPtr = text;

            Source = source;
        }
    }
}
