using System;

namespace Capture.Interface
{
    [Serializable]
    public class ScreenshotRequest: MarshalByRefObject
    {
        public Guid RequestId { get; set; }

        public ScreenshotRequest(): this(Guid.NewGuid())
        {
        }

        public ScreenshotRequest(Guid requestId)
        {
            RequestId = requestId;
        }
    }
}
