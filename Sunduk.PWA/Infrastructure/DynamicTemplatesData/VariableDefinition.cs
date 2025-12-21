using Sunduk.PWA.Infrastructure.Extensions;

namespace Sunduk.PWA.Infrastructure.DynamicTemplatesData
{
    public class VariableDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Expression { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string Error { get; set; } = string.Empty;

        public void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                HasError = false;
                Error = string.Empty;
                return;
            }

            if (Name.StartsWith(c => char.IsDigit(c)))
            {
                HasError = true;
                Error = "Начинать с цифры нельзя";
                return;
            }

            HasError = false;
            Error = string.Empty;
        }

        public void SetCalculationError(string error)
        {
            HasError = true;
            Error = error;
        }

        public void ClearError()
        {
            HasError = false;
            Error = string.Empty;
        }
    }
}