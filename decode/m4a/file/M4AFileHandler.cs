using MP3Decoder.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MP3Decoder.decode.m4a.file
{
    internal class M4AFileHandler(string m4aFilePath)
    {
        static readonly int ATOM_SIZE_SEGMENT_LENGTH = 4;
        static readonly int ATOM_HEAD_SEGMENT_LENGTH = 4;

        private byte[] data = [];
        private int bytePosition = 0;
        private string m4aFilePath = m4aFilePath;

        AtomContainer atomContainer;

        public void LoadInMemory()
        {
            data = File.ReadAllBytes(m4aFilePath);

            ValidateFileType();
            ByteToAtomConverter(new Atom());

            data = [];
        }

        private void ByteToAtomConverter(Atom atom, int atomContainerEnd = -1, bool processForChild = false) {
            int containerEnd = atomContainerEnd == -1 ? data.Length : atomContainerEnd;
      
            while (bytePosition < containerEnd)
            {
                (int atomStart, int atomEnd) = CalculateAtomStartEndPosition();
                int atomSize = atomEnd - atomStart;

                if (atomSize == 0)
                {
                    continue;
                }

                int atomTypeSize = atomStart + ATOM_HEAD_SEGMENT_LENGTH;
                string atomType = ByteHelper.getStringValueFromByteArray(
                    data[atomStart..atomTypeSize]
                );

                if (Atom.isAtomIgnored(atomType))
                {
                    bytePosition += ATOM_SIZE_SEGMENT_LENGTH + atomSize;
                    continue;
                }

                // initialize parent atom as container, begin recursive call
                if (Atom.isContainerAtom(atomType))
                {
                    bytePosition += ATOM_SIZE_SEGMENT_LENGTH + ATOM_HEAD_SEGMENT_LENGTH;

                    if (processForChild)
                    {
                        Atom childAtom = new Atom { 
                            Header = atomType,
                            IsContainer = true,
                            Size = atomSize
                        };

                        ByteToAtomConverter(childAtom, atomEnd, true);
                        atom.Container.addAtom(atomType, childAtom);
                        atomContainerEnd = -1;
                    }
                    else
                    {
                        atom.Header = atomType;
                        atom.IsContainer = true;
                        atom.Size = atomSize;

                        ByteToAtomConverter(atom, atomEnd, true);
                        this.atomContainer.addAtom(atomType, atom);
                        atomContainerEnd = -1;
                    }
                } 

                // We're within a recursive call, all atoms should be stored in parent
                else if (processForChild)
                {
                    atom.Container.addAtom(
                        atomType,
                        new Atom
                        {
                            Header = atomType,
                            Size = atomSize,
                            Payload = data[atomStart..atomEnd]
                        }
                    );

                    bytePosition += ATOM_SIZE_SEGMENT_LENGTH + atomSize;
                }

                // Top-most level atom, no recursion -- watch this bug out lmao
                else
                {
                    atom.Header = atomType;
                    atom.IsContainer = true;
                    atom.Size = atomSize;
                    atom.Payload = data[atomStart..atomEnd];

                    this.atomContainer.addAtom(atomType, atom);

                    // ensure lingering data isn't present
                    atom = new Atom();

                    bytePosition += ATOM_SIZE_SEGMENT_LENGTH + atomSize;
                }
            }
        }

        /// <summary>
        /// Calculates the size of the next block in the M4A byte array.
        /// </summary>
        /// 
        /// <returns>
        /// The start and end position of the current block
        /// (The starting index DOES NOT include the 4 byte size segment)
        /// </returns>
        private (int, int) CalculateAtomStartEndPosition()
        {
            int atomStart = bytePosition + ATOM_SIZE_SEGMENT_LENGTH;
            int atomEnd = ByteHelper.calculateIntegerFromByteArray(
                data[bytePosition..atomStart],
                true
            );

            return (atomStart, (bytePosition + atomEnd));
        }

        private void ValidateFileType()
        {
            string atomType = ByteHelper.getStringValueFromByteArray(
                data[ATOM_SIZE_SEGMENT_LENGTH..(ATOM_SIZE_SEGMENT_LENGTH + (ATOM_HEAD_SEGMENT_LENGTH * 2))]
            );

            if (atomType != "ftypM4A ")
            {
                // do something else
                Environment.Exit(500);
            }
        }

        public void setAtomContainer(AtomContainer atomContainer)
        {
            this.atomContainer = atomContainer;
        }
    }
}
