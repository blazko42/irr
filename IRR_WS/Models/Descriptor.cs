using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;

namespace IRR_WS.Models
{
	[Serializable]
	public class Descriptor
	{
		public KeyPoint[] Keypoints { get; set; }
		public byte[] DescriptorData { get; set; }
		public int DescriptorRows { get; set; }
		public int DescriptorCols { get; set; }

		public Descriptor() { }
		public Descriptor(KeyPoint[] keyPoints, byte[] descriptorData, int descriptorRows, int descriptorCols)
		{
			Keypoints = keyPoints;
			DescriptorData = descriptorData;
			DescriptorRows = descriptorRows;
			DescriptorCols = descriptorCols;
		}

		public Descriptor ComputeSIFTKeypointsAndDescriptors(Mat image)
		{
			Mat grayscaleImage = new Mat();

			Cv2.CvtColor(image, grayscaleImage, ColorConversionCodes.BGR2GRAY);

			SIFT sift = SIFT.Create();

			KeyPoint[] keypoints;
			MatOfFloat descriptors = new MatOfFloat();
			sift.DetectAndCompute(grayscaleImage, null, out keypoints, descriptors);

			int byteCount = IntPtr.Size == 8 ? (int)(descriptors.DataEnd.ToInt64() - descriptors.DataStart.ToInt64()) : descriptors.DataEnd.ToInt32() - descriptors.DataStart.ToInt32();

			byte[] descriptorBuffer = new byte[byteCount];

			Marshal.Copy(descriptors.DataStart, descriptorBuffer, 0, byteCount);


			return new Descriptor(keypoints, descriptorBuffer, descriptors.Rows, descriptors.Cols);
		}

		public Descriptor ComputeSURFKeypointsAndDescriptors(Mat image)
		{
			Mat grayscaleImage = new Mat();

			Cv2.CvtColor(image, grayscaleImage, ColorConversionCodes.BGR2GRAY);

			SURF surf = SURF.Create(500, 4, 2, true);

			KeyPoint[] keypoints;
			MatOfFloat descriptors = new MatOfFloat();

			surf.DetectAndCompute(grayscaleImage, null, out keypoints, descriptors);

			int byteCount = IntPtr.Size == 8 ? (int)(descriptors.DataEnd.ToInt64() - descriptors.DataStart.ToInt64()) : descriptors.DataEnd.ToInt32() - descriptors.DataStart.ToInt32();

			byte[] descriptorBuffer = new byte[byteCount];

			Marshal.Copy(descriptors.DataStart, descriptorBuffer, 0, byteCount);


			return new Descriptor(keypoints, descriptorBuffer, descriptors.Rows, descriptors.Cols);
		}
	}
}