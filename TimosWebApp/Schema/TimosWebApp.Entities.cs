
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

using Aspectize.Core;

[assembly:AspectizeDALAssemblyAttribute]

namespace TimosWebApp
{
	public static partial class SchemaNames
	{
		public static partial class Entities
		{
			public const string Todos = "Todos";
			public const string User = "User";
		}
	}

	[SchemaNamespace]
	public class DomainProvider : INamespace
	{
		public string Name { get { return GetType().Namespace; } }
		public static string DomainName { get { return new DomainProvider().Name; } }
	}


	[DataDefinition]
	public class Todos : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Label = "Label";
			public const string StartDate = "StartDate";
			public const string Instructions = "Instructions";
			public const string TimosId = "TimosId";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data]
		public string Label
		{
			get { return getValue<string>("Label"); }
			set { setValue<string>("Label", value); }
		}

		[Data]
		public DateTime StartDate
		{
			get { return getValue<DateTime>("StartDate"); }
			set { setValue<DateTime>("StartDate", value); }
		}

		[Data]
		public string Instructions
		{
			get { return getValue<string>("Instructions"); }
			set { setValue<string>("Instructions", value); }
		}

		[Data(IsPrimaryKey = true)]
		public int TimosId
		{
			get { return getValue<int>("TimosId"); }
			set { setValue<int>("TimosId", value); }
		}

	}

	[DataDefinition]
	public class User : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Name = "Name";
			public const string IsAuthentificated = "IsAuthentificated";
			public const string Login = "Login";
			public const string TimosKey = "TimosKey";
			public const string TimosSessionId = "TimosSessionId";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data]
		public string Name
		{
			get { return getValue<string>("Name"); }
			set { setValue<string>("Name", value); }
		}

		[Data(DefaultValue = false)]
		public bool IsAuthentificated
		{
			get { return getValue<bool>("IsAuthentificated"); }
			set { setValue<bool>("IsAuthentificated", value); }
		}

		[Data]
		public string Login
		{
			get { return getValue<string>("Login"); }
			set { setValue<string>("Login", value); }
		}

		[Data]
		public string TimosKey
		{
			get { return getValue<string>("TimosKey"); }
			set { setValue<string>("TimosKey", value); }
		}

		[Data(DefaultValue = -1)]
		public int TimosSessionId
		{
			get { return getValue<int>("TimosSessionId"); }
			set { setValue<int>("TimosSessionId", value); }
		}

	}

}


  
