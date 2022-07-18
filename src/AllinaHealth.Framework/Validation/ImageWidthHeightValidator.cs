using System;
using System.Globalization;
using System.Runtime.Serialization;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Data.Validators;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;

namespace AllinaHealth.Framework.Validation
{
    [Serializable]
    public class ImageWidthHeightValidator : StandardValidator
    {
        public ImageWidthHeightValidator()
        {
        }

        public ImageWidthHeightValidator(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Name => "Image Width/Height";

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return GetFailedResult(ValidatorResult.CriticalError);
        }

        protected override ValidatorResult Evaluate()
        {
            var minWidth = MainUtil.GetInt(Parameters["MinWidth"], 0);
            var minHeight = MainUtil.GetInt(Parameters["MinHeight"], 0);
            var maxWidth = MainUtil.GetInt(Parameters["MaxWidth"], int.MaxValue);
            var maxHeight = MainUtil.GetInt(Parameters["MaxHeight"], int.MaxValue);
            var aspectRatio = MainUtil.GetFloat(Parameters["AspectRatio"], 0.00f);

            if (ItemUri == null)
            {
                return ValidatorResult.Valid;
            }

            var mediaItem = MediaItem;

            if (mediaItem == null)
                return ValidatorResult.Valid;

            var width = MainUtil.GetInt(mediaItem.InnerItem["Width"], 0);
            var height = MainUtil.GetInt(mediaItem.InnerItem["Height"], 0);

            if (minWidth == maxWidth && minHeight == maxHeight && (width != minWidth || height != minHeight))
                return GetResult("The image referenced in the Image field \"{0}\" does not match the size requirements. Image needs to be exactly {1}x{2} but is {3}x{4}",
                    GetField().DisplayName, minWidth.ToString(), minHeight.ToString(), width.ToString(),
                    height.ToString());

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (aspectRatio != 0.00f && height != 0 && Math.Round((double)width / height, 2) != Math.Round(aspectRatio, 2))
                return GetResult("The image referenced in the Image field \"{0}\" has an invalid aspect ratio. The aspect ratio needs to be {1} but is {2}.",
                    GetField().DisplayName, Math.Round(aspectRatio, 2).ToString(CultureInfo.InvariantCulture),
                    (Math.Round((double)width / height, 2).ToString(CultureInfo.InvariantCulture)));

            if (minWidth == maxWidth && width != minWidth)
                return GetResult("The image referenced in the Image field \"{0}\" does not match the size requirements. Image needs to be exactly {1} wide but is {2}",
                    GetField().DisplayName, minWidth.ToString(), width.ToString());

            if (minHeight == maxHeight && height != minHeight)
                return GetResult("The image referenced in the Image field \"{0}\" does not match the size requirements. Image needs to be exactly {1} high but is {2}",
                    GetField().DisplayName, minHeight.ToString(), height.ToString());

            if (width < minWidth)
                return GetResult("The image referenced in the Image field \"{0}\" is too small. The width needs to be at least {1} pixels but is {2}.",
                    GetField().DisplayName, minWidth.ToString(), width.ToString());

            if (height < minHeight)
                return GetResult("The image referenced in the Image field \"{0}\" is too small. The height needs to be at least {1} pixels but is {2}.",
                    GetField().DisplayName, minHeight.ToString(), height.ToString());

            if (width > maxWidth)
                return GetResult("The image referenced in the Image field \"{0}\" is too big. The width needs to at most {1} pixels but is {2}.",
                    GetField().DisplayName, maxWidth.ToString(), width.ToString());

            if (height > maxHeight)
                return GetResult("The image referenced in the Image field \"{0}\" is too big. The height needs to be at most {1} pixels but is {2}.",
                    GetField().DisplayName, maxHeight.ToString(), height.ToString());
            return ValidatorResult.Valid;
        }

        private MediaItem MediaItem
        {
            get
            {
                var itemUri = ItemUri;

                var field = GetField();
                if (field == null)
                    return null;

                if (string.IsNullOrEmpty(field.Value))
                    return null;

                if (field.Value == "<image />")
                    return null;

                var attribute = new XmlValue(field.Value, "image").GetAttribute("mediaid");
                if (string.IsNullOrEmpty(attribute))
                    return null;

                var database = Sitecore.Configuration.Factory.GetDatabase(itemUri.DatabaseName);
                Assert.IsNotNull(database, itemUri.DatabaseName);
                return database.GetItem(attribute);
            }
        }

        private ValidatorResult GetResult(string text, params string[] arguments)
        {
            Text = GetText(text, arguments);
            return GetFailedResult(ValidatorResult.Warning);
        }
    }
}