// Copyright 2016-2020 ?????????????. All Rights Reserved.
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GameFramework.Common.Utilities
{
	public static class IconMaker
	{
		public const int MaxIconWidth = 256;
		public const int MaxIconHeight = 256;

		private const ushort HeaderReserved = 0;
		private const ushort HeaderIconType = 1;
		private const byte HeaderLength = 6;
		private const byte EntryReserved = 0;
		private const byte EntryLength = 16;
		private const byte PngColorsInPalette = 0;
		private const ushort PngColorPlanes = 1;

		public static void Make(string ImagePath, string IconPath)
		{
			Make(Image.FromFile(ImagePath), IconPath);
		}

		public static void Make(Image Image, string IconPath)
		{
			Make((Bitmap)Image, IconPath);
		}

		public static void Make(Bitmap Bitmap, string IconPath)
		{
			Make(new Bitmap[] { Bitmap }, IconPath);
		}

		public static void Make(IEnumerable<Bitmap> Bitmaps, string IconPath)
		{
			Bitmap[] orderedImages = Bitmaps.OrderBy(i => i.Width)
										   .ThenBy(i => i.Height)
										   .ToArray();

			using (BinaryWriter stream = new BinaryWriter(File.OpenWrite(IconPath)))
			{
				// write the header
				stream.Write(HeaderReserved);
				stream.Write(HeaderIconType);
				stream.Write((ushort)orderedImages.Length);

				// save the image buffers and offsets
				Dictionary<uint, byte[]> buffers = new Dictionary<uint, byte[]>();

				// tracks the length of the buffers as the iterations occur
				// and adds that to the offset of the entries
				uint lengthSum = 0;
				uint baseOffset = (uint)(HeaderLength + EntryLength * orderedImages.Length);

				for (int i = 0; i < orderedImages.Length; i++)
				{
					Bitmap image = orderedImages[i];

					// creates a byte array from an image
					byte[] buffer = CreateImageBuffer(image);

					// calculates what the offset of this image will be
					// in the stream
					uint offset = (baseOffset + lengthSum);

					// writes the image entry
					stream.Write(GetIconWidth(image));
					stream.Write(GetIconHeight(image));
					stream.Write(PngColorsInPalette);
					stream.Write(EntryReserved);
					stream.Write(PngColorPlanes);
					stream.Write((ushort)Image.GetPixelFormatSize(image.PixelFormat));
					stream.Write((uint)buffer.Length);
					stream.Write(offset);

					lengthSum += (uint)buffer.Length;

					// adds the buffer to be written at the offset
					buffers.Add(offset, buffer);
				}

				// writes the buffers for each image
				foreach (var kvp in buffers)
				{
					// seeks to the specified offset required for the image buffer
					stream.BaseStream.Seek(kvp.Key, SeekOrigin.Begin);

					// writes the buffer
					stream.Write(kvp.Value);
				}
			}
		}

		private static byte GetIconHeight(Bitmap Bitmap)
		{
			if (Bitmap.Height == MaxIconHeight)
				return 0;

			return (byte)Bitmap.Height;
		}

		private static byte GetIconWidth(Bitmap Bitmap)
		{
			if (Bitmap.Width == MaxIconWidth)
				return 0;

			return (byte)Bitmap.Width;
		}

		private static byte[] CreateImageBuffer(Bitmap Bitmap)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				Bitmap.Save(stream, Bitmap.RawFormat);

				return stream.ToArray();
			}
		}
	}
}
