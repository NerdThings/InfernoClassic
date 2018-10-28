using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Inferno.Audio.Formats
{
    public class Wave
    {
        public const string Signature = "RIFF";
        public const string Format = "WAVE";
        public const string FormatSignature = "fmt ";
        public const string DataSignature = "data";

        public int RiffChunkSize;
        public int FormatChunkSize;
        public short AudioFormat;
        public short NumChannels;
        public int SampleRate;
        public int ByteRate;
        public short BlockAlign;
        public short BitsPerSample;
        public int DataChunkSize;
        public byte[] Data;

        public Sound ToSound()
        {
            return new Sound(Data, SampleRate, NumChannels, BitsPerSample);
        }

        public static Wave FromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var wave = new Wave();

            using (var reader = new BinaryReader(stream))
            {
                // RIFF header
                var signature = new string(reader.ReadChars(4));
                if (signature != Signature)
                    throw new NotSupportedException("Specified stream is not a wave file.");

                wave.RiffChunkSize = reader.ReadInt32();

                var format = new string(reader.ReadChars(4));
                if (format != Format)
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                var formatSignature = new string(reader.ReadChars(4));
                if (formatSignature != FormatSignature)
                    throw new NotSupportedException("Specified wave file is not supported.");

                wave.FormatChunkSize = reader.ReadInt32();
                wave.AudioFormat = reader.ReadInt16();
                wave.NumChannels = reader.ReadInt16();
                wave.SampleRate = reader.ReadInt32();
                wave.ByteRate = reader.ReadInt32();
                wave.BlockAlign = reader.ReadInt16();
                wave.BitsPerSample = reader.ReadInt16();

                var dataSignature = new string(reader.ReadChars(4));
                if (dataSignature != DataSignature)
                    throw new NotSupportedException("Specified wave file is not supported.");

                wave.DataChunkSize = reader.ReadInt32();

                wave.Data = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            return wave;
        }
    }
}
