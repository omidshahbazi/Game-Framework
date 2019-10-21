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

        public Flags FlagMask
        {
            get;
            private set;
        }

        public object DefaultValue
        {
            get;
            private set;
        }

        public DataType DataType
		{
            get;
            private set;
        }

        public Column(string Name, DataType DataType)
        {
            this.Name = Name;
            this.DataType = DataType;
        }

        public Column(string Name, DataType DataType, Flags FlagMask) : this(Name, DataType)
        {
            this.FlagMask = FlagMask;
        }

		public Column(string name, DataType type, object DefaultValue) : this(name, type)
		{
			this.DefaultValue = DefaultValue;
		}

        public Column(string name, DataType type, Flags FlagMask, object DefaultValue) : this(name, type, FlagMask)
        {
            this.DefaultValue = DefaultValue;
        }
	}
}