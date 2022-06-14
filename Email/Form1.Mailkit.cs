using MailKit;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MimeKit;

namespace Email
{
    public partial class Form1
    {
        private List<MailListItems> getMails()
        {
            var list = new List<MailListItems>();
            try
            {
                using (var client = new MailKit.Net.Imap.ImapClient())
                {
                    client.Connect("imap.yandex.ru", 993, true);
                    client.Authenticate(Properties.Settings.Default.YandexUser, Properties.Settings.Default.YandexPass);

                    client.Inbox.Open(MailKit.FolderAccess.ReadOnly);

                    var uids = client.Inbox.Search(SearchQuery.SentSince(DateTime.Now.AddDays(-7)));

                    var messages = client.Inbox.Fetch(uids, MessageSummaryItems.Envelope | MessageSummaryItems.BodyStructure);

                    if (messages != null && messages.Count > 0)
                    {
                        foreach(var msg in messages)
                        {
                            list.Add(new MailListItems
                            {
                                Date = msg.Date.ToString(),
                                From = msg.Envelope.From.ToString(),
                                Subj = msg.Envelope.Subject,
                                HasAttachments = msg.Attachments != null && msg.Attachments.Count() > 0,
                            });
                                
                            foreach(var att in msg.Attachments.OfType<BodyPartBasic>())
                            {
                                var part = (MimePart)client.Inbox.GetBodyPart(msg.UniqueId, att);

                                var pathDir = Path.Combine(Environment.CurrentDirectory, "Emails", msg.UniqueId.ToString());
                                if(!Directory.Exists(pathDir))
                                {
                                    Directory.CreateDirectory(pathDir);
                                }

                                var path = Path.Combine(pathDir, part.FileName);
                                if(!File.Exists(path))
                                {
                                    using(var strm = File.Create(path))
                                    {
                                        part.Content.DecodeTo(strm);
                                    }
                                }
                            }
                              
                        }
                    }

                }
            }

            catch (Exception ex) { }

            return list;
            
        }
    }
}
 
