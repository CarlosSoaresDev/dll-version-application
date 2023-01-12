using dotnet.dll.version.messages.core;

namespace dotnet.dll.version.messages.v1._0.hello
{
    public class ABC0001 : MessageStructure
    {
        public override string GetMessageValue()
        {
            return $"Message {nameof(ABC0001)}, Version 1.0";
        }
    }
}