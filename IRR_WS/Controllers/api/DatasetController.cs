using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IRR_WS.Controllers.api
{
	public class DatasetController : ApiController
	{
		[HttpGet]
		public async Task<bool> ComputeActiveFeatureExtractor()
		{
			return await Models.IndexImage.ComputeDataset(Models.FeatureExtractor.GetActiveFeatureExtractor().Code);
		}

		[HttpGet]
		public async Task<bool> ClearDatasetForActiveFeatureExtractor()
		{
			return await Models.IndexImage.ClearDatasetForActiveFeatureExtractor();
		}

		[HttpPost]
		public async Task<bool> FeatureMatch()
		{
			// Check if the request contains multipart/form-data.  
			if (!Request.Content.IsMimeMultipartContent())
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}

			var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

			Models.Landmark landmark = new Models.Landmark();

			IList<HttpContent> files = provider.Files;

			HttpContent file = files[0];
			string landmarkId = System.Web.HttpContext.Current.Request.Params["landmarkId"];

			byte[] fileContent = await file.ReadAsByteArrayAsync();
			var fileName = file.Headers.ContentDisposition.FileName.Trim('\"');

			landmark.LandmarkUploadedImageName = fileName;
			landmark.LandmarkUploadedIamgeContent = fileContent;

			try
			{
				Models.Landmark.FeatureMatch(landmark.LandmarkUploadedIamgeContent, landmarkId, Models.FeatureExtractor.GetActiveFeatureExtractor().Code);
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

	}
}
