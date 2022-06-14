namespace Email
{
    internal class MailListItems
    {
        public string Date { get; internal set; }
        public string From { get; internal set; }
        public object Subj { get; internal set; }
        public bool HasAttachments { get; internal set; }
    }
}