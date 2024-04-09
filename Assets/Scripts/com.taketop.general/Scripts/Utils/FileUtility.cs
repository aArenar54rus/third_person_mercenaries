﻿namespace Module.General.Utils
{
	public static class FileUtility
	{
		/// <summary>
		/// Returns the human-readable file size for an arbitrary, 64-bit file size
		/// The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
		/// </summary>
		/// <param name="i"></param>
		/// <remarks>http://stackoverflow.com/a/281684/147003</remarks>
		/// <returns></returns>
		public static string GetBytesReadable(long i)
		{
			string sign = i < 0 ? "-" : "";
			double readable = i < 0 ? -i : i;
			string suffix;
			if (i >= 0x1000000000000000) // Exabyte
			{
				suffix = "EB";
				readable = i >> 50;
			}
			else if (i >= 0x4000000000000) // Petabyte
			{
				suffix = "PB";
				readable = i >> 40;
			}
			else if (i >= 0x10000000000) // Terabyte
			{
				suffix = "TB";
				readable = i >> 30;
			}
			else if (i >= 0x40000000) // Gigabyte
			{
				suffix = "GB";
				readable = i >> 20;
			}
			else if (i >= 0x100000) // Megabyte
			{
				suffix = "MB";
				readable = i >> 10;
			}
			else if (i >= 0x400) // Kilobyte
			{
				suffix = "KB";
				readable = i;
			}
			else
			{
				return i.ToString(sign + "0 B"); // Byte
			}
			readable /= 1024;

			return sign + readable.ToString("0.### ") + suffix;
		}
	}
}
