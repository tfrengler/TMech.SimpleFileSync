namespace FileWatcher
{
    public enum CommandDirection
    {
        UP, DOWN
    }

    public abstract class CommandBase
    {
        public CommandDirection Direction { get; set; }
        protected bool Success { get; }

        public CommandBase(CommandDirection direction)
        {
            Direction = direction;
        }
    }
}
