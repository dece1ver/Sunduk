using Sunduk.PWA.Infrastructure.Extensions;

namespace Sunduk.PWA.Infrastructure.DynamicTemplatesData
{
    public class VariableDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Expression { get; set; } = string.Empty;

        public bool HasError => !string.IsNullOrEmpty(Error);

        public string Error
        {
            get
            {
                if (Name.StartsWith(c => char.IsDigit(c))) return "Начинать с цифры нельзя";
                return string.Empty;
            }
        }
    }
}
