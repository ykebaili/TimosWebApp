
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
			public const string ChampTimos = "ChampTimos";
			public const string ValeursChamp = "ValeursChamp";
			public const string TodoValeurChamp = "TodoValeurChamp";
		}

		public static partial class Relations
		{
			public const string ValeursPossibles = "ValeursPossibles";
			public const string RelationTodoDefinitionChamp = "RelationTodoDefinitionChamp";
			public const string RelationTodoValeurChamp = "RelationTodoValeurChamp";
		}
	}

	[SchemaNamespace]
	public class DomainProvider : INamespace
	{
		public string Name { get { return GetType().Namespace; } }
		public static string DomainName { get { return new DomainProvider().Name; } }
	}


	[DataDefinition(MustPersist = false)]
	public enum TypeDonnee
	{
		[Description("TypeEntier")]
		TypeEntier,
		[Description("TypeDecimal")]
		TypeDecimal		 = 		1,
		[Description("TypeString")]
		TypeString		 = 		2,
		[Description("TypeDate")]
		TypeDate		 = 		3,
		[Description("TypeBool")]
		TypeBool		 = 		4,
		[Description("ObjetTimos")]
		ObjetTimos		 = 		5
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
			public const string ElementType = "ElementType";
			public const string ElementId = "ElementId";
			public const string ElementDescription = "ElementDescription";
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

		[Data]
		public string ElementType
		{
			get { return getValue<string>("ElementType"); }
			set { setValue<string>("ElementType", value); }
		}

		[Data]
		public int ElementId
		{
			get { return getValue<int>("ElementId"); }
			set { setValue<int>("ElementId", value); }
		}

		[Data]
		public string ElementDescription
		{
			get { return getValue<string>("ElementDescription"); }
			set { setValue<string>("ElementDescription", value); }
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

	[DataDefinition]
	public class ChampTimos : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Label = "Label";
			public const string DisplayOrder = "DisplayOrder";
			public const string TimosId = "TimosId";
			public const string TypeDonneChamp = "TypeDonneChamp";
			public const string AspectizeControlType = "AspectizeControlType";
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
		public int DisplayOrder
		{
			get { return getValue<int>("DisplayOrder"); }
			set { setValue<int>("DisplayOrder", value); }
		}

		[Data(IsPrimaryKey = true)]
		public int TimosId
		{
			get { return getValue<int>("TimosId"); }
			set { setValue<int>("TimosId", value); }
		}

		[Data(DefaultValue = 2)]
		public TypeDonnee TypeDonneChamp
		{
			get { return getValue<TypeDonnee>("TypeDonneChamp"); }
			set { setValue<TypeDonnee>("TypeDonneChamp", value); }
		}

		[Data]
		public string AspectizeControlType
		{
			get { return getValue<string>("AspectizeControlType"); }
			set { setValue<string>("AspectizeControlType", value); }
		}

	}

	[DataDefinition]
	public class ValeursChamp : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Index = "Index";
			public const string StoredValue = "StoredValue";
			public const string DisplayedValue = "DisplayedValue";
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
		public int Index
		{
			get { return getValue<int>("Index"); }
			set { setValue<int>("Index", value); }
		}

		[Data]
		public string StoredValue
		{
			get { return getValue<string>("StoredValue"); }
			set { setValue<string>("StoredValue", value); }
		}

		[Data]
		public string DisplayedValue
		{
			get { return getValue<string>("DisplayedValue"); }
			set { setValue<string>("DisplayedValue", value); }
		}

	}

	[DataDefinition]
	public class TodoValeurChamp : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string ValeurChamp = "ValeurChamp";
			public const string LibelleChamp = "LibelleChamp";
			public const string OrdreChamp = "OrdreChamp";
			public const string ChampTimosId = "ChampTimosId";
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
		public string ValeurChamp
		{
			get { return getValue<string>("ValeurChamp"); }
			set { setValue<string>("ValeurChamp", value); }
		}

		[Data]
		public string LibelleChamp
		{
			get { return getValue<string>("LibelleChamp"); }
			set { setValue<string>("LibelleChamp", value); }
		}

		[Data]
		public int OrdreChamp
		{
			get { return getValue<int>("OrdreChamp"); }
			set { setValue<int>("OrdreChamp", value); }
		}

		[Data]
		public int ChampTimosId
		{
			get { return getValue<int>("ChampTimosId"); }
			set { setValue<int>("ChampTimosId", value); }
		}

	}

	[DataDefinition]
	public class ValeursPossibles : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(ChampTimos), Role = typeof(ChampTimos), Multiplicity = Multiplicity.ZeroOrOne)]
		public IEntity ChampTimos;

		[RelationEnd(Type = typeof(ValeursChamp), Role = typeof(ValeursChamp), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity ValeursChamp;

	}

	[DataDefinition]
	public class RelationTodoDefinitionChamp : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(ChampTimos), Role = typeof(ChampTimos), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity ChampTimos;

		[RelationEnd(Type = typeof(Todos), Role = typeof(Todos), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Todos;

	}

	[DataDefinition]
	public class RelationTodoValeurChamp : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Todos), Role = typeof(Todos), Multiplicity = Multiplicity.One)]
		public IEntity Todos;

		[RelationEnd(Type = typeof(TodoValeurChamp), Role = typeof(TodoValeurChamp), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity TodoValeurChamp;

	}

}


  
