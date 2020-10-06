using System.Collections.Generic;

namespace PictureBot
{
    /// <summary>
    /// Class for storing conversation data. 
    /// </summary>
    public class ConversationInfo : Dictionary<string, object> { }

    /// <summary>
    /// Class for storing user data in the conversation. 
    /// </summary>
    public class UserData
    {

        public string Greeted { get; set; } = "not greeted";
        // A list of things that users have said to the bot
        public List<string> UtteranceList { get; private set; } = new List<string>();
    }
}
