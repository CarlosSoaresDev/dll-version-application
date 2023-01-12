using dotnet.dll.version.messages.core;

namespace dotnet.dll.version.messages.v1._1.hello
{
    public class ABC0002 : MessageStructure
    {
        public override string GetMessageValue()
        {
            return $"Message {nameof(ABC0002)}, Version 1.1";
        }
    }
}