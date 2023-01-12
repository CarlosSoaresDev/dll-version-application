using dotnet.dll.version.messages.core;

namespace dotnet.dll.version.messages.v1._1.hello
{
    public class BCA0001 : MessageStructure
    {
        public override string GetMessageValue()
        {
            return $"Message {nameof(BCA0001)}, Version 1.1";
        }
    }
}