using Microsoft.EntityFrameworkCore;

namespace DataEF
{
    /// <summary>
    /// This class is for testing only.
    /// Mocking Context requires a parameterless constructor.
    /// If added into Context directly it will be removed every time Context is scaffolded.
    /// </summary>
    public class ContextParameterlessConstructor : Context
    {
        public ContextParameterlessConstructor() : base(new DbContextOptions<Context>())
        {
        }
    }
}
