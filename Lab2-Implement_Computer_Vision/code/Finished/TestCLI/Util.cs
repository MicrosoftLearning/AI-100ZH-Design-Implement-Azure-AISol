using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ProcessingLibrary;
using Newtonsoft.Json;
using SkiaSharp;

namespace TestCLI
{
    /// <summary>
    /// ImageMetadata stores the image data from Cognitive Services into CosmosDB.
    /// </summary>
    public class ImageMetadata
    {
        /// <summary>
        /// Build from an image path, storing the full local path, but using the filename as ID.
        /// </summary>
        /// <param name="imageFilePath">Local file path.</param>
        public ImageMetadata(string imageFilePath)
        {
            this.LocalFilePath = imageFilePath;
            this.FileName = Path.GetFileName(imageFilePath);
            this.Id = this.FileName; // TODO: Worry about collisions, but ID can't handle slashes.
        }

        /// <summary>
        /// Public parameterless constructor for serialization-friendliness.
        /// </summary>
        public ImageMetadata()
        {
            
        }

        /// <summary>
        /// Store the ImageInsights into the metadata - pulls out tags and caption, stores number of faces and face details.
        /// </summary>
        /// <param name="insights"></param>
        public void AddInsights(ImageInsights insights)
        {
            this.Caption = insights.Caption;
            this.Tags = insights.Tags;

        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public Uri BlobUri { get; set; }

        public string LocalFilePath { get; set; }

        public string FileName { get; set; }

        public string Caption { get; set; }

        public string[] Tags { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    static class Util
    {
        /// <summary>
        /// Simple resize method for use when we're trying to run the cognitive services against large images. 
        /// We resize downward to avoid too much data over the wire.
        /// </summary>
        /// <param name="imageFile">Image file to resize.</param>
        /// <param name="size">Maximum height/width - will retain aspect ratio.</param>
        /// <returns>Revised width/height and resized image filename.</returns>
        public static Tuple<Tuple<double, double>, string> ResizeIfRequired(string imageFile, int size)
        {
            using (var input = File.OpenRead(imageFile))
            using (var inputStream = new SKManagedStream(input))
            {
                using (var original = SKBitmap.Decode(inputStream))
                {
                    int width, height;
                    if (original.Width > original.Height)
                    {
                        width = size;
                        height = original.Height * size / original.Width;
                    }
                    else
                    {
                        width = original.Width * size / original.Height;
                        height = size;
                    }

                    using (var resized = original.Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3))
                    {
                        // No need to resize
                        if (resized == null)
                            return Tuple.Create((Tuple<double, double>)null, imageFile);

                        using (var image = SKImage.FromBitmap(resized))
                        {
                            var resizedImageFile = Path.GetTempFileName();
                            using (var output = File.OpenWrite(resizedImageFile))
                            {
                                image.Encode(SKEncodedImageFormat.Png, 75)
                                    .SaveTo(output);
                            }
                            return Tuple.Create(Tuple.Create((double)original.Width / width, (double)original.Height / height), resizedImageFile);

                        }
                    }

                }
            }
        }   

    }
}
