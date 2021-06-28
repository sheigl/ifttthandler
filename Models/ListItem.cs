using System;
using System.Text.Json;

namespace ifttthandler.Models 
{
    public class ListItem 
    {
        public string Item { get; set; }
        public Action? Action { get; set; }
        public string ActionDate { get; set; }
        public string ApiKey { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}