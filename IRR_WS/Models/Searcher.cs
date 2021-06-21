using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using static IRR_WS.Models.IndexImage;


namespace IRR_WS.Models
{
	public class Searcher
	{
		private List<ImageIndex> indexedDatset;
		private Descriptor descriptor;
		public Searcher(List<ImageIndex> dataset)
		{
			this.indexedDatset = dataset;
			descriptor = new Descriptor();
		}

		public List<string> SearchDescriptor(byte[] imageContent, string descriptorName)
		{
			if (descriptorName == "SIFT")
				return SearchSIFT(imageContent);
			else if (descriptorName == "SURF")
				return SearchSURF(imageContent);
			else
				return null;
		}

		private List<string> SearchSIFT(byte[] imageContent)
		{
			Mat image = Cv2.ImDecode(imageContent, ImreadModes.Color);

			Dictionary<string, float> results = new Dictionary<string, float>();

			Descriptor queryFeatures = descriptor.ComputeSIFTKeypointsAndDescriptors(image);

			//BFMatcher bfMatcher = new BFMatcher(NormTypes.L2, false);

			FlannBasedMatcher flannMatcher = new FlannBasedMatcher();

			Mat queryImageDescriptors = new Mat(queryFeatures.DescriptorRows, queryFeatures.DescriptorCols, MatType.CV_32FC1, queryFeatures.DescriptorData);

			foreach (var item in indexedDatset)
			{
				Mat trainImageDescriptors = new Mat(item.Descriptor.DescriptorRows, item.Descriptor.DescriptorCols, MatType.CV_32FC1, item.Descriptor.DescriptorData);

				//DMatch[][] bfMatches = bfMatcher.KnnMatch(queryImageDescriptors, trainImageDescriptors, 2);
				DMatch[][] flannMatches = flannMatcher.KnnMatch(queryImageDescriptors, trainImageDescriptors, 2);

				List<DMatch> goodMatches = new List<DMatch>();

				for (int i = 0; i < flannMatches.Length; i++)
				{
					if (flannMatches[i][0].Distance < 0.80 * flannMatches[i][1].Distance)
					{
						goodMatches.Add(flannMatches[i][0]);
					}
				}
				results.Add(item.ImageId, goodMatches.Average(x => (x.Distance)));

			}

			return results.OrderBy(x => (x.Value)).Select(x => (x.Key)).Take(5).ToList();

		}

		private List<string> SearchSURF(byte[] imageContent)
		{
			Mat image = Cv2.ImDecode(imageContent, ImreadModes.Color);

			Dictionary<string, float> results = new Dictionary<string, float>();

			Descriptor queryFeatures = descriptor.ComputeSURFKeypointsAndDescriptors(image);

			//BFMatcher bfMatcher = new BFMatcher(NormTypes.L2, false);

			FlannBasedMatcher flannMatcher = new FlannBasedMatcher();

			Mat queryImageDescriptors = new Mat(queryFeatures.DescriptorRows, queryFeatures.DescriptorCols, MatType.CV_32FC1, queryFeatures.DescriptorData);

			foreach (var item in indexedDatset)
			{
				Mat trainImageDescriptors = new Mat(item.Descriptor.DescriptorRows, item.Descriptor.DescriptorCols, MatType.CV_32FC1, item.Descriptor.DescriptorData);
				//DMatch[][] bfMatches = bfMatcher.KnnMatch(queryImageDescriptors, trainImageDescriptors, 2);
				DMatch[][] flannMatches = flannMatcher.KnnMatch(queryImageDescriptors, trainImageDescriptors, 2);

				List<DMatch> goodMatches = new List<DMatch>();

				for (int i = 0; i < flannMatches.Length; i++)
				{
					if (flannMatches[i][0].Distance < 0.80 * flannMatches[i][1].Distance)
					{
						goodMatches.Add(flannMatches[i][0]);
					}
				}
				results.Add(item.ImageId, goodMatches.Average(x => (x.Distance)));

			}

			return results.OrderBy(x => (x.Value)).Select(x => (x.Key)).Take(5).ToList();

		}

		public void DrawMatchesBetweenImages(byte[] imageContent, string landmarkId, string descriptorName)
		{
			Mat featureMatchedImage = new Mat();
			Descriptor queryFeatures = new Descriptor();
			List<DMatch> goodMatches = new List<DMatch>();

			Mat queryImage = Cv2.ImDecode(imageContent, ImreadModes.Color);

			if (descriptorName == "SIFT")
				queryFeatures = descriptor.ComputeSIFTKeypointsAndDescriptors(queryImage);
			else if (descriptorName == "SURF")
				queryFeatures = descriptor.ComputeSURFKeypointsAndDescriptors(queryImage);

			FlannBasedMatcher flannMatcher = new FlannBasedMatcher();

			Mat queryImageDescriptors = new Mat(queryFeatures.DescriptorRows, queryFeatures.DescriptorCols, MatType.CV_32FC1, queryFeatures.DescriptorData);

			ImageIndex trainImageIndex = indexedDatset.First(x => x.ImageId == landmarkId);
			Mat trainImage = Cv2.ImRead(trainImageIndex.ImagePath, ImreadModes.Color);

			Mat trainImageDescriptors = new Mat(trainImageIndex.Descriptor.DescriptorRows, trainImageIndex.Descriptor.DescriptorCols, MatType.CV_32FC1, trainImageIndex.Descriptor.DescriptorData);

			DMatch[][] flannMatches = flannMatcher.KnnMatch(queryImageDescriptors, trainImageDescriptors, 2);

			for (int i = 0; i < flannMatches.Length; i++)
			{
				if (flannMatches[i][0].Distance < 0.80 * flannMatches[i][1].Distance)
				{
					goodMatches.Add(flannMatches[i][0]);
				}
			}

			Cv2.DrawMatches(queryImage, queryFeatures.Keypoints, trainImage, trainImageIndex.Descriptor.Keypoints, goodMatches, featureMatchedImage, new Scalar(0, 255, 0), null);

			using (new Window(descriptorName + " matching", WindowMode.Normal, featureMatchedImage))
			{
				Cv2.WaitKey();
			}

		}

	}
}