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
            // load file into mem using stream
            data = File.ReadAllBytes(m4aFilePath);

            ValidateFileType();
            ParseAtoms();

            // GC, do your thing
            data = [];
        }

        private void ParseAtoms()
        {
            bool isContainer = false;
            int containerEnd = -1;

            Atom containerAtom = new Atom
            {
                IsContainer = true
            };

            while (bytePosition < data.Length)
            {
                (int atomStart, int atomEnd) = CalculateAtomSizeUsingGlobalIndex();

                int headEnd = atomStart + ATOM_HEAD_SEGMENT_LENGTH;
                string atomHead = ByteHelper.getStringValueFromByteArray(
                    data[atomStart..headEnd]
                );

                // TODO: Fix how child atoms are parse, possible decouple from global index
                if (ATOMS_WITH_CHILDREN.Contains(atomHead))
                {
                    isContainer = true;
                    containerEnd = atomEnd;
                    containerAtom.Header = atomHead;
                } 
                else if (isContainer && bytePosition <= containerEnd) 
                {
                    containerAtom.Children.addBlock(
                        atomHead,
                        new Atom
                        {
                            Header = atomHead,
                            Payload = data[atomStart..atomEnd],
                            Size = headEnd - atomStart
                        }
                    );
                }
                else
                {
                    isContainer = false;
                    containerEnd = -1;
                    containerAtom = new Atom
                    {
                        IsContainer = true
                    };

                    atomContainer.addBlock(
                        atomHead,
                        new Atom
                        {
                            Header = atomHead,
                            Payload = data[atomStart..atomEnd],
                            Size = headEnd - atomStart
                        }
                    );
                }
            }
        }

        private void ParseChildAtoms(string atomHead, byte[] payload)
        {
            Atom atom = new Atom
            {
                IsContainer = true,
                Header = atomHead
            };


        }

        /// <summary>
        /// Calculates the size of the next block in the M4A byte array.
        /// </summary>
        /// 
        /// <returns>
        /// The start and end position of the current block
        /// (The starting index DOES NOT include the 4 byte size segment)
        /// </returns>
        private (int, int) CalculateAtomSizeUsingGlobalIndex()
        {
            int atomStart = bytePosition + ATOM_SIZE_SEGMENT_LENGTH;
            int atomEnd = ByteHelper.calculateIntegerFromByteArray(
                data[bytePosition..atomStart],
                true
            );


            bytePosition += atomEnd;

            return (atomStart, bytePosition);
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
