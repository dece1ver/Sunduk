using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.DynamicTemplatesData
{
    public class TemplateDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<VariableDefinition> Variables { get; set; } = [];
        public string Result { get; set; } = string.Empty;
    }
}
