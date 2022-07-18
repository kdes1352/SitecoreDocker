using System;
using System.Runtime.Serialization;
using Sitecore;
using Sitecore.Data.Validators;
using Sitecore.Text;

namespace AllinaHealth.Framework.Validation
{
    [Serializable]
    public class NumberOfSelectedItemsValidator : StandardValidator
    {
        public NumberOfSelectedItemsValidator()
        {
        }

        public NumberOfSelectedItemsValidator(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Name => "Number of Selected Items";

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return GetFailedResult(ValidatorResult.CriticalError);
        }

        protected override ValidatorResult Evaluate()
        {
            var minNumberOfItems = MainUtil.GetInt(Parameters["Min"], 0);
            var maxNumberOfItems = MainUtil.GetInt(Parameters["Max"], 3);

            var fieldToValidate = GetField();

            if (fieldToValidate == null)
            {
                return GetMaxValidatorResult();
            }

            var list = new ListString(fieldToValidate.Value);

            if (list.Count >= minNumberOfItems && list.Count <= maxNumberOfItems)
            {
                return ValidatorResult.Valid;
            }

            return GetMaxValidatorResult();
        }
    }
}