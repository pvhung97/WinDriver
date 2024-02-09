namespace UIADriver.actions.action
{
    public abstract class Action
    {
        public string id;
        public string type;
        public string subtype;

        public Action(string id, string type, string subtype) 
        {
            this.id = id;
            this.type = type;
            this.subtype = subtype;
        }
    }
}
