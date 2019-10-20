// Copyright 2019. All Rights Reserved.


namespace GameFramework.DatabaseManaged.Generator
{
    public class Column
    {
        public string Name
        {
            get;
            private set;
        }

        public Constraints Flags
        {
            get;
            private set;
        }

        public object DefaultValue
        {
            get;
            private set;
        }

        public SQLType Type
        {
            get;
            private set;
        }

        public int Length
        {
            get;
            set;
        }

        public Column(string name, SQLType type)
        {
            this.Name = name;
            this.Type = type;
            this.Length = name.Length;
        }

        public Column(string name, SQLType type, Constraints flags) : this(name, type)
        {
            this.Flags = flags;
        }

        public Column(string name, SQLType type, Constraints flags, object defaultValue) : this(name, type, flags)
        {
            this.DefaultValue = defaultValue;
        }

		public Column(string name, SQLType type, object defaultValue) : this(name, type)
		{
			this.DefaultValue = defaultValue;
		}
	}
}