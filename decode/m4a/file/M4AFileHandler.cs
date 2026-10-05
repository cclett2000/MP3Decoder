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
        static readonly HashSet<string> ATOMS_WITH_CHILDREN = new HashSet<string> { 
            "moov" 
        };

        private byte[] data = [];
        private int bytePosition = 0;
        private string m4aFilePath = m4aFilePath;

        AtomContainer atomContainer;

        public void LoadInMemory()
        {
            data = File.ReadAllBytes(m4aFilePath);

            ValidateFileType();
            ParseAtoms();

            data = [];
        }

        private void ParseAtoms(int atomContainerEnd = -1, string parentAtom = "")
        {
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

                if (ATOMS_WITH_CHILDREN.Contains(atomType) || parentAtom != "")
                {
                    // (first iteration) Initialize atom and recursion
                    if (!atomContainer.doesExist(atomType) && parentAtom == "")
                    {
                        atomContainer.addAtom(atomType, 
                            new Atom {
                                Header = atomType,
                                Size = atomSize,
                                IsContainer = true
                            }
                        );

                        bytePosition += ATOM_SIZE_SEGMENT_LENGTH + ATOM_HEAD_SEGMENT_LENGTH;
                        ParseAtoms(atomEnd, atomType);

                        atomContainerEnd = -1;
                        parentAtom = "";
                    }

                    // (subseqeunt iteration) add children to existing atom
                    else
                    {
                        Atom atom = atomContainer.getAtomByName(parentAtom);
                        atom.Children.addAtom(
                            atomType,
                            new Atom {
                                Header = atomType,
                                Size = atomSize,
                                Payload = data[atomStart..atomEnd]
                            }
                        );

                        bytePosition += atomSize + ATOM_HEAD_SEGMENT_LENGTH;
                    }

                    // if exist, 
                }
                else
                {
                    atomContainer.addAtom(
                        atomType,
                        new Atom
                        {
                            Header = atomType,
                            Payload = data[atomStart..atomEnd],
                            Size = atomSize
                        }
                    );

                    // update global position
                    bytePosition = atomEnd;
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
