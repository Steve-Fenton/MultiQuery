namespace mq
{
    internal class Input
    {
        public string Query { get; set; }

        public IList<string> Fields { get; set; }

        public IList<string> ConnectionStrings { get; set; }
    }
}
